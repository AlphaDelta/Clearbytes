using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Clearbytes
{
    public class ListViewExtended : ListView
    {
        public ListViewExtended()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
