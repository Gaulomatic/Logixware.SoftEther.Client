using System;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;

using System.Reactive.Subjects;
using System.Collections.Generic;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Logixware.SoftEther.Client.VpnService;

using Logixware.SoftEther.Client.Daemon.Options;
using Logixware.SoftEther.Client.Daemon.Platform;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public class ClientService : IClientService, IDisposable
	{
		private readonly ILogger<ClientService> _Logger;
		private readonly ILogger<VirtualNetworkService> _NetworkLogger;
		private readonly IHostApplicationLifetime _AppLifetime;
		private readonly IOptions<GeneralOptions> _GeneralOptions;
		private readonly IOptions<NetworkOptions> _NetworkOptions;
		private readonly ICommandLineInterface _Cli;
		private readonly IVpnConnectionVerifier _VpnConnectionVerifier;
		private readonly IInternetConnectionVerifier _InternetConnectionVerifierVerifier;
		private readonly IPlatform _Platform;

		private readonly Subject<Object> _ClientServiceRestarting;
		private readonly Subject<Object> _ClientServiceRestarted;

		private readonly Dictionary<VirtualNetworkService, Int32> _Networks;
		private Boolean? _IsInternetConnected;

		private CancellationTokenSource _RunCancellationTokenSource;
		private Boolean _IsRunning;
		private Task _RunTask;

		public ClientService
		(
			ILogger<ClientService> logger,
			ILogger<VirtualNetworkService> networkLogger,
			IHostApplicationLifetime appLifetime,
			IOptions<GeneralOptions> generalOptions,
			IOptions<NetworkOptions> networkOptions,
			ICommandLineInterface cli,
			IInternetConnectionVerifier internetConnectionVerifierVerifier,
			IVpnConnectionVerifier vpnConnectionVerifier,
			IPlatform platform
		)
		{
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._NetworkLogger = networkLogger ?? throw new ArgumentNullException(nameof(networkLogger));
			this._AppLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
			this._GeneralOptions = generalOptions ?? throw new ArgumentNullException(nameof(generalOptions));
			this._NetworkOptions = networkOptions ?? throw new ArgumentNullException(nameof(networkOptions));
			this._Cli = cli ?? throw new ArgumentNullException(nameof(cli));
			this._InternetConnectionVerifierVerifier = internetConnectionVerifierVerifier ?? throw new ArgumentNullException(nameof(internetConnectionVerifierVerifier));
			this._VpnConnectionVerifier = vpnConnectionVerifier ?? throw new ArgumentNullException(nameof(vpnConnectionVerifier));
			this._Platform = platform ?? throw new ArgumentNullException(nameof(platform));

			this._ClientServiceRestarting = new Subject<Object>();
			this._ClientServiceRestarted = new Subject<Object>();

			this._Networks = new Dictionary<VirtualNetworkService, Int32>();
			this._IsInternetConnected = null;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			if (this._IsRunning)
			{
				throw new InvalidOperationException("Service is already running.");
			}

			this._IsRunning = true;
			this._RunCancellationTokenSource = new CancellationTokenSource();

			this._Logger?.Inform("Initializing platform.");
			this._Platform.Initialize();

			this._Logger?.Inform("Stopping VPN client service, if currently running...");
			this._Cli.StopClient();

			this._Logger?.Inform("Starting VPN client service...");
			this._Cli.StartClient();

			var __ValidNetworks = this._NetworkOptions.Value.GetValidNetworks(this._Cli).ToList();

			if (__ValidNetworks.Count == 0)
			{
				this._Logger?.Critical("No valid network found.");
				this._AppLifetime.StopApplication();

				return Task.CompletedTask;
			}

			foreach (var __Network in __ValidNetworks)
			{
				this._Networks.Add(new VirtualNetworkService(

					this._NetworkLogger,
					this._Cli,
					this._VpnConnectionVerifier,
					this._Platform,
					this._ClientServiceRestarting,
					this._ClientServiceRestarted,
					__Network

				), 0);
			}

			this._RunTask = Task.Run(async () =>
			{
				while (true)
				{
					await Task.Delay(TimeSpan.FromSeconds(5), this._RunCancellationTokenSource.Token);
					await this.TickAsync(this._RunCancellationTokenSource.Token);
				}

				// ReSharper disable once FunctionNeverReturns
			}, this._RunCancellationTokenSource.Token);

			return Task.CompletedTask;
		}

		private async Task TickAsync(CancellationToken cancellationToken)
		{
			var __IsOnline = await this._InternetConnectionVerifierVerifier

				.IsAvailableAsync(this._GeneralOptions.Value.InternetConnectionTestUrl)
				.ConfigureAwait(false);

			if (__IsOnline)
			{
				if (this._IsInternetConnected == null || !(Boolean) this._IsInternetConnected)
				{
					this._IsInternetConnected = true;
					this._Logger?.Inform("Connected to the internet");
				}

				var __Results = new List<ReachableResult>();

				foreach (var __VirtualNetworkService in this._Networks.Keys.ToList())
				{
					__Results.Add(await __VirtualNetworkService.IsReachableAsync().ConfigureAwait(false));
				}

				var __ReachableServices = __Results

					.Where(r => r.Configuration != ConfigurationState.Error)
					.Where(r => r.Connection == ConnectionState.Connected)
					.Where(r => r.Reachable == ReachableState.Reachable)
					.ToList();

				var __DisconnectedServices = __Results

					.Where(r => r.Configuration != ConfigurationState.Error)
					.Where(r => r.Connection == ConnectionState.Disconnected)
					.ToList();

				var __NonReachableServices = __Results

					.Where(r => r.Network.Configuration.AlwaysOn)
					.Where(r => r.Configuration != ConfigurationState.Error)
					.Where(r => r.Connection != ConnectionState.Disconnected)
					.Where(r => r.Reachable == ReachableState.Unreachable)
					.ToList();

				var __ServicesThatShouldBeReachable = __Results

					.Where(r => r.Network.Configuration.AlwaysOn)
					.Where(r => r.Configuration != ConfigurationState.Error)
					.Where(r => r.Connection != ConnectionState.Disconnected)
					.ToList();

				__ServicesThatShouldBeReachable.ForEach(s => this._Networks[s.Network] += 1);

				this.ResetAttemptCounters(__ReachableServices.Select(x => x.Network));

				var __OverdueNetworks = this._Networks

					.ToList()
					.Where(p => p.Value >= this._GeneralOptions.Value.ConnectionAttemptsBeforeClientRestart)
					.Select(p => p.Key)
					.ToList();

				__NonReachableServices.ForEach(r =>
				{
					this._Logger?.Error($"VPN \"{r.Network.Configuration.Name}\": Connection test host \"{r.Network.Configuration.ConnectionTestHost}\" did not respond {this._Networks[r.Network]} times.");
				});

				if (__ReachableServices.Count == 0 && __DisconnectedServices.Count == 0)
				{
					this.RestartClientService();
				}
				else if (__ReachableServices.Count == 0 && __OverdueNetworks.Count > 0 && __OverdueNetworks.Count >= __ServicesThatShouldBeReachable.Count)
				{
					this.RestartClientService();
				}
			}
			else
			{
				if (this._IsInternetConnected == null || (Boolean) this._IsInternetConnected)
				{
					this._IsInternetConnected = false;
					this._Logger?.Inform("Not connected to the internet");
				}

				this.ResetAttemptCounters(this._Networks.Keys);
			}
		}

		private void ResetAttemptCounters(IEnumerable<VirtualNetworkService> items)
		{
			foreach (var __VirtualNetwork in items.ToList())
			{
				this._Networks[__VirtualNetwork] = 0;
			}
		}

		private void RestartClientService()
		{
			this._Logger?.Warn("Restarting the VPN client...");

			this._ClientServiceRestarting.OnNext(null);

			this._Cli.RestartClient();
			this.ResetAttemptCounters(this._Networks.Keys);

			this._ClientServiceRestarted.OnNext(null);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			if (!this._IsRunning)
			{
				throw new InvalidOperationException("Service not running.");
			}

			this._RunCancellationTokenSource.Cancel();
			this._IsRunning = false;

			this._Cli.StopClient();

			if (this._RunTask != null && this._RunTask.Status == TaskStatus.Running)
			{
				await this._RunTask;
			}
		}

		public void Dispose()
		{
			this._ClientServiceRestarting?.Dispose();
			this._ClientServiceRestarted?.Dispose();
			this._RunCancellationTokenSource?.Dispose();
		}
	}
}
