using System;
using System.Threading;
using System.Net.NetworkInformation;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Logixware.SoftEther.Client.Shell;

using Logixware.SoftEther.Client.Daemon.Options;

namespace Logixware.SoftEther.Client.Daemon.Platform
{
	public class MacPlatform : IPlatform
	{
		private readonly ILogger<MacPlatform> _Logger;
		private readonly IOptions<MacPlatformOptions> _Options;
		private readonly IShell _Shell;

		public MacPlatform
		(
			ILogger<MacPlatform> logger,
			IOptions<MacPlatformOptions> options,
			IShell shell
		)
		{
			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._Options = options ?? throw new ArgumentNullException(nameof(options));
			this._Shell = shell ?? throw new ArgumentNullException(nameof(shell));
		}

		public void Initialize()
		{
			if (!this.IsDriverLoaded())
			{
				this._Logger.Inform($"Kernel extension \"{this._Options.Value.TapKextIdentifier}\" not loaded.");
				this._Logger.Inform($"Loading Kernel extension \"{this._Options.Value.PathToTapKext}\".");

				var __Command = $"kextload \"{this._Options.Value.PathToTapKext}\"";
				var __Response = this._Shell.ExecuteCommand(__Command);

				if (!__Response.Succeeded)
				{
					throw new Exception(__Response.Result);
				}

				while (!this.IsDriverLoaded())
				{
					Thread.Sleep(1000);
					this._Logger.Inform($"Kernel extension \"{this._Options.Value.TapKextIdentifier}\" still not loaded.");
				}
			}
		}

		private Boolean IsDriverLoaded()
		{
			var __Command = $"kextstat | grep \"{this._Options.Value.TapKextIdentifier}\"";
			var __Response = this._Shell.ExecuteCommand(__Command);

			if (!__Response.Succeeded)
			{
				throw new Exception(__Response.Result);
			}

			return !String.IsNullOrEmpty(__Response.Result) && __Response.Result.Contains(this._Options.Value.TapKextIdentifier) && !__Response.Result.Contains("failed to load");
		}

		public ExecutionResult Ping(String host)
		{
			var __Command = $"ping -c 1 -t 4 {host}";
			return this._Shell.ExecuteCommand(__Command, false);
		}

		public ExecutionResult AssignIPAddress(NetworkInterface networkInterface, IPv4Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet {info.Address} netmask {info.Mask}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult AssignIPAddress(NetworkInterface networkInterface, IPv6Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet6 {info.Address} prefixlen {info.Prefix}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseIPAddress(NetworkInterface networkInterface, IPv4Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet {info.Address} delete";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseIPAddress(NetworkInterface networkInterface, IPv6Information info)
		{
			var __Command = $"ifconfig {networkInterface.Name} inet6 {info.Address}/{info.Prefix} delete";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult AssignRoute(NetworkInterface networkInterface, IPv4Route info)
		{
			var __Command = $"route -n add -net {info.Network}/{info.Prefix} {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult AssignRoute(NetworkInterface networkInterface, IPv6Route info)
		{
			var __Command = $"route -n add -net {info.Network}/{info.Prefix} {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseRoute(NetworkInterface networkInterface, IPv4Route info)
		{
			var __Command = $"route -n delete -net {info.Network}/{info.Prefix} {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}

		public ExecutionResult ReleaseRoute(NetworkInterface networkInterface, IPv6Route info)
		{
			var __Command = $"route -n delete -net {info.Network}/{info.Prefix} {info.Gateway}";
			return this._Shell.ExecuteCommand(__Command);
		}

//		[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
//		public static extern Int32 IORegisterForSystemPower(IntPtr refcon, ref Int32 thePortRef, MyCallback func, ref Int32 notifier);
//
////		CFRunLoopRef CFRunLoopGetCurrent(void);
//		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
//		extern static Int32 CFRunLoopGetCurrent();
//
////		void CFRunLoopAddSource(CFRunLoopRef rl, CFRunLoopSourceRef source, CFRunLoopMode mode);
//		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
//		extern static void CFRunLoopAddSource(ref Int32 rl, ref Int32 source, IntPtr e);
//
////		CFRunLoopSourceRef IONotificationPortGetRunLoopSource(IONotificationPortRef notify);
//		[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
//		public static extern Int32 IONotificationPortGetRunLoopSource(ref Int32 notify);
//
//		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
//		extern static IntPtr CFStringCreateWithCharacters(IntPtr allocator, string str, Int32 count);
//
////		void CFRunLoopRun(void);
//		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
//		extern static void CFRunLoopRun();
//
//		// void MySleepCallBack( void * refCon, io_service_t service, natural_t messageType, void * messageArgument )
//		public delegate void MyCallback(IntPtr refCon, Int32 service, Int32 messageType, IntPtr messageArgument);
//
//		public static void MySleepCallBack(IntPtr refCon, Int32 service, Int32 messageType, IntPtr messageArgument)
//		{
//			Console.WriteLine("REC: " + messageType);
//		}
//
//		public MacPlatform()
//		{
//			IntPtr e = new IntPtr();
//			IntPtr e2 = new IntPtr();
//			var __NotifyPortRef = 0;
//			var notifier = 0;
//
//			var yillmazz = IORegisterForSystemPower(e, ref __NotifyPortRef, MySleepCallBack, ref notifier);
//
//			var __RunLoopRef = CFRunLoopGetCurrent();
//			var __RunLoopSourceRef = IONotificationPortGetRunLoopSource(ref __NotifyPortRef);
//
//			IntPtr handle = CFStringCreateWithCharacters(IntPtr.Zero, "kCFRunLoopCommonModes", "kCFRunLoopCommonModes".Length);
//
//			CFRunLoopAddSource(ref __RunLoopRef, ref __RunLoopSourceRef, handle);
//
////			CFRunLoopRun();
//
//			Console.WriteLine("kk");
//		}
	}
}
