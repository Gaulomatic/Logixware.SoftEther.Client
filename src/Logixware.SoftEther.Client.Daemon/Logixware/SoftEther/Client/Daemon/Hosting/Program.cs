using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Logixware.SoftEther.Client.Daemon.Hosting
{
	public class Program
	{

		public static async Task Main(String[] args)
		{
			// var tet = new BehaviorSubject<Int32?>(null);
			//
			// tet
			//
			// 	// .Catch(Observable.Return<Int32?>(0))
			//
			// 	.OnErrorResumeNext(Observable.Return<Int32?>(0))
			// 	.Subscribe(value => Console.WriteLine(value));
			//
			// for (var Index = 0; Index < 10; Index++)
			// {
			//
			// 	if (Index > 5)
			// 	{
			// 		tet.OnError(new Exception("yill"));
			// 		tet.OnNext(Index);
			// 	}
			// 	else
			// 	{
			// 		tet.OnNext(Index);
			// 	}
			// }

			// // string targetHost = "www.google.de";
			// string targetHost = "2a00:1450:4025:401::5e";
			// string data = "a quick brown fox jumped over the lazy dog";
			//
			// using Ping pingSender = new Ping();
			// PingOptions options = new PingOptions
			// {
			// 	DontFragment = true
			//
			// };
			//
			// byte[] buffer = Encoding.ASCII.GetBytes(data);
			// int timeout = 1024;
			//
			// Console.WriteLine($"Pinging {targetHost}");
			//
			// PingReply reply = await pingSender.SendPingAsync(targetHost, timeout, buffer, options);
			// // PingReply reply = await pingSender.SendPingAsync(targetHost, timeout, buffer);
			// if (reply.Status == IPStatus.Success)
			// {
			// 	Console.WriteLine($"Address: {reply.Address}");
			// 	Console.WriteLine($"RoundTrip time: {reply.RoundtripTime}");
			// 	// Console.WriteLine($"Time to live: {reply.Options.Ttl}");
			// 	// Console.WriteLine($"Don't fragment: {reply.Options.DontFragment}");
			// 	// Console.WriteLine($"Buffer size: {reply.Buffer.Length}");
			// }
			// else
			// {
			// 	Console.WriteLine(reply.Status);
			// }


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

				.ConfigureServices((hostContext, services) => { services.ConfigureServices(__ConfigurationRoot); })

				.ConfigureLogging((hostContext, configLogging) =>
				{
					configLogging.AddConsole();
					configLogging.AddDebug();
				})

				.UseConsoleLifetime();

			return __Host;
		}
	}
}
