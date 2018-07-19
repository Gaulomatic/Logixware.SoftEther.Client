using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.Extensions.Logging;

using Logixware.SoftEther.Client.VpnService;
using Logixware.SoftEther.Client.Daemon.Entities;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public class VirtualNetworkService : IDisposable
	{
		private readonly ILogger<VirtualNetworkService> _Logger;
		private readonly ICommandLineInterface _Cli;
		private readonly IVpnConnectionVerifier _VpnConnectionVerifier;
		private readonly IPlatform _Platform;

		private readonly Subject<Object> _IsDisposed;

		private readonly BehaviorSubject<String> _InterfaceName;
		private readonly BehaviorSubject<Account> _Account;
		private readonly BehaviorSubject<Device> _Device;

		private readonly BehaviorSubject<Boolean?> _IPv4AddressAssigned;
		private readonly BehaviorSubject<Boolean?> _IPv6AddressAssigned;

		private readonly BehaviorSubject<Boolean?> _IPv4RoutesAssigned;
		private readonly BehaviorSubject<Boolean?> _IPv6RoutesAssigned;

		private readonly BehaviorSubject<ConnectionVerificationResult> _ConnectionVerificationResult;

		private readonly BehaviorSubject<ConfigurationState?> _ConfigurationState;
		private readonly BehaviorSubject<ConnectionState?> _ConnectionState;
		private readonly BehaviorSubject<ReachableState?> _ReachableState;

		public VirtualNetworkService
		(
			ILogger<VirtualNetworkService> logger,
			ICommandLineInterface cli,
			IVpnConnectionVerifier vpnConnectionVerifier,
			IPlatform platform,
			Subject<Object> clientServiceRestarting,
			Subject<Object> clientServiceRestarted,
			VirtualNetwork network
		)
		{
			if (clientServiceRestarting == null) throw new ArgumentNullException(nameof(clientServiceRestarting));
			if (clientServiceRestarted == null) throw new ArgumentNullException(nameof(clientServiceRestarted));

			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._Cli = cli ?? throw new ArgumentNullException(nameof(cli));
			this._VpnConnectionVerifier = vpnConnectionVerifier ?? throw new ArgumentNullException(nameof(vpnConnectionVerifier));
			this._Platform = platform ?? throw new ArgumentNullException(nameof(platform));
			this.Configuration = network ?? throw new ArgumentNullException(nameof(network));

			this._IsDisposed = new Subject<Object>();

			var __IsInitialized = new Subject<Object>();

			clientServiceRestarting

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.Subscribe(this.OnClientServiceRestarting);

			clientServiceRestarted

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.Subscribe(this.OnClientServiceRestarted);

			this._Account = new BehaviorSubject<Account>(null);
			this._Account

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.DistinctUntilChanged()
				.Subscribe(this.OnAccountFoundChanged);

			this._Device = new BehaviorSubject<Device>(null);
			this._Device

				.SkipUntil(__IsInitialized)
				.TakeUntil(this._IsDisposed)
				.DistinctUntilChanged()
				.Subscribe(this.OnDeviceFoundChanged);

			this._InterfaceName = new BehaviorSubject<String>(null);
			this._InterfaceName

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.DistinctUntilChanged()
				.Subscribe(this.OnInterfaceFoundChanged);

			this._IPv4AddressAssigned = new BehaviorSubject<Boolean?>(null);
			this._IPv4AddressAssigned

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnIPv4AddressAssignedChanged, this.OnIPv4AddressAssignmentError);

			this._IPv6AddressAssigned = new BehaviorSubject<Boolean?>(null);
			this._IPv6AddressAssigned

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnIPv6AddressAssignedChanged, this.OnIPv6AddressAssignmentError);

			this._IPv4RoutesAssigned = new BehaviorSubject<Boolean?>(null);
			this._IPv4RoutesAssigned

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnIPv4RoutesAppliedChanged, this.OnIPv4RoutesAssignmentError);

			this._IPv6RoutesAssigned = new BehaviorSubject<Boolean?>(null);
			this._IPv6RoutesAssigned

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnIPv6RoutesAppliedChanged, this.OnIPv6RoutesAssignmentError);

			this._ConfigurationState = new BehaviorSubject<ConfigurationState?>(null);
			this._ConfigurationState

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnConfigurationStateChanged);

			this._ConnectionState = new BehaviorSubject<ConnectionState?>(null);
			this._ConnectionState

				.TakeUntil(this._IsDisposed)
				.SkipUntil(__IsInitialized)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnConnectionStateChanged);

			this._ReachableState = new BehaviorSubject<ReachableState?>(null);
			this._ReachableState

				.TakeUntil(this._IsDisposed)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnReachableStateChanged);

			this._ConnectionVerificationResult = new BehaviorSubject<ConnectionVerificationResult>(null);
			this._ConnectionVerificationResult

				.TakeUntil(this._IsDisposed)
				.NotNull()
				.DistinctUntilChanged()
				.Subscribe(this.OnConnectionVerificationResultChanged);

			__IsInitialized.OnNext(null);
		}

		public VirtualNetwork Configuration { get; }

		private void OnClientServiceRestarting(Object e)
		{
			this._ConnectionState.OnNext(ConnectionState.Disconnected);
			this._ReachableState.OnNext(ReachableState.Unreachable);
		}

		private void OnClientServiceRestarted(Object e)
		{
		}

		private void OnConfigurationStateChanged(ConfigurationState? state)
		{
			if (state == ConfigurationState.OK)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": Interface configuration completed.");
			}
			else
			{
				this._Logger?.Warn($"VPN \"{this.Configuration.Name}\": Interface configuration error.");
			}
		}

		private void OnConnectionStateChanged(ConnectionState? state)
		{
			if (state == ConnectionState.Connected)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": Connection is established.");
				this.AssignIPAddresses();
			}
			else
			{
				this._Logger?.Warn($"VPN \"{this.Configuration.Name}\": Connection is not established, State: \"{state}\".");
				this.ReleaseIPAddresses();
			}
		}

		private void OnReachableStateChanged(ReachableState? state)
		{
			if (state == ReachableState.Reachable)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": Network is reachable.");
			}
			else
			{
				this._Logger?.Warn($"VPN \"{this.Configuration.Name}\": Network is not reachable.");
			}
		}

		private void OnAccountFoundChanged(Account value)
		{
			if (value != null)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": Account found in the VPN client service.");
			}
			else
			{
				this._Logger?.Warn($"VPN \"{this.Configuration.Name}\": Account not found in the VPN client service.");
			}
		}

		private void OnDeviceFoundChanged(Device value)
		{
			if (value != null)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": Device \"{this._Account.Value?.DeviceName}\" in the VPN client service found.");
			}
			else
			{
				this._Logger?.Warn($"VPN \"{this.Configuration.Name}\": Device \"{this._Account.Value?.DeviceName}\" in the VPN client service not found.");
			}
		}

		private void OnInterfaceFoundChanged(String value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": Interface \"{value} found.");
			}
			else
			{
				this._Logger?.Warn($"VPN \"{this.Configuration.Name}\": No interface for physical address \"{this._Device.Value?.PhysicalAddress}\" found.");
			}
		}

		private void OnIPv4AddressAssignedChanged(Boolean? value)
		{
			if (value == null || this.Configuration.IPv4 == null) return;

			if ((Boolean) value)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IP address \"{this.Configuration.IPv4.Address}\" assigned to adapter \"{this._InterfaceName.Value}\".");
				this.AssignIPv4Routes();
			}
			else
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IP address \"{this.Configuration.IPv4.Address}\" released from adapter \"{this._InterfaceName.Value}\".");
				this.ReleaseIPv4Routes();
			}
		}

		private void OnIPv4AddressAssignmentError(Exception ex)
		{
			if (this.Configuration.IPv4 == null) return;
			this._Logger?.Error($"VPN \"{this.Configuration.Name}\": Error assigning or releasing IP address \"{this.Configuration.IPv4.Address}\" on adapter \"{this._InterfaceName.Value}\": {ex.Message}");
		}

		private void OnIPv6AddressAssignedChanged(Boolean? value)
		{
			if (value == null || this.Configuration.IPv6 == null) return;

			if ((Boolean) value)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IP address \"{this.Configuration.IPv6.Address}/{this.Configuration.IPv6.Prefix}\" assigned to adapter \"{this._InterfaceName.Value}\".");
				this.AssignIPv6Routes();
			}
			else
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IP address \"{this.Configuration.IPv6.Address}/{this.Configuration.IPv6.Prefix}\" released from adapter \"{this._InterfaceName.Value}\".");
				this.ReleaseIPv6Routes();
			}
		}

		private void OnIPv6AddressAssignmentError(Exception ex)
		{
			if (this.Configuration.IPv6 == null) return;
			this._Logger?.Error($"VPN \"{this.Configuration.Name}\": Error assigning or releasing IP address \"{this.Configuration.IPv6.Address}/{this.Configuration.IPv6.Prefix}\" on adapter \"{this._InterfaceName.Value}\": {ex.Message}");
		}

		private void OnIPv4RoutesAppliedChanged(Boolean? value)
		{
			if (value == null) return;

			if ((Boolean) value)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IPv4 routes assigned.");
				this.AssignIPv6Routes();
			}
			else
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IPv4 routes released.");
				this.ReleaseIPv6Routes();
			}
		}

		private void OnIPv4RoutesAssignmentError(Exception ex)
		{
			this._Logger?.Error($"VPN \"{this.Configuration.Name}\": Error assigning or releasing IPv4 routes: {ex.Message}");
		}

		private void OnIPv6RoutesAppliedChanged(Boolean? value)
		{
			if (value == null) return;

			if ((Boolean) value)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IPv6 routes assigned.");
				this.AssignIPv6Routes();
			}
			else
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": IPv6 routes released.");
				this.ReleaseIPv6Routes();
			}
		}

		private void OnIPv6RoutesAssignmentError(Exception ex)
		{
			this._Logger?.Error($"VPN \"{this.Configuration.Name}\": Error assigning or releasing IPv6 routes: {ex.Message}");
		}

		private void OnConnectionVerificationResultChanged(ConnectionVerificationResult value)
		{
			if (value == null) return;
			if (value.Reachable == ReachableState.Reachable)
			{
				this._Logger?.Inform($"VPN \"{this.Configuration.Name}\": Connection test host \"{this.Configuration.ConnectionTestHost}\" successfully reached: \"{value.Details}\".");
			}
			else
			{
				this._Logger?.Error($"VPN \"{this.Configuration.Name}\": Error reaching connection test host \"{this.Configuration.ConnectionTestHost}\": \"{value.Details}\".");
			}
		}

		public async Task<ReachableResult> IsReachableAsync()
		{
			// ReSharper disable once InconsistentNaming
			var ReturnResult = (Func<Task<ReachableResult>>) (() => Task.FromResult(new ReachableResult(

				this,
				// ReSharper disable once PossibleInvalidOperationException
				(ConfigurationState) this._ConfigurationState.Value,
				// ReSharper disable once PossibleInvalidOperationException
				(ConnectionState) this._ConnectionState.Value,
				// ReSharper disable once PossibleInvalidOperationException
				(ReachableState) this._ReachableState.Value)));

			var __ConfigurationState = await this.ConfigureInterfaceAsync().ConfigureAwait(false);
			this._ConfigurationState.OnNext(__ConfigurationState);

			if (__ConfigurationState == ConfigurationState.Error)
			{
				this._ConfigurationState.OnNext(ConfigurationState.Error);
				this._ConnectionState.OnNext(ConnectionState.Disconnected);
				this._ReachableState.OnNext(ReachableState.Unreachable);
				return await ReturnResult().ConfigureAwait(false);
			}

			var __ConnectionState = this._Cli.GetAccountStatus(this.Configuration.Name).ConnectionState;
			this._ConnectionState.OnNext(__ConnectionState);

			if (__ConnectionState != ConnectionState.Connected)
			{
				this._ReachableState.OnNext(ReachableState.Unreachable);
				return await ReturnResult().ConfigureAwait(false);
			}

			this._ConnectionVerificationResult.OnNext(this._VpnConnectionVerifier.Verify(this.Configuration.ConnectionTestHost));
			this._ReachableState.OnNext(this._ConnectionVerificationResult.Value.Reachable);

			return await ReturnResult().ConfigureAwait(false);
		}

		private async Task<ConfigurationState> ConfigureInterfaceAsync()
		{
			this._Account.OnNext(this._Cli.GetAccount(this.Configuration.Name));

			if (this._Account.Value == null)
			{
				return await Task.FromResult(ConfigurationState.Error).ConfigureAwait(false);
			}

			this._Device.OnNext(this._Cli.GetDevice(this._Account.Value.DeviceName));

			if (this._Device.Value == null)
			{
				return await Task.FromResult(ConfigurationState.Error).ConfigureAwait(false);
			}

			var __Interface = NetworkInterface

				.GetAllNetworkInterfaces()
				.SingleOrDefault(i => String.Equals(i.GetPhysicalAddress().ToString(), this._Device.Value.PhysicalAddress, StringComparison.OrdinalIgnoreCase));

			if (__Interface == null)
			{
				return await Task.FromResult(ConfigurationState.Error).ConfigureAwait(false);
			}

			this._InterfaceName.OnNext(__Interface.Name);

			return await Task.FromResult(ConfigurationState.OK).ConfigureAwait(false);
		}

		// ReSharper disable once InconsistentNaming
		private void AssignIPAddresses()
		{
			if (this.Configuration.IPv4 != null && !this.HasAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv4.Address))
			{
				var __Execution = this._Platform.AssignIPAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv4);

				if (__Execution.Succeeded)
				{
					this._IPv4AddressAssigned.OnNext(true);
				}
				else
				{
					this._IPv4AddressAssigned.OnError(new Exception(__Execution.Result));
				}
			}

			if (this.Configuration.IPv6 != null && !this.HasAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv6.Address))
			{
				var __Execution = this._Platform.AssignIPAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv6);

				if (__Execution.Succeeded)
				{
					this._IPv6AddressAssigned.OnNext(true);
				}
				else
				{
					this._IPv6AddressAssigned.OnError(new Exception(__Execution.Result));
				}
			}
		}

		// ReSharper disable once InconsistentNaming
		private void ReleaseIPAddresses()
		{
			if (this.Configuration.IPv4 != null && this.HasAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv4.Address))
			{
				var __Execution = this._Platform.ReleaseIPAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv4);

				if (__Execution.Succeeded)
				{
					this._IPv4AddressAssigned.OnNext(false);
				}
				else
				{
					this._IPv4AddressAssigned.OnError(new Exception(__Execution.Result));
				}
			}

			if (this.Configuration.IPv6 != null && this.HasAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv6.Address))
			{
				var __Execution = this._Platform.ReleaseIPAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv6);

				if (__Execution.Succeeded)
				{
					this._IPv6AddressAssigned.OnNext(false);
				}
				else
				{
					this._IPv6AddressAssigned.OnError(new Exception(__Execution.Result));
				}
			}
		}

		private void AssignIPv4Routes()
		{
			if (this.Configuration.IPv4?.Routes != null && this.Configuration.IPv4.Routes.Count > 0 && this.HasAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv4.Address))
			{
				foreach (var __Route in this.Configuration.IPv4.Routes)
				{
					var __Execution = this._Platform.AssignRoute(this.GetInterface(this._InterfaceName.Value), __Route);

					if (__Execution.Succeeded) continue;

					this._IPv4RoutesAssigned.OnError(new Exception(__Execution.Result));
					return;
				}

				this._IPv4RoutesAssigned.OnNext(true);
			}
		}

		private void ReleaseIPv4Routes()
		{
			if (this.Configuration.IPv4?.Routes != null && this.Configuration.IPv4.Routes.Count > 0)
			{
				foreach (var __Route in this.Configuration.IPv4.Routes)
				{
					var __Execution = this._Platform.ReleaseRoute(this.GetInterface(this._InterfaceName.Value), __Route);

					if (__Execution.Succeeded) continue;

					this._IPv4RoutesAssigned.OnError(new Exception(__Execution.Result));
					return;
				}

				this._IPv4RoutesAssigned.OnNext(false);
			}
		}

		private void AssignIPv6Routes()
		{
			if (this.Configuration.IPv6?.Routes != null && this.Configuration.IPv6.Routes.Count > 0 && this.HasAddress(this.GetInterface(this._InterfaceName.Value), this.Configuration.IPv6.Address))
			{
				foreach (var __Route in this.Configuration.IPv6.Routes)
				{
					var __Execution = this._Platform.AssignRoute(this.GetInterface(this._InterfaceName.Value), __Route);

					if (__Execution.Succeeded) continue;

					this._IPv6RoutesAssigned.OnError(new Exception(__Execution.Result));
					return;
				}

				this._IPv6RoutesAssigned.OnNext(true);
			}
		}

		private void ReleaseIPv6Routes()
		{
			if (this.Configuration.IPv6?.Routes != null && this.Configuration.IPv6.Routes.Count > 0)
			{
				foreach (var __Route in this.Configuration.IPv6.Routes)
				{
					var __Execution = this._Platform.ReleaseRoute(this.GetInterface(this._InterfaceName.Value), __Route);

					if (__Execution.Succeeded) continue;

					this._IPv6RoutesAssigned.OnError(new Exception(__Execution.Result));
					return;
				}

				this._IPv6RoutesAssigned.OnNext(false);
			}
		}

		private NetworkInterface GetInterface(String name)
		{
			return NetworkInterface

				.GetAllNetworkInterfaces()
				.SingleOrDefault(i => String.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		private Boolean HasAddress(NetworkInterface networkInterface, String address)
		{
			var __IpAddresses = networkInterface.GetIPProperties().UnicastAddresses;
			var __HasIpAddress = false;

			foreach (var __IpAddress in __IpAddresses)
			{
				__HasIpAddress = String.Equals(__IpAddress.Address.ToString(), address, StringComparison.OrdinalIgnoreCase);

				if (__HasIpAddress)
				{
					break;
				}
			}

			return __HasIpAddress;
		}

		public void Dispose()
		{
			this._IsDisposed.OnNext(null);
		}

		public override String ToString()
		{
			return this.Configuration.Name;
		}
	}
}