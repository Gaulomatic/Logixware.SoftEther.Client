using System;

namespace Logixware.SoftEther.Client.Shell
{
	public interface IShell
	{
		ExecutionResult ExecuteCommand(String command);
	}
}