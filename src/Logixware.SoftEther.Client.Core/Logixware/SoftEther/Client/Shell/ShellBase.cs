using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Logixware.SoftEther.Client.Shell
{
	public abstract class ShellBase : IShell
	{
		public ShellBase
		(
			ILogger logger,
			IOptions<ShellOptions> options
		)
		{
			this.Logger = logger;
			this.Options = options;
		}

		protected ILogger Logger { get; }
		protected IOptions<ShellOptions> Options { get; }

		protected abstract ProcessStartInfo GetProcessStartInfo(String command);


		public virtual ExecutionResult ExecuteCommand(String command) =>
			this.ExecuteCommand(command, true);

		public virtual ExecutionResult ExecuteCommand(String command, Boolean logCommand)
		{
			var __Process = new Process
			{
				StartInfo = this.GetProcessStartInfo(command)
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

			using (new Timer(KillProcess, null, this.Options.Value.Timeout, Timeout.Infinite))
			{
				__Process.Start();
				__Process.WaitForExit(this.Options.Value.Timeout);

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
