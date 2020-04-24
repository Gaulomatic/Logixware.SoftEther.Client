using System;
using System.Net.NetworkInformation;

using Logixware.SoftEther.Client.Daemon.Platform;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public class ConsolePingVpnConnectionVerifier : IVpnConnectionVerifier
	{
		private readonly IPlatform _Platform;

		public ConsolePingVpnConnectionVerifier(IPlatform shell)
		{
			this._Platform = shell ?? throw new ArgumentNullException(nameof(shell));
		}

		public ConnectionVerificationResult Verify(String host)
		{
			var __Status = IPStatus.Unknown;

			try
			{
				var __Reply = this._Platform.Ping(host);
				__Status = __Reply.Succeeded ? IPStatus.Success : IPStatus.TimedOut;
			}
			catch
			{
				__Status = IPStatus.Unknown;
			}

			return new ConnectionVerificationResult
			(
				__Status == IPStatus.Success ? ReachableState.Reachable : ReachableState.Unreachable,
				__Status
			);
		}
	}
}
