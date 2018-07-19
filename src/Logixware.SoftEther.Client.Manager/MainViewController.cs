// This file has been autogenerated from a class added in the UI designer.

using System;
using AppKit;

namespace Logixware.SoftEther.Client.Manager
{
	public partial class MainViewController : NSViewController
	{
		private NSViewController _CurrentController;

		public MainViewController (IntPtr handle) : base (handle)
		{
		}

		public override void LoadView()
		{
			base.LoadView();
            this.DisplayStoryboard("Interfaces");
		}

		public void DisplayView(Views view)
		{
			if (view == Views.Connections)
			{
				this.DisplayStoryboard("Connections");
			}
			else if (view == Views.Interfaces)
			{
				this.DisplayStoryboard("Interfaces");
			}
		}

		public void DisplayStoryboard(String storyboard)
		{
			var __Storyboard = NSStoryboard.FromName(storyboard, null);

			if (__Storyboard == null)
			{
				throw new EntryPointNotFoundException("Could not find storyboard");
			}

			var __ViewController = __Storyboard.InstantiateInitialController() as NSViewController;

			if (__ViewController == null)
			{
				throw new EntryPointNotFoundException("Storyboard has no initial ViewController");
			}

//			NSSplitViewController e;
//			e.SplitViewItems[0].
			this.DisplayViewController(__ViewController);
		}

		public void DisplayViewController(NSViewController viewController)
		{
			this.ClearChildViewControllers();

			var __CurrentSize = this.View.Frame;
			this._CurrentController = viewController;
			this.AddChildViewController(this._CurrentController);
			this._CurrentController.View.Frame = __CurrentSize;
			this.View = this._CurrentController.View;
		}

		private void ClearChildViewControllers()
		{
			for (var __Index = 0; __Index < base.ChildViewControllers.Length; __Index++)
			{
				base.RemoveChildViewController(__Index);
			}
		}
	}
}
