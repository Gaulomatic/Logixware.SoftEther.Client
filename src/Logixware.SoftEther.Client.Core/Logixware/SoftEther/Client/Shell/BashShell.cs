using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace Logixware.SoftEther.Client.Shell
{
	public class BashShell : ShellBase
	{
		public BashShell
		(
			ILogger<BashShell> logger
		)
		: base(logger)
		{
		}

		protected override ProcessStartInfo GetProcessStartInfo(String command)
		{
			var __EscapedArgs = command.Replace("\"", "\\\"");

			return new ProcessStartInfo
			{
				FileName = "/bin/bash",
				Arguments = $"-c \"{__EscapedArgs}\"",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false
			};
		}
	}
}
