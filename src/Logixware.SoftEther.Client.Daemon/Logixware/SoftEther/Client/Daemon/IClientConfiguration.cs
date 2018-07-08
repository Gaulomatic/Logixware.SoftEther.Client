using System.Collections.Generic;

using Logixware.SoftEther.Client.VpnService;

namespace Logixware.SoftEther.Client.Daemon
{
	public interface IClientConfiguration
	{
		ClientConfigurationSection Settings { get; }
		IEnumerable<VirtualNetwork> GetValidNetworks(ICommandLineInterface cli);
	}
}