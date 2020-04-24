using System;
using System.Net.NetworkInformation;

using Logixware.SoftEther.Client.Shell;
using Logixware.SoftEther.Client.Daemon.Entities;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public interface IPlatform
	{
		void Initialize();

		ExecutionResult Ping(String host);

		ExecutionResult AssignIPAddress(NetworkInterface networkInterface, IPv4Information info);
		ExecutionResult AssignIPAddress(NetworkInterface networkInterface, IPv6Information info);

		ExecutionResult ReleaseIPAddress(NetworkInterface networkInterface, IPv4Information info);
		ExecutionResult ReleaseIPAddress(NetworkInterface networkInterface, IPv6Information info);

		ExecutionResult AssignRoute(NetworkInterface networkInterface, IPv4Route info);
		ExecutionResult AssignRoute(NetworkInterface networkInterface, IPv6Route info);
		ExecutionResult ReleaseRoute(NetworkInterface networkInterface, IPv4Route info);
		ExecutionResult ReleaseRoute(NetworkInterface networkInterface, IPv6Route info);
	}
}
