using System;
using System.Threading;
using System.Diagnostics;

namespace Logixware.SoftEther.Client.Shell.Platform
{

	public class MacShell : IShell
	{
		public ExecutionResult ExecuteCommand(String command)
		{
			var __EscapedArgs = command.Replace("\"", "\\\"");

			var __Process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "/bin/bash",
					Arguments = $"-c \"{__EscapedArgs}\"",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = false
				}
			};

			using (new Timer(delegate { __Process.Kill(); }, null, 5000, Timeout.Infinite))
			{
				__Process.Start();

				var __Error = __Process.StandardError.ReadToEnd();

				if (!String.IsNullOrEmpty(__Error))
				{
					__Process.WaitForExit(10000);
					return new ExecutionResult(true, __Error);
				}

				__Process.WaitForExit(10000);

				return new ExecutionResult(true, __Process.StandardOutput.ReadToEnd());
			}
		}
	}
}