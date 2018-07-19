using System.Collections.Generic;

using Logixware.SoftEther.Client.VpnService;

using Logixware.SoftEther.Client.Daemon.Entities;
using Logixware.SoftEther.Client.Daemon.Configuration;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public interface IClientConfiguration
	{
		ClientConfigurationSection Settings { get; }
		IEnumerable<VirtualNetwork> GetValidNetworks(ICommandLineInterface cli);
	}
}