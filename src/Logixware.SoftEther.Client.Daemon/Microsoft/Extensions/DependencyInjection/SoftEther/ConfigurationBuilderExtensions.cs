using System;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection.Mail
{
	public static class ConfigurationBuilderExtensions
	{
		internal static void AddHostConfiguration(this IConfigurationBuilder configHost)
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

		internal static void AddSoftEtherConfiguration(this IConfigurationBuilder configApp, HostBuilderContext hostContext)
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
	}
}
