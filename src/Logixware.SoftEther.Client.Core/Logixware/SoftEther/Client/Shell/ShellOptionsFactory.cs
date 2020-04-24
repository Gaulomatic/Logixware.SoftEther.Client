using System;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Logixware.SoftEther.Client.Shell
{
	public class ShellOptionsFactory : IOptionsFactory<ShellOptions>
	{
		private readonly IConfiguration _Configuration;
		private readonly ILogger<ShellOptionsFactory> _Logger;

		public ShellOptionsFactory
		(
			IConfiguration configuration,
			ILogger<ShellOptionsFactory> logger
		)
		{
			this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public ShellOptions Create(String name)
		{
			var __Configuration = this._Configuration.GetSection("VPN:Shell")?.Get<ShellConfiguration>();

			if (__Configuration == null)
			{
				this._Logger.Error("No shell configuration defined.");
			}

			if (String.IsNullOrEmpty(__Configuration?.Path))
			{
				this._Logger.Error("No shell path defined.");
			}

			return new ShellOptions
			{
				Path = __Configuration.Path,
				Timeout = __Configuration.Timeout ?? 5000
			};
		}
	}
}
