using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Logixware.SoftEther.Client.Shell
{
	public abstract class ShellBase : IShell
	{
		public ShellBase
		(
			ILogger logger
		)
		{
			this.Logger = logger;
		}

		protected ILogger Logger { get; }

		protected abstract ProcessStartInfo GetProcessStartInfo(String command);


		public virtual ExecutionResult ExecuteCommand(String command) =>
			this.ExecuteCommand(command, true);

		public virtual ExecutionResult ExecuteCommand(String command, Boolean logCommand)
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

			if (logCommand)
			{
				this.Logger.Debug($"Executing [{command}]");
			}

			void KillProcess(Object state)
			{
				this.Logger.Warn($"Killing command [{command}]");
				__Process.Kill();
			}

			// using (new Timer(delegate { __Process.Kill(); }, null, 5000, Timeout.Infinite))
			// {
			// 	__Process.Start();
			//
			// 	var __Error = __Process.StandardError.ReadToEnd();
			//
			// 	if (!String.IsNullOrEmpty(__Error))
			// 	{
			// 		__Process.WaitForExit(10000);
			// 		return new ExecutionResult(true, __Error);
			// 	}
			//
			// 	__Process.WaitForExit(10000);
			//
			// 	return new ExecutionResult(true, __Process.StandardOutput.ReadToEnd());
			// }

			using (new Timer(KillProcess, null, 5000, Timeout.Infinite))
			{
				__Process.Start();

				__Process.WaitForExit(10000);

				var __Output = __Process.StandardOutput.ReadToEnd();
				var __Error = __Process.StandardError.ReadToEnd();

				if (String.IsNullOrEmpty(__Error))
				{
					return new ExecutionResult(true, __Output);
				}

				this.Logger.Error($"Error executing command [{command}]");

				return new ExecutionResult(false, __Error);
			}
		}
	}
}
