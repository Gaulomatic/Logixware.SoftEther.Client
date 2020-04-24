using System;
using System.Text;
using System.Net.NetworkInformation;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public class NetPingVpnConnectionVerifier : IVpnConnectionVerifier
	{
		public ConnectionVerificationResult Verify(String host)
		{
			const String __Data = "a quick brown fox jumped over the lazy dog";
			const Int32 __Timeout = 1024;

			using var __Ping = new Ping();
			var __Options = new PingOptions
			{
				DontFragment = true
			};

			var __Buffer = Encoding.ASCII.GetBytes(__Data);
			var __Status = IPStatus.Unknown;

			try
			{
				var __Reply = __Ping.Send(host, __Timeout, __Buffer, __Options);
				__Status = __Reply?.Status ?? IPStatus.Unknown;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
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
