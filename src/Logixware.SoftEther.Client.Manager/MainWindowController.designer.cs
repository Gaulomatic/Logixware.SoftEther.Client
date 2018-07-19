// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Logixware.SoftEther.Client.Manager
{
    [Register ("MainWindowController")]
    partial class MainWindowController
    {
        [Outlet]
        AppKit.NSSegmentedControl SectionSelector { get; set; }

        [Outlet]
        AppKit.NSToolbar ToolBar { get; set; }

        [Action ("SectionSelectorChanged:")]
        partial void SectionSelectorChanged (AppKit.NSSegmentedControl sender);

        [Action ("SegmentSelected:")]
        partial void SegmentSelected (AppKit.NSSegmentedControl sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (ToolBar != null) {
                ToolBar.Dispose ();
                ToolBar = null;
            }

            if (SectionSelector != null) {
                SectionSelector.Dispose ();
                SectionSelector = null;
            }
        }
    }
}
