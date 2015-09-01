using ClearbytesBridge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Clearbytes
{
    public partial class SearchParams : Form
    {
        List<Type> AllModules = new List<Type>();
        public List<Type> Modules = new List<Type>();
        List<ClearbytesModuleAttributes> AllAttribs = new List<ClearbytesModuleAttributes>();
        public List<ClearbytesModuleAttributes> Attribs = new List<ClearbytesModuleAttributes>();
        
        public bool canceled = true;
        public SearchParams()
        {
            InitializeComponent();

            #region Reflect internal modules
            Type[] classes = Assembly.GetExecutingAssembly().GetTypes();

            list.BeginUpdate();
            foreach (Type c in classes)
            {
                if (c.BaseType != typeof(ClearbytesModule) || c.Namespace != "Clearbytes.Modules")
                    continue;

                object[] mods = c.GetCustomAttributes(typeof(ClearbytesModuleAttributes), false);
                if (mods.Length < 1) continue;

                ClearbytesModuleAttributes attrib = (ClearbytesModuleAttributes)mods[0];
                if (!attrib.Active) continue;

                AllModules.Add(c);
                AllAttribs.Add(attrib);

                list.Items.Add(attrib.Name);
            }
            colModules.Width = -1;
            list.EndUpdate();
            #endregion
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            canceled = false;

            foreach (int i in list.CheckedIndices)
            {
                Modules.Add(AllModules[i]);
                Attribs.Add(AllAttribs[i]);
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
