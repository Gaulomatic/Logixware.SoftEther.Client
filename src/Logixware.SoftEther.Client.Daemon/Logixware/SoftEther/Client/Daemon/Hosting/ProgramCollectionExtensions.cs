using System;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Logixware.SoftEther.Client.Shell;
using Logixware.SoftEther.Client.VpnService;

using Logixware.SoftEther.Client.Daemon.Services;

namespace Logixware.SoftEther.Client.Daemon.Hosting
{
	internal static class ProgramCollectionExtensions
	{
		internal static void ConfigureHostConfiguration(this IConfigurationBuilder configHost)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				configHost.AddJsonFile("/Library/Preferences/Logixware/Logixware.SoftEther.Client.Daemon/hostsettings.json", optional: true);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				configHost.AddJsonFile("/etc/logixware/Logixware.SoftEther.Client.Daemon/hostsettings.json", optional: true);
			}
			else
			{
				throw new NotSupportedException("Platform not supported.");
			}
		}

		internal static void ConfigureAppConfiguration(this IConfigurationBuilder configApp, HostBuilderContext hostContext)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				configApp.AddJsonFile("/Library/Preferences/Logixware/Logixware.SoftEther.Client.Daemon/appsettings.json", optional: true);
				configApp.AddJsonFile($"/Library/Preferences/Logixware/Logixware.SoftEther.Client.Daemon/appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				configApp.AddJsonFile("/etc/logixware/Logixware.SoftEther.Client.Daemon/appsettings.json", optional: true);
				configApp.AddJsonFile($"/etc/logixware/Logixware.SoftEther.Client.Daemon/appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
			}
			else
			{
				throw new NotSupportedException("Platform not supported.");
			}
		}

		internal static void ConfigureServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
		{
			services.AddSingleton(services);
			services.AddSingleton(configurationRoot);

			services.AddHostedService<ProgramService>();

			services.AddSingleton<IClientService, ClientService>();
			services.AddSingleton<IInternetConnectionVerifier, InternetConnectionVerifier>();
			services.AddSingleton<IClientConfiguration, ClientConfiguration>();
			services.AddSingleton<ICommandLineInterface, CommandLineInterface>();
			services.AddSingleton<IShell, BashShell>();

			// services.AddSingleton<IVpnConnectionVerifier, NetPingVpnConnectionVerifier>();
			services.AddSingleton<IVpnConnectionVerifier, ConsolePingVpnConnectionVerifier>();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				services.AddSingleton<IPlatform, MacPlatform>();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				services.AddSingleton<IPlatform, LinuxPlatform>();
			}
			else
			{
				throw new NotSupportedException("Platform not supported.");
			}
		}
	}
}
