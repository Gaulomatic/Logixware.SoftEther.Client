using System;

namespace Logixware.SoftEther.Client.Shell
{
	public class ExecutionResult
	{
		public ExecutionResult(Boolean succeeded, String result)
		{
			this.Succeeded = succeeded;
			this.Result = result;
		}

		public Boolean Succeeded { get; }
		public String Result { get; }
	}
}