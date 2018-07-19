using System;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Logixware.SoftEther.Client.Daemon.Services;

namespace Logixware.SoftEther.Client.Daemon.Hosting
{
	public class ProgramService : IHostedService
	{
		private readonly ILogger<ProgramService> _Logger;
		private readonly IApplicationLifetime _AppLifetime;
		private readonly IClientService _ClientService;

		public ProgramService
		(
			ILogger<ProgramService> logger,
			IApplicationLifetime appLifetime,
			IClientService nasService
		)
		{
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._AppLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
			this._ClientService = nasService ?? throw new ArgumentNullException(nameof(nasService));
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			this._AppLifetime.ApplicationStarted.Register(this.OnStarted);
			this._AppLifetime.ApplicationStopping.Register(this.OnStopping);
			this._AppLifetime.ApplicationStopped.Register(this.OnStopped);

			await this._ClientService.StartAsync(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			this._Logger.Inform("Shutting down application...");
			await this._ClientService.StopAsync(cancellationToken);
		}

		private void OnStarted()
		{
			this._Logger.Inform("Starting application...");
		}

		private void OnStopping()
		{
			this._Logger.Inform("Shutting down application...");
		}

		private void OnStopped()
		{
			this._Logger.Inform("Application shut down. C ya.");
		}
	}
}
