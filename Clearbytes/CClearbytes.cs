using System;
using System.Collections.Generic;
using System.Text;
using ClearbytesBridge;
using System.Drawing;
using System.Windows.Forms;

namespace Clearbytes
{
    internal class CClearbytes : IClearbytes
    {
        Main main;
        public CClearbytes(Main m) { main = m; }

        public void SwitchPanel(InformationType type) { if (dumpmode) dumptype = type; else main.SwitchPanel(type); }

        internal bool dumpmode = false;
        internal InformationType dumptype = InformationType.None;
        internal object dumpobj = null;
        public void SetImage(Image img) { if (dumpmode) dumpobj = img; else main.imgData.Image = img; }
        public void SetText(string txt) { if (dumpmode) dumpobj = txt; else main.txtData.Text = txt; }
        public void SetBinary(byte[] arr) { if (dumpmode) dumpobj = arr; else main.hexData.ReadBytes(arr); }
        public void SetBinary(string file) { if (dumpmode) dumpobj = file; else main.hexData.ReadFile(file); }
        public void SetTitle(TitleInfo info) { if (dumpmode) { dumpobj = info; return; } main.lblTitle.Text = info.Title; main.lblTitlecontent.Text = info.Description; }
        public void SetTable(TableInfo info)
        {
            if (dumpmode) { dumpobj = info; return; } 

            main.tableData.Clear();

            main.tableData.BeginUpdate();
            foreach (string s in info.Columns)
                main.tableData.Columns.Add(s);

            foreach (ListViewItem item in info.Rows)
                main.tableData.Items.Add(item);

            foreach (ColumnHeader ch in main.tableData.Columns)
                ch.Width = -1;
            main.tableData.EndUpdate();
        }

        public bool IsCanceled() { return Main.SearchCanceled; }
    }
}
