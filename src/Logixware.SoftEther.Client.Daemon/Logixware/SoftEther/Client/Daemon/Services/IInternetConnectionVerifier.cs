using System;
using System.Threading.Tasks;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public interface IInternetConnectionVerifier
	{
		Boolean IsAvailable(String url);
		Task<Boolean> IsAvailableAsync(String url);
	}
}