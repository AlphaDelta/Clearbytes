using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ClearbytesBridge;
using System.Threading;
using System.IO;

namespace Clearbytes
{
    public partial class Main : Form
    {
        List<ClearbytesModule> Modules = new List<ClearbytesModule>();
        List<ClearbytesModuleAttributes> Attribs = new List<ClearbytesModuleAttributes>();

        public Main()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.icon;

            

            split.GotFocus += delegate { this.Focus(); };

            panelTitle.Visible = false;
            panelText.Visible = false;
            panelImage.Visible = false;
            panelBinary.Visible = false;
            panelTable.Visible = false;
            panelTitle.Dock = DockStyle.Fill;
            panelText.Dock = DockStyle.Fill;
            panelImage.Dock = DockStyle.Fill;
            panelBinary.Dock = DockStyle.Fill;
            panelTable.Dock = DockStyle.Fill;

            txtData.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.Control && e.KeyCode == Keys.A)
                    txtData.SelectAll();
            };
        }

        private void Main_Load(object sender, EventArgs e)
        {
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            using(About about = new About())
                about.ShowDialog();
        }

        public void SwitchPanel(InformationType type)
        {
            panelTitle.Visible = false;
            panelTitle.Visible = (type == InformationType.Title);
            panelText.Visible = (type == InformationType.Text);
            panelImage.Visible = (type == InformationType.Image || type == InformationType.ImageFile);
            panelBinary.Visible = (type == InformationType.Binary || type == InformationType.BinaryFile);
            panelTable.Visible = (type == InformationType.Table);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                lblTitle.Text = Attribs[e.Node.Index].Name;
                lblTitlecontent.Text = Attribs[e.Node.Index].Description;

                SwitchPanel(InformationType.Title);
            }
            else// if (e.Node.Level == 1)
            {
                //Resolve node path
                int[] indices = new int[e.Node.Level + 1];
                TreeNode tn = e.Node;
                int pos = e.Node.Level;
                do
                {
                    indices[pos] = tn.Index;
                    pos--;
                } while ((tn = tn.Parent) != null);// && tn.Level != 0);

                if(indices.Length < 2) return;

                //Resolve SearchNode
                ClearbytesModule root = Modules[indices[0]];
                SearchNode n = root.Nodes[indices[1]];

                for (int i = 2; i < indices.Length; i++)
                    n = n.Nodes[indices[i]];

                //SearchNode n = root.Nodes[e.Node.Index];
                //for (int i = e.Node.Level; i > 0; i--)
                //    n = n.Nodes
                //SearchNode n = root.Nodes[e.Node.Index];

                if (n.Data == null)
                    SwitchPanel(InformationType.None);
                else
                {
                IT_SWITCH:
                    switch (n.Type)
                    {
                        case InformationType.Title:
                            lblTitle.Text = ((TitleInfo)n.Data).Title;
                            lblTitlecontent.Text = ((TitleInfo)n.Data).Description;
                            break;
                        case InformationType.Text:
                            txtData.Text = (string)n.Data;
                            break;
                        case InformationType.Image:
                            imgData.Image = (Image)n.Data;
                            break;
                        case InformationType.ImageFile:
                            if (!File.Exists((string)n.Data))
                            {
                                n.Type = InformationType.Text;
                                n.Data = "File is missing";
                                goto IT_SWITCH;
                            }
                            imgData.ImageLocation = (string)n.Data;
                            //imgData.Image = Image.FromFile((string)n.Data); //Necessary for some GIFs to load correctly
                            break;
                        case InformationType.Binary:
                            hexData.ReadBytes((byte[])n.Data);
                            break;
                        case InformationType.BinaryFile:
                            hexData.ReadFile((string)n.Data);
                            break;
                        case InformationType.Table:
                            TableInfo info = (TableInfo)n.Data;

                            tableData.Clear();

                            tableData.BeginUpdate();
                            foreach (string s in info.Columns)
                                tableData.Columns.Add(s);

                            foreach (ListViewItem item in info.Rows)
                                tableData.Items.Add(item);

                            foreach (ColumnHeader ch in tableData.Columns)
                                ch.Width = -1;
                            tableData.EndUpdate();
                            break;
                        case InformationType.Delegate:
                            Action del = (Action)n.Data;
                            del();
                            break;
                    }
                    if (n.Type != InformationType.Delegate) SwitchPanel(n.Type);
                }
            }
        }

        static volatile bool searchrunning = false, searchcancel = false;
        public static bool SearchCanceled { get { return searchcancel; } }
        private void menuFileStart_Click(object sender, EventArgs e)
        {
            if (searchrunning)
            {
                searchcancel = true;
                return;
            }

            SearchParams sp = new SearchParams();
            sp.ShowDialog();
            if (sp.canceled) { sp.Dispose(); return; }

            SwitchPanel(InformationType.None);

            //Clear TreeView and release bitmap data
            treeView.Nodes.Clear();
            foreach (Image img in Bridge.ImageCache)
                img.Dispose();
            Bridge.ImageCache.Clear();
            Modules.Clear();
            Attribs.Clear();

            menuFileStart.Text = "Cancel search";
            this.Text = "Clearbytes - Searching";

            ulong taskbarcount = (ulong)sp.Modules.Count;
            if (WinAPI.ISABOVEVISTA)
            {
                WinAPI.Taskbar.SetProgressState(this.Handle, WinAPI.TaskbarStates.Normal);
                WinAPI.Taskbar.SetProgressValue(this.Handle, 0, taskbarcount);
            }

            #region Reflect internal modules
            IntPtr mainhandle = this.Handle;
            Async.RunAsync(delegate
            {
                searchrunning = true;
                /*Type[] classes = Assembly.GetExecutingAssembly().GetTypes();

                foreach (Type c in classes)
                {
                    if (searchcancel) break;

                    if (c.BaseType != typeof(ClearbytesModule) || c.Namespace != "Clearbytes.Modules")
                        continue;

                    object[] mods = c.GetCustomAttributes(typeof(ClearbytesModuleAttributes), false);
                    if (mods.Length < 1) continue;

                    ClearbytesModuleAttributes attrib = (ClearbytesModuleAttributes)mods[0];
                    if (!attrib.Active) continue;

                    ClearbytesModule instance = (ClearbytesModule)Activator.CreateInstance(c);

                    Modules.Add(instance);
                    Attribs.Add(attrib);

                    TreeNode node = new TreeNode(attrib.Name);
                    //this.Invoke((Async.Action)delegate { treeView.Nodes.Add(node); });
                    instance.SetParent(node);
                    instance.SetParentTreeView(treeView);

                    //try
                    //{
                        instance.Search();
                    //}
                    //catch { node.Text += " (ERROR)"; }
                }*/

                ulong progress = 1;
                for (int i = 0; i < sp.Modules.Count; i++, progress++)
                {
                    if (searchcancel) break;

                    ClearbytesModule instance = (ClearbytesModule)Activator.CreateInstance(sp.Modules[i]);

                    Modules.Add(instance);
                    Attribs.Add(sp.Attribs[i]);

                    TreeNode node = new TreeNode(sp.Attribs[i].Name);
                    //this.Invoke((Async.Action)delegate { treeView.Nodes.Add(node); });
                    instance.SetParent(node);
                    instance.SetParentTreeView(treeView);

                    instance.Search();

                    if(WinAPI.ISABOVEVISTA)
                        WinAPI.Taskbar.SetProgressValue(mainhandle, progress, taskbarcount);
                }

                searchrunning = false;
                searchcancel = false;

                this.Invoke((Async.Action)delegate
                {
                    sp.Dispose();
                    menuFileStart.Text = "Start search";
                    this.Text = "Clearbytes";

                    if (WinAPI.ISABOVEVISTA)
                        WinAPI.Taskbar.SetProgressState(this.Handle, WinAPI.TaskbarStates.NoProgress);
                });
            });
            #endregion
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            if (searchrunning)
            {
                searchcancel = true;

                //Wait for search to end gracefully, if it doesn't after 10 seconds then force-quit
                for (int i = 0; i < 100; i++)
                {
                    if (!searchrunning) break;

                    Thread.Sleep(100);
                }
            }
            this.Close();
        }
    }
}
