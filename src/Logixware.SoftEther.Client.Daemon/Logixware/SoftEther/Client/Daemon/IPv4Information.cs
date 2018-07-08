using System;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.Daemon
{
	// ReSharper disable once InconsistentNaming
	public class IPv4Information
	{
		public String Address { get; set; }
		public String Mask { get; set; }

		public IList<IPv4Route> Routes { get; set; }
	}
}