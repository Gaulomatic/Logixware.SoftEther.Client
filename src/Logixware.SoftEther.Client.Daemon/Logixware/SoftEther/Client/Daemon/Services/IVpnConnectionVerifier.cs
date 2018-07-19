using System;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public interface IVpnConnectionVerifier
	{
		ConnectionVerificationResult Verify(String host);
	}
}