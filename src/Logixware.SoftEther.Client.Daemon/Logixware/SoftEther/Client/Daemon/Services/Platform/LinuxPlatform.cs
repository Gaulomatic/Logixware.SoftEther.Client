using System;
using System.Net.NetworkInformation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Logixware.SoftEther.Client.Shell;

using Logixware.SoftEther.Client.Daemon.Entities;

namespace Logixware.SoftEther.Client.Daemon.Services
{

	public class LinuxPlatform : IPlatform
	{
		private readonly ILogger<LinuxPlatform> _Logger;
		private readonly IShell _Shell;

		public LinuxPlatform
		(
			ILogger<LinuxPlatform> logger,
			IConfiguration configuration,
			IShell shell
		)
		{
			if (configuration == null) throw new ArgumentNullException(nameof(configuration));

			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._Shell = shell ?? throw new ArgumentNullException(nameof(shell));
		}

		public void Initialize()
		{
		}

		public ExecutionResult AssignIPAddress(NetworkInterface networkInterface, IPv4Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet {info.Address} netmask {info.Mask}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult AssignIPAddress(NetworkInterface networkInterface, IPv6Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet6 {info.Address} prefixlen {info.Prefix}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseIPAddress(NetworkInterface networkInterface, IPv4Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet {info.Address} delete";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseIPAddress(NetworkInterface networkInterface, IPv6Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet6 {info.Address}/{info.Prefix} delete";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult AssignRoute(NetworkInterface networkInterface, IPv4Route info)
		{
			var __Command = $"route add -net {info.Network}/{info.Prefix} gw {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult AssignRoute(NetworkInterface networkInterface, IPv6Route info)
		{
			var __Command = $"route add -net {info.Network}/{info.Prefix} gw {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseRoute(NetworkInterface networkInterface, IPv4Route info)
		{
			var __Command = $"route delete -net {info.Network}/{info.Prefix} gw {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseRoute(NetworkInterface networkInterface, IPv6Route info)
		{
			var __Command = $"route delete -net {info.Network}/{info.Prefix} gw {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}
	}
}