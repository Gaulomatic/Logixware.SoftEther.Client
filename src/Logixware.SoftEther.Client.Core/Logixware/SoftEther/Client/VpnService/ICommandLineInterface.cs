using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.VpnService
{
	public interface ICommandLineInterface
	{
		IEnumerable<Account> GetAccounts();
		Account GetAccount(String name);

		AccountStatus GetAccountStatus(String name);

		IEnumerable<Device> GetDevices();
		Device GetDevice(String name);

		void RestartClient();
		void StartClient();
		void StopClient();
	}
}