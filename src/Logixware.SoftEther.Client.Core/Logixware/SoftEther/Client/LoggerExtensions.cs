using System;

using Microsoft.Extensions.Logging;

namespace Logixware.SoftEther.Client
{
	public static class LoggerExtensions
	{
		public static void Inform(this ILogger target, String message)
		{
			target.Log(LogLevel.Information, 1, message, null, (s, e) => DateTime.Now + " " + s.ToString());
		}

		public static void Warn(this ILogger target, String message)
		{
			target.Log(LogLevel.Warning, 1, message, null, (s, e) => DateTime.Now + " " + s.ToString());
		}

		public static void Error(this ILogger target, String message, Exception exception = null)
		{
			target.Log(LogLevel.Error, 1, message, exception, (s, e) => DateTime.Now + " " + s.ToString());
		}

		public static void Critical(this ILogger target, String message, Exception exception = null)
		{
			target.Log(LogLevel.Critical, 1, message, exception, (s, e) => DateTime.Now + " " + s.ToString());
		}
	}
}