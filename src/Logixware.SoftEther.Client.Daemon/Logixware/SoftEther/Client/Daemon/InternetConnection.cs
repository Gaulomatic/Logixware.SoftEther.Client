using System;
using System.Net;
using System.Threading.Tasks;

namespace Logixware.SoftEther.Client.Daemon
{
	public class InternetConnection
	{
		public static Boolean IsAvailible(String url)
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

		public static async Task<Boolean> IsAvailibleAsync(String url)
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