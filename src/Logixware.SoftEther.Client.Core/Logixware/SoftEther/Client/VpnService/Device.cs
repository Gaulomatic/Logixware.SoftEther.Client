using System;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.VpnService
{
	public class Device : QueryResult
	{
		public Device(ICommandLineInterface cli, Dictionary<String, String> data)
			: base(cli, data)
		{
		}

		public String Name => base.Get("Virtual Network Adapter Name");
		public String PhysicalAddress => base.Get("MAC Address");
	}
}