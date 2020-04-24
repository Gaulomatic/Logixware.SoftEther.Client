using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Logixware.SoftEther.Client.Shell
{
	public class BashShell : ShellBase
	{
		public BashShell
		(
			ILogger<BashShell> logger,
			IOptions<ShellOptions> options
		)
		: base(logger, options)
		{
		}

		protected override ProcessStartInfo GetProcessStartInfo(String command)
		{
			var __EscapedArgs = command.Replace("\"", "\\\"");

			return new ProcessStartInfo
			{
				FileName = base.Options.Value.Path,
				Arguments = $"-c \"{__EscapedArgs}\"",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false
			};
		}
	}
}
