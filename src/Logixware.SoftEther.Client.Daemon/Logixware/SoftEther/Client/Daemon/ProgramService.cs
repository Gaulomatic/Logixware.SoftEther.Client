using System;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;

using System.Reactive.Subjects;

using System.Collections.Generic;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Logixware.SoftEther.Client.VpnService;

namespace Logixware.SoftEther.Client.Daemon
{
	public class ProgramService : IHostedService
	{
		private readonly ILogger<ProgramService> _Logger;
		private readonly ILogger<VirtualNetworkService> _NetworkLogger;
		private readonly IApplicationLifetime _AppLifetime;
		private readonly IClientConfiguration _Configuration;
		private readonly ICommandLineInterface _Cli;
		private readonly IConnectionVerifier _ConnectionVerifier;
		private readonly IPlatform _Platform;

		private readonly Subject<Object> _ClientServiceRestarting;
		private readonly Subject<Object> _ClientServiceRestarted;

		private readonly Dictionary<VirtualNetworkService, Int32> _Networks;
		private Boolean? _IsInternetConnected;
		private Boolean _Run;

		private Timer _Timer;

		public ProgramService
		(
			ILogger<ProgramService> logger,
			ILogger<VirtualNetworkService> networklogger,
			IApplicationLifetime appLifetime,
			IClientConfiguration configuration,
			ICommandLineInterface cli,
			IConnectionVerifier connectionVerifier,
			IPlatform platform
		)
		{
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._NetworkLogger = networklogger ?? throw new ArgumentNullException(nameof(networklogger));
			this._AppLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
			this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this._Cli = cli ?? throw new ArgumentNullException(nameof(cli));
			this._ConnectionVerifier = connectionVerifier ?? throw new ArgumentNullException(nameof(connectionVerifier));
			this._Platform = platform ?? throw new ArgumentNullException(nameof(platform));

			this._ClientServiceRestarting = new Subject<Object>();
			this._ClientServiceRestarted = new Subject<Object>();

			this._Networks = new Dictionary<VirtualNetworkService, Int32>();
			this._IsInternetConnected = null;
			this._Run = false;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			this._AppLifetime.ApplicationStarted.Register(this.OnStarted);
			this._AppLifetime.ApplicationStopping.Register(this.OnStopping);
			this._AppLifetime.ApplicationStopped.Register(this.OnStopped);

			return Task.CompletedTask;
		}

		private void OnStarted()
		{
			this._Logger.Inform("Starting application...");

			var __StartTimeSpan = TimeSpan.Zero;
			var __PeriodTimeSpan = TimeSpan.FromSeconds(5);

			var __ValidNetworks = this._Configuration.GetValidNetworks(this._Cli).ToList();

			if (__ValidNetworks.Count == 0)
			{
				this._Logger.Critical("No valid network found.");
				this._AppLifetime.StopApplication();

				return;
			}

			this._Logger.Inform("Initializing platform.");
			this._Platform.Initialize();

			this._Logger.Inform("Starting VPN client service...");
			this._Cli.StartClient();

			foreach (var __Network in __ValidNetworks)
			{
				this._Networks.Add(new VirtualNetworkService(

					this._NetworkLogger,
					this._Cli,
					this._ConnectionVerifier,
					this._Platform,
					this._ClientServiceRestarting,
					this._ClientServiceRestarted,
					__Network

				), 0);
			}

			this._Run = true;
			this._Timer = new Timer(this.Tick, null, __StartTimeSpan, __PeriodTimeSpan);
		}

		private async void Tick(Object parameter)
		{
			if (!this._Run)
			{
				this._Timer.Dispose();
				this._AppLifetime.StopApplication();

				return;
			}

			if (await InternetConnection.IsAvailibleAsync(this._Configuration.Settings.InternetConnectionTestUrl).ConfigureAwait(false))
			{
				if (this._IsInternetConnected == null || !(Boolean) this._IsInternetConnected)
				{
					this._IsInternetConnected = true;
					this._Logger.Inform("Connected to the internet");
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
					.Where(p => p.Value >= this._Configuration.Settings.ConnectionAttemptsBeforeClientRestart)
					.Select(p => p.Key)
					.ToList();

				__NonReachableServices.ForEach(r =>
				{
					this._Logger.Error($"VPN \"{r.Network.Configuration.Name}\": Connection test host \"{r.Network.Configuration.ConnectionTestHost}\" did not respond {this._Networks[r.Network]} times.");
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
					this._Logger.Inform("Not connected to the internet");
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
			this._Logger.Warn("Restarting the VPN client...");

			this._ClientServiceRestarting.OnNext(null);

			this._Cli.RestartClient();
			this.ResetAttemptCounters(this._Networks.Keys);

			this._ClientServiceRestarted.OnNext(null);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			this._Logger.Inform("Shutting down application...");
			this._Run = false;

			return Task.CompletedTask;
		}

		private void OnStopping()
		{
			this._Logger.Inform("Stoppping VPN client service...");
			this._Cli.StopClient();
		}

		private void OnStopped()
		{
			this._Logger.Inform("Application shut down. C ya.");
		}

//		private Task Sleep(int millisecondsTimeout)
//		{
//			var taskCompletionSource = new TaskCompletionSource<bool>();
//
//			ThreadPool.QueueUserWorkItem((state) =>
//			{
//				Thread.Sleep(millisecondsTimeout);
//				taskCompletionSource.SetResult(true);
//			}, null);
//
//			return taskCompletionSource.Task;
//		}
	}
}
