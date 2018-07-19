using System;
using System.Collections.Generic;

using Logixware.SoftEther.Client.Daemon.Entities;

namespace Logixware.SoftEther.Client.Daemon.Configuration
{
	public class ClientConfigurationSection
	{
		public Int32 Interval { get; set; }
		public Int32 ConnectionAttemptsBeforeClientRestart { get; set; }
		public String InternetConnectionTestUrl { get; set; }
		public IList<VirtualNetwork> Networks { get; set; }
	}
}