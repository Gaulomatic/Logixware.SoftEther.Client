using System;

namespace Logixware.SoftEther.Client.Daemon
{
	public interface IConnectionVerifier
	{
		ConnectionVerificationResult Verify(String host);
	}
}