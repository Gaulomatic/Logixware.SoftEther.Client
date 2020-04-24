using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.Extensions.Logging;

using Logixware.SoftEther.Client.VpnService;

namespace Logixware.SoftEther.Client.Daemon.Options
{
	public class NetworkOptions
	{
		public IList<VirtualNetwork> Networks { get; internal set; }
		public ILogger<NetworkOptions> Logger { get; internal set; }

		public IEnumerable<VirtualNetwork> GetValidNetworks(ICommandLineInterface cli)
		{
			if (cli == null) throw new ArgumentNullException(nameof(cli));

			var __ValidNetworks = new List<VirtualNetwork>();
			var __Accounts = cli.GetAccounts().ToList();

			foreach (var __Network in this.Networks)
			{
				var __ValidationResult = this.ValidateVirtualNetworkConfiguration(__Network);

				if (!__ValidationResult)
				{
					this.Logger.Error($"Virtual network configuration \"{__Network.Name}\" invalid. This network will not be monitored.");
				}
				else if (__Accounts.SingleOrDefault(i => String.Equals(__Network.Name, i.Name)) == null)
				{
					this.Logger.Warn($"Account \"{__Network.Name}\" not found in VPN client service.");
					__ValidNetworks.Add(__Network);
				}
				else
				{
					__ValidNetworks.Add(__Network);
				}
			}

			return __ValidNetworks;
		}

		private Boolean ValidateVirtualNetworkConfiguration(VirtualNetwork network)
		{
			if (String.IsNullOrEmpty(network.ConnectionTestHost))
			{
				this.Logger.Warn($"VPN \"{network.Name}\": No connection test host defined.");
				return false;
			}

			if (network.IPv4 == null & network.IPv6 == null)
			{
				this.Logger.Warn($"VPN \"{network.Name}\": Neither IPv4 nor IPv6 address defined.");
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

			if (!IPAddress.TryParse(network.IPv4.Address, out var __IPAddress))
			{
				this.Logger.Warn($"VPN \"{network.Name}\": IPv4 address \"{network.IPv4.Address}\" cound not be parsed.");
				return false;
			}

			if (!IPAddress.TryParse(network.IPv4.Mask, out var __IPMask))
			{
				this.Logger.Warn($"VPN \"{network.Name}\": IPv4 netmask \"{network.IPv4.Mask}\" cound not be parsed.");
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
				this.Logger.Warn($"VPN \"{network.Name}\": IPv6 address \"{network.IPv6.Address}\" cound not be parsed.");
				return false;
			}

			if (network.IPv6.Prefix < 1 || network.IPv6.Prefix > 128)
			{
				this.Logger.Warn($"VPN \"{network.Name}\": IPv6 address prefix invalid: \"{network.IPv6.Prefix}\". Values needs to be between 1 and 128.");
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
					this.Logger.Warn($"VPN \"{network.Name}\": IPv4 route target network address \"{__Route.Network}\" could not be parsed.");
					return false;
				}

				if (__Route.Prefix < 8 || __Route.Prefix > 30)
				{
					this.Logger.Warn($"VPN \"{network.Name}\": IPv4 route target network prefix invalid: \"{__Route.Prefix}\". Values needs to be between 8 and 30.");
					return false;
				}

				if (!IPAddress.TryParse(__Route.Gateway, out var __Gateway))
				{
					this.Logger.Warn($"VPN \"{network.Name}\": IPv4 route gateway address \"{__Route.Gateway}\" could not be parsed.");
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
					this.Logger.Warn($"VPN \"{network.Name}\": IPv6 route target network address \"{__Route.Network}\" could not be parsed.");
					return false;
				}

				if (!IPAddress.TryParse(__Route.Gateway, out var __Gateway))
				{
					this.Logger.Warn($"VPN \"{network.Name}\": IPv6 route gateway address \"{__Route.Gateway}\" could not be parsed.");
					return false;
				}

				if (__Route.Prefix < 1 || __Route.Prefix > 128)
				{
					this.Logger.Warn($"VPN \"{network.Name}\": IPv6 route target network prefix invalid: \"{__Route.Prefix}\". Values needs to be between 1 and 128.");
					return false;
				}
			}

			return true;
		}
	}
}
