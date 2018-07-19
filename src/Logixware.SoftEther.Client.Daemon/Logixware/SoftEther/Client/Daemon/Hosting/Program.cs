using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Logixware.SoftEther.Client.Daemon.Hosting
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
					configHost.ConfigureHostConfiguration();
					configHost.AddCommandLine(args);
				})

				.ConfigureAppConfiguration((hostContext, configApp) =>
				{
					configApp.AddJsonFile("appsettings.json", optional: true);
					configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
					configApp.AddEnvironmentVariables(prefix: "PREFIX_");
					configApp.ConfigureAppConfiguration(hostContext);
					configApp.AddCommandLine(args);

					__ConfigurationRoot = configApp.Build();
				})

				.ConfigureServices((hostContext, services) =>
				{
					services.ConfigureServices(__ConfigurationRoot);
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