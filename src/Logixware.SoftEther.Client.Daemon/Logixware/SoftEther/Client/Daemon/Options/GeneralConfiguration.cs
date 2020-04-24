using System;

namespace Logixware.SoftEther.Client.Daemon.Options
{
	public class GeneralConfiguration
	{
		public Int32? ConnectionAttemptsBeforeClientRestart { get; set; }
		public String InternetConnectionTestUrl { get; set; }
	}
}
