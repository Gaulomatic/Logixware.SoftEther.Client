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
    [Register ("InterfacesViewController")]
    partial class InterfacesViewController
    {
        [Outlet]
        AppKit.NSButton AddButton { get; set; }

        [Outlet]
        AppKit.NSTableView InterfacesTableView { get; set; }

        [Outlet]
        AppKit.NSButton RemoveButton { get; set; }

        [Action ("InterfacesTableViewChanged:")]
        partial void InterfacesTableViewChanged (AppKit.NSTableView sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (InterfacesTableView != null) {
                InterfacesTableView.Dispose ();
                InterfacesTableView = null;
            }

            if (AddButton != null) {
                AddButton.Dispose ();
                AddButton = null;
            }

            if (RemoveButton != null) {
                RemoveButton.Dispose ();
                RemoveButton = null;
            }
        }
    }
}
