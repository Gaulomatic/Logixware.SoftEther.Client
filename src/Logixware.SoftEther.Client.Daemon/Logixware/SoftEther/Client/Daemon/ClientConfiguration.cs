using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Logixware.SoftEther.Client.VpnService;

namespace Logixware.SoftEther.Client.Daemon
{
	public class ClientConfiguration : IClientConfiguration
	{
		private readonly ILogger<IClientConfiguration> _Logger;
		private readonly ClientConfigurationSection _ProvidedConfiguration;

		public ClientConfiguration
		(
			ILogger<IClientConfiguration> logger,
			IConfigurationRoot configurationRoot
		)
		{
			if (configurationRoot == null) throw new ArgumentNullException(nameof(configurationRoot));
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));

			var __Section = configurationRoot.GetSection("VPN");

			if (__Section == null)
			{
				throw new InvalidOperationException("No VPN configuration section.");
			}

			this._ProvidedConfiguration = __Section.Get<ClientConfigurationSection>();

			if (this._ProvidedConfiguration == null)
			{
				throw new InvalidOperationException("No VPN configuration found.");
			}

			if (this._ProvidedConfiguration.Networks == null || this._ProvidedConfiguration.Networks.ToList().Count == 0)
			{
				throw new InvalidOperationException("No virtual networks defined.");
			}

			this.Settings = new ClientConfigurationSection
			{
				Networks = new List<VirtualNetwork>()
			};

			if (String.IsNullOrEmpty(this._ProvidedConfiguration.InternetConnectionTestUrl))
			{
				this.Settings.InternetConnectionTestUrl = "http://clients3.google.com/generate_204";
				this._Logger.Inform($"\"InternetConnectionTestUrl\" not defined. Using \"{this.Settings.InternetConnectionTestUrl}\".");
			}
			else
			{
				this.Settings.InternetConnectionTestUrl = this._ProvidedConfiguration.InternetConnectionTestUrl;
			}

			if (this._ProvidedConfiguration.ConnectionAttemptsBeforeClientRestart == 0)
			{
				this.Settings.ConnectionAttemptsBeforeClientRestart = 10;
				this._Logger.Inform($"\"ConnectionAttemptsBeforeClientRestart\" not defined. Using \"{this.Settings.ConnectionAttemptsBeforeClientRestart}\".");
			}
			else
			{
				this.Settings.ConnectionAttemptsBeforeClientRestart = this._ProvidedConfiguration.ConnectionAttemptsBeforeClientRestart;
			}

			if (this._ProvidedConfiguration.Interval == 0)
			{
				this.Settings.Interval = 5000;
				this._Logger.Inform($"\"Interval\" not defined. Using \"{this.Settings.Interval / 1000}\" (seconds).");
			}
			else
			{
				this.Settings.Interval = this._ProvidedConfiguration.Interval;
			}
		}

		public ClientConfigurationSection Settings { get; }

		public IEnumerable<VirtualNetwork> GetValidNetworks(ICommandLineInterface cli)
		{
			var __Accounts = cli.GetAccounts().ToList();

			foreach (var __Network in this._ProvidedConfiguration.Networks)
			{
				var __ValidationResult = this.ValidateVirtualNetworkConfiguration(__Network);

				if (!__ValidationResult)
				{
					this._Logger.Error($"Virtual network configuration \"{__Network.Name}\" invalid. This network will not be monitored.");
				}
				else if (__Accounts.SingleOrDefault(i => String.Equals(__Network.Name, i.Name)) == null)
				{
					this._Logger.Warn($"Account \"{__Network.Name}\" not found in VPN client service.");
					this.Settings.Networks.Add(__Network);
				}
				else
				{
					this.Settings.Networks.Add(__Network);
				}
			}

			return this.Settings.Networks;
		}

		private Boolean ValidateVirtualNetworkConfiguration(VirtualNetwork network)
		{
			if (String.IsNullOrEmpty(network.ConnectionTestHost))
			{
				this._Logger.Warn($"VPN \"{network.Name}\": No connection test host defined.");
				return false;
			}

			if (network.IPv4 == null & network.IPv6 == null)
			{
				this._Logger.Warn($"VPN \"{network.Name}\": Neither IPv4 nor IPv6 address defined.");
				return false;
			}

			return this.ValidateIPv4(network) && this.ValidateIPv6(network);
		}

		private Boolean ValidateIPv4(VirtualNetwork network)
		{
			if (network.IPv4 == null)
			{
				return true;
			}

			;

			if (!IPAddress.TryParse(network.IPv4.Address, out var __IPAddress))
			{
				this._Logger.Warn($"VPN \"{network.Name}\": IPv4 address \"{network.IPv4.Address}\" cound not be parsed.");
				return false;
			}

			if (!IPAddress.TryParse(network.IPv4.Mask, out var __IPMask))
			{
				this._Logger.Warn($"VPN \"{network.Name}\": IPv4 netmask \"{network.IPv4.Mask}\" cound not be parsed.");
				return false;
			}

			return this.ValidateIPv4Routes(network);
		}

		private Boolean ValidateIPv6(VirtualNetwork network)
		{
			if (network.IPv6 == null)
			{
				return true;
			}

			if (!IPAddress.TryParse(network.IPv6.Address, out var __IPAddress))
			{
				this._Logger.Warn($"VPN \"{network.Name}\": IPv6 address \"{network.IPv6.Address}\" cound not be parsed.");
				return false;
			}

			if (network.IPv6.Prefix < 1 || network.IPv6.Prefix > 128)
			{
				this._Logger.Warn($"VPN \"{network.Name}\": IPv6 address prefix invalid: \"{network.IPv6.Prefix}\". Values needs to be between 1 and 128.");
				return false;
			}

			return this.ValidateIPv6Routes(network);
		}

		private Boolean ValidateIPv4Routes(VirtualNetwork network)
		{
			if (network.IPv4.Routes == null)
			{
				return true;
			}

			foreach (var __Route in network.IPv4.Routes)
			{
				if (!IPAddress.TryParse(__Route.Network, out var __IPAddress))
				{
					this._Logger.Warn($"VPN \"{network.Name}\": IPv4 route target network address \"{__Route.Network}\" could not be parsed.");
					return false;
				}

				if (__Route.Prefix < 8 || __Route.Prefix > 30)
				{
					this._Logger.Warn($"VPN \"{network.Name}\": IPv4 route target network prefix invalid: \"{__Route.Prefix}\". Values needs to be between 8 and 30.");
					return false;
				}

				if (!IPAddress.TryParse(__Route.Gateway, out var __Gateway))
				{
					this._Logger.Warn($"VPN \"{network.Name}\": IPv4 route gateway address \"{__Route.Gateway}\" could not be parsed.");
					return false;
				}
			}

			return true;
		}

		private Boolean ValidateIPv6Routes(VirtualNetwork network)
		{
			if (network.IPv4.Routes == null)
			{
				return true;
			}

			foreach (var __Route in network.IPv6.Routes)
			{
				if (!IPAddress.TryParse(__Route.Network, out var __IPAddress))
				{
					this._Logger.Warn($"VPN \"{network.Name}\": IPv6 route target network address \"{__Route.Network}\" could not be parsed.");
					return false;
				}

				if (!IPAddress.TryParse(__Route.Gateway, out var __Gateway))
				{
					this._Logger.Warn($"VPN \"{network.Name}\": IPv6 route gateway address \"{__Route.Gateway}\" could not be parsed.");
					return false;
				}

				if (__Route.Prefix < 1 || __Route.Prefix > 128)
				{
					this._Logger.Warn($"VPN \"{network.Name}\": IPv6 route target network prefix invalid: \"{__Route.Prefix}\". Values needs to be between 1 and 128.");
					return false;
				}
			}

			return true;
		}
	}
}