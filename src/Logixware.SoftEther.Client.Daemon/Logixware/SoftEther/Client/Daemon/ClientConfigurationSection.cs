using System;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.Daemon
{
	public class ClientConfigurationSection
	{
		public Int32 Interval { get; set; }
		public Int32 ConnectionAttemptsBeforeClientRestart { get; set; }
		public String InternetConnectionTestUrl { get; set; }
		public IList<VirtualNetwork> Networks { get; set; }
	}
}