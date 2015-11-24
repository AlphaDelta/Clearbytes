using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Clearbytes
{
    public partial class DumpForm : Form
    {
        public DumpForm()
        {
            InitializeComponent();
        }

        public void UpdateFile(string file)
        {
            this.Invoke((Async.Action)delegate { label2.Text = "File: " + file; });
        }

        private void DumpForm_Load(object sender, EventArgs e)
        {

        }
    }
}
