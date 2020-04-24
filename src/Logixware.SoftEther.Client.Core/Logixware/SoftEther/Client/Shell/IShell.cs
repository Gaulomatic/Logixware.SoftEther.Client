using System;

namespace Logixware.SoftEther.Client.Shell
{
	public interface IShell
	{
		ExecutionResult ExecuteCommand(String command);
		ExecutionResult ExecuteCommand(String command, Boolean logCommand);
	}
}
