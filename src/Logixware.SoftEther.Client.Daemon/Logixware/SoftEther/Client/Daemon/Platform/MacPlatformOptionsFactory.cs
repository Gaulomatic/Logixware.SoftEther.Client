using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Logixware.SoftEther.Client.Daemon.Platform
{
	public class MacPlatformOptionsFactory : IOptionsFactory<MacPlatformOptions>
	{
		private readonly IConfiguration _Configuration;
		private readonly ILogger<MacPlatformOptionsFactory> _Logger;

		public MacPlatformOptionsFactory
		(
			IConfiguration configuration,
			ILogger<MacPlatformOptionsFactory> logger
		)
		{
			this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public MacPlatformOptions Create(String name)
		{
			var __Configuration = this._Configuration.GetSection("VPN:Networks")?.Get<MacPlatformConfiguration>();

			return new MacPlatformOptions
			{
				TapKextIdentifier = __Configuration?.TapKextIdentifier ?? "net.sf.tuntaposx.tap",
				PathToTapKext = __Configuration?.PathToTapKext ?? "/Library/Extensions/tap.kext"
			};
		}
	}
}
