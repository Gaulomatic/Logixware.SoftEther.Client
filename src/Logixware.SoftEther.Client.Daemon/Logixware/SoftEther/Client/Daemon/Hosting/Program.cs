using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Mail;

using Logixware.Diagnostics;

namespace Logixware.SoftEther.Client.Daemon.Hosting
{
	public class Program
	{

		public static async Task Main(String[] args)
		{
			ApplicationInfo.Current()

				.SetConsoleTitle()
				.WriteToConsole()
				.WritePidToConsole();

			await Program.CreateHostBuilder(args)

				.Build()
				.RunAsync()
				.ConfigureAwait(false);
		}

		public static IHostBuilder CreateHostBuilder(String[] args)
		{
			IConfigurationRoot __ConfigurationRoot = null;

			var __Host = Host.CreateDefaultBuilder(args)

				.ConfigureHostConfiguration(configHost =>
				{
					configHost.SetBasePath(Directory.GetCurrentDirectory());
					configHost.AddJsonFile("hostsettings.json", optional: true);
					configHost.AddEnvironmentVariables(prefix: "PREFIX_");
					configHost.AddHostConfiguration();
					configHost.AddCommandLine(args);
				})

				.ConfigureAppConfiguration((hostContext, configApp) =>
				{
					configApp.AddJsonFile("appsettings.json", optional: true);
					configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
					configApp.AddEnvironmentVariables(prefix: "PREFIX_");
					configApp.AddSoftEtherConfiguration(hostContext);
					configApp.AddCommandLine(args);

					__ConfigurationRoot = configApp.Build();
				})

				.ConfigureLogging((hostContext, configLogging) =>
				{
					configLogging.AddConsole();
					configLogging.AddDebug();
				})

				.UseConsoleLifetime()

				.ConfigureServices((hostContext, services) =>
				{
					services.AddSoftEther(__ConfigurationRoot);
				});

			return __Host;
		}
	}
}
