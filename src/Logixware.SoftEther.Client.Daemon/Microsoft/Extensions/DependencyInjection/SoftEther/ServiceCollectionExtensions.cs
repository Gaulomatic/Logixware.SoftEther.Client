﻿using System;
 using System.Runtime.InteropServices;

 using Microsoft.Extensions.Options;
 using Microsoft.Extensions.Configuration;

 using Logixware.SoftEther.Client.Shell;
 using Logixware.SoftEther.Client.VpnService;

 using Logixware.SoftEther.Client.Daemon.Hosting;
 using Logixware.SoftEther.Client.Daemon.Options;
 using Logixware.SoftEther.Client.Daemon.Platform;
 using Logixware.SoftEther.Client.Daemon.Services;

 namespace Microsoft.Extensions.DependencyInjection.Mail
{
	public static class ServiceCollectionExtensions
	{
		internal static IServiceCollection AddSoftEther(this IServiceCollection services, IConfigurationRoot configurationRoot, Action<ISoftEtherBuilder> setupAction = null)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				services.AddTransient<IOptionsFactory<MacPlatformOptions>, MacPlatformOptionsFactory>();
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

			services.AddOptions();

			services.AddTransient<IOptionsFactory<CommandLineInterfaceOptions>, CommandLineInterfaceOptionsFactory>();
			services.AddTransient<IOptionsFactory<GeneralOptions>, GeneralOptionsFactory>();
			services.AddTransient<IOptionsFactory<NetworkOptions>, NetworkOptionsFactory>();
			services.AddTransient<IOptionsFactory<ShellOptions>, ShellOptionsFactory>();

			var __ShellConfiguration = configurationRoot.GetSection("VPN:Shell")?.Get<ShellConfiguration>();

			if (__ShellConfiguration == null)
			{
				throw new InvalidOperationException("No shell configuration defined.");
			}

			if (String.Equals(__ShellConfiguration.Type, "BashShell", StringComparison.OrdinalIgnoreCase))
			{
				services.AddSingleton<IShell, BashShell>();
			}
			else
			{
				throw new NotSupportedException($"Shell '{__ShellConfiguration.Type}' not supported.");
			}

			services.AddSingleton(services);
			services.AddSingleton(configurationRoot);

			services.AddHostedService<ProgramService>();

			services.AddSingleton<IClientService, ClientService>();
			services.AddSingleton<IInternetConnectionVerifier, InternetConnectionVerifier>();
			services.AddSingleton<ICommandLineInterface, CommandLineInterface>();

			// services.AddSingleton<IVpnConnectionVerifier, NetPingVpnConnectionVerifier>();
			services.AddSingleton<IVpnConnectionVerifier, ConsolePingVpnConnectionVerifier>();

			if (setupAction != null)
			{
				var __Builder = new SoftEtherBuilder(services);
				setupAction.Invoke(__Builder);
			}

			return services;
		}
	}
}
