using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Logixware.SoftEther.Client.Daemon.Options
{
	public class NetworkOptionsFactory : IOptionsFactory<NetworkOptions>
	{
		private readonly IConfiguration _Configuration;
		private readonly ILogger<NetworkOptionsFactory> _Logger;
		private readonly ILogger<NetworkOptions> _NetworkLogger;

		public NetworkOptionsFactory
		(
			IConfiguration configuration,
			ILogger<NetworkOptions> networkLogger,
			ILogger<NetworkOptionsFactory> logger
		)
		{
			this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this._NetworkLogger = networkLogger ?? throw new ArgumentNullException(nameof(networkLogger));
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public NetworkOptions Create(String name)
		{
			var __Configuration = this._Configuration.GetSection("VPN:Networks")?.Get<IList<VirtualNetwork>>();

			if (__Configuration == null || __Configuration.Count == 0)
			{
				this._Logger.Error("No virtual networks defined.");
			}

			return new NetworkOptions
			{
				Networks = __Configuration,
				Logger = this._NetworkLogger
			};
		}
	}
}
