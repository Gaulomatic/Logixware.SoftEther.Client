using System;

namespace Logixware.SoftEther.Client.Daemon.Options
{
	public class VirtualNetwork
	{
		public String Name { get; set; }
		public Boolean AlwaysOn { get; set; }

		public String ConnectionTestHost { get; set; }

		// ReSharper disable once InconsistentNaming
		public IPv4Information IPv4 { get; set; }

		// ReSharper disable once InconsistentNaming
		public IPv6Information IPv6 { get; set; }
	}
}