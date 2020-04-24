using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Logixware.SoftEther.Client.VpnService
{
	public class CommandLineInterfaceOptionsFactory : IOptionsFactory<CommandLineInterfaceOptions>
	{
		private readonly IConfiguration _Configuration;
		private readonly ILogger<CommandLineInterfaceOptionsFactory> _Logger;

		public CommandLineInterfaceOptionsFactory
		(
			IConfiguration configuration,
			ILogger<CommandLineInterfaceOptionsFactory> logger
		)
		{
			this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public CommandLineInterfaceOptions Create(String name)
		{
			var __Configuration = this._Configuration.GetSection("VPN:CommandLineInterface")?.Get<CommandLineInterfaceConfiguration>();

			if (__Configuration == null)
			{
				this._Logger.Error("No command line interface configuration found.");
			}

			if (String.IsNullOrEmpty(__Configuration.PathToClient))
			{
				this._Logger.Error("No path to the VPN client service found.");
			}

			if (String.IsNullOrEmpty(__Configuration.PathToCli))
			{
				this._Logger.Error("No path to the VPN client command line interface found.");
			}

			return new CommandLineInterfaceOptions
			{
				PathToClient = __Configuration.PathToClient,
				PathToCli = __Configuration.PathToCli,
				CliPassword = __Configuration.CliPassword
			};
		}
	}
}
