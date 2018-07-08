using System;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.VpnService
{
	public class AccountStatus : QueryResult
	{
		public AccountStatus(ICommandLineInterface cli, Dictionary<String, String> data)
			: base(cli, data)
		{
		}

		public ConnectionState ConnectionState
		{
			get
			{
				var __State = base.Get("Session Status");

				if (String.Equals(__State, "Connection Completed (Session Established)"))
				{
					return ConnectionState.Connected;
				}

				return String.Equals(__State, "Retrying") ? ConnectionState.Retrying : ConnectionState.Disconnected;
			}
		}
	}
}