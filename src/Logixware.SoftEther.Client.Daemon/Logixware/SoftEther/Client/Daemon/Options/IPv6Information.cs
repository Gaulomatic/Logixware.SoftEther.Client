using System;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.Daemon.Options
{
	// ReSharper disable once InconsistentNaming
	public class IPv6Information
	{
		public String Address { get; set; }
		public Int32 Prefix { get; set; }

		public IList<IPv6Route> Routes { get; set; }
	}
}