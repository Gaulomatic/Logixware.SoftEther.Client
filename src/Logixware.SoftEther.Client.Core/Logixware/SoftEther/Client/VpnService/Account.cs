using System;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.VpnService
{
	public class Account : QueryResult
	{
		public Account(ICommandLineInterface cli, Dictionary<String, String> data)
			: base(cli, data)
		{
		}

		public String Name => base.Get("VPN Connection Setting Name");
		public String DeviceName => base.Get("Device Name Used for Connection");
	}
}