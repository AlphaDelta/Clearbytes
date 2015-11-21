using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ClearbytesBridge
{
    public class TableInfo
    {
        public readonly string[] Columns;
        public readonly ListViewItem[] Rows;

        public TableInfo(string[] Columns, ListViewItem[] Rows)
        {
            this.Columns = Columns;
            this.Rows = Rows;
        }
    }
}
