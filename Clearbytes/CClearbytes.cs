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

        public void SwitchPanel(InformationType type) { main.SwitchPanel(type); }

        public void SetImage(Image img) { main.imgData.Image = img; }
        public void SetText(string txt) { main.txtData.Text = txt; }
        public void SetBinary(byte[] arr) { main.hexData.ReadBytes(arr); }
        public void SetBinary(string file) { main.hexData.ReadFile(file); }
        public void SetTitle(TitleInfo info) { main.lblTitle.Text = info.Title; main.lblTitlecontent.Text = info.Description; }
        public void SetTable(TableInfo info)
        {
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
