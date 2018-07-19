using System;
using System.Net;
using System.Threading.Tasks;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public class InternetConnectionVerifier : IInternetConnectionVerifier
	{
		public Boolean IsAvailable(String url)
		{
			try
			{
				using (var __Client = new WebClient())
				using (__Client.OpenRead(url))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		public async Task<Boolean> IsAvailableAsync(String url)
		{
			try
			{
				using (var __Client = new WebClient())
				using (await __Client.OpenReadTaskAsync(url).ConfigureAwait(false))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}
	}
}