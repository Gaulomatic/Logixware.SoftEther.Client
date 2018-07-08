using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Logixware.SoftEther.Client.VpnService;
using Logixware.SoftEther.Client.Daemon.Platform;

using Logixware.SoftEther.Client.Shell;
using Logixware.SoftEther.Client.Shell.Platform;

namespace Logixware.SoftEther.Client.Daemon
{
	public class Program
	{
		// ReSharper disable once AsyncConverter.AsyncMethodNamingHighlighting
		public static async Task Main(String[] args)
		{
			IConfigurationRoot __ConfigurationRoot = null;

			var host = new HostBuilder()

				.ConfigureHostConfiguration(configHost =>
				{
					configHost.SetBasePath(Directory.GetCurrentDirectory());
					configHost.AddJsonFile("hostsettings.json", optional: true);
					configHost.AddEnvironmentVariables(prefix: "PREFIX_");
					configHost.AddCommandLine(args);
				})

				.ConfigureAppConfiguration((hostContext, configApp) =>
				{
					configApp.AddJsonFile("appsettings.json", optional: true);
					configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
					configApp.AddEnvironmentVariables(prefix: "PREFIX_");
					configApp.AddCommandLine(args);

					__ConfigurationRoot = configApp.Build();
				})

				.ConfigureServices((hostContext, services) =>
				{
					services.AddLogging();
					services.AddHostedService<ProgramService>();
					services.AddSingleton(services);
					services.AddSingleton<IClientConfiguration, ClientConfiguration>();
					services.AddSingleton<ICommandLineInterface, CommandLineInterface>();
					services.AddSingleton<IConnectionVerifier, PingConnectionVerifier>();

					services.AddSingleton(__ConfigurationRoot);

					if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					{
						services.AddSingleton<IShell, MacShell>();
						services.AddSingleton<IPlatform, MacPlatform>();
					}
					else
					{
						throw new NotSupportedException("Platform not supported.");
					}
				})

				.ConfigureLogging((hostContext, configLogging) =>
				{
					configLogging.AddConsole();
					configLogging.AddDebug();
				})

				.UseConsoleLifetime()
				.Build();

			await host.RunAsync()

				.ConfigureAwait(false);
		}
	}
}