using System;
using System.Text;
using System.Net.NetworkInformation;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public class PingVpnConnectionVerifier : IVpnConnectionVerifier
	{
		private IClientConfiguration _Configuration;

		public PingVpnConnectionVerifier(IClientConfiguration configuration)
		{
			this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}
		public ConnectionVerificationResult Verify(String host)
		{
			const String __Data = "a quick brown fox jumped over the lazy dog";
			const Int32 __Timeout = 1024;

			var __PingSender = new Ping();
			var __Options = new PingOptions
			{
				DontFragment = true
			};

			var __Buffer = Encoding.ASCII.GetBytes(__Data);
			var __Status = IPStatus.Unknown;

			try
			{
				var __Reply = __PingSender.Send(host, __Timeout, __Buffer, __Options);
				__Status = __Reply?.Status ?? IPStatus.Unknown;
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