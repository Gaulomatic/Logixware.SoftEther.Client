using System;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Logixware.SoftEther.Client.Daemon.Options
{
	public class GeneralOptionsFactory : IOptionsFactory<GeneralOptions>
	{
		private readonly IConfiguration _Configuration;
		private readonly ILogger<GeneralOptionsFactory> _Logger;

		public GeneralOptionsFactory
		(
			IConfiguration configuration,
			ILogger<GeneralOptionsFactory> logger
		)
		{
			this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public GeneralOptions Create(String name)
		{
			var __Configuration = this._Configuration.GetSection("VPN:General")?.Get<GeneralConfiguration>();

			if (__Configuration == null)
			{
				this._Logger.Warn("No general configuration found, using default values.");
			}

			return new GeneralOptions
			{
				ConnectionAttemptsBeforeClientRestart = __Configuration?.ConnectionAttemptsBeforeClientRestart ?? 5,
				InternetConnectionTestUrl = __Configuration?.InternetConnectionTestUrl ?? "http://clients3.google.com/generate_204"
			};
		}
	}
}
