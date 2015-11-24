using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Clearbytes
{
    public class TreeViewExtended : TreeView
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }

        bool busy = false, busy2 = false;
        TreeNode busy2n = null, root = null;
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            if (busy) return;

            base.OnAfterCheck(e);
            
            if (root == null) {
                this.BeginUpdate();
                root = e.Node;
            }

            if (!busy2)
                busy2n = e.Node;
            busy2 = true;
            //TODO: Proper recursion to prevent StackOverflowException.
            foreach (TreeNode n in e.Node.Nodes)
                n.Checked = e.Node.Checked;

            if (busy2 && e.Node != busy2n) return;

            busy2n = null;
            busy2 = false;

            try {
                if (e.Node.Level < 1)
                    return;
                TreeNode parent = e.Node;
                if (e.Node.Checked)
                {
                    busy = true;
                    while ((parent = parent.Parent) != null) parent.Checked = true;
                    busy = false;
                    return;
                }

                while ((parent = parent.Parent) != null)
                {
                    bool pchecked = false;
                    foreach (TreeNode pn in parent.Nodes)
                        if (pn.Checked)
                        {
                            pchecked = true;
                            break;
                        }

                    if (pchecked) continue;

                    busy = true;
                    parent.Checked = false;
                    busy = false;
                }
            }
            finally
            {
                if (e.Node == root)
                {
                    root = null;
                    this.EndUpdate();
                }
            }
        }

        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203 /*WM_LBUTTONDBLCLK*/) { m.Result = IntPtr.Zero; }
            else base.WndProc(ref m);
        } 
    }
}
