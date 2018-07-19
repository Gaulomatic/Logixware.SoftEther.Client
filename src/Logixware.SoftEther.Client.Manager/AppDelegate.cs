using System;
using AppKit;
using Foundation;

namespace Logixware.SoftEther.Client.Manager
{
	[Register("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		public AppDelegate()
		{
		}

		public override void DidFinishLaunching(NSNotification notification)
		{
			// Insert code here to initialize your application
		}

		public override void WillTerminate(NSNotification notification)
		{
			// Insert code here to tear down your application
		}

//		public override Boolean ApplicationShouldHandleReopen(NSApplication sender, Boolean hasVisibleWindows)
//		{
//			if (hasVisibleWindows) return false;
//
//			foreach (var __Window in sender.Windows)
//			{
//				__Window.MakeKeyAndOrderFront(this);
//			}
//
////				for (int n = 0; n < NSApplication.SharedApplication.Windows.Length; ++n)
////				{
////					var content = NSApplication.SharedApplication.Windows[n].ContentViewController as ViewController;
////					if (content != null)
////					{
////						// Bring window to front - MakeKey makes it active as well
////						NSApplication.SharedApplication.Windows[n].MakeKeyAndOrderFront(this);
////						return true;
////					}
////				}
////
////				var mainWindow = new MainWindowController();
////				mainWindow.ShowWindow(this);
//
//			return true;
//		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
		{
			return true;
		}
	}
}
