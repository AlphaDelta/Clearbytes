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
using System.Diagnostics;
using System.Drawing.Imaging;

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

            this.FormClosing += delegate(object sender, FormClosingEventArgs e)
            {
                if (dumpmode && MessageBox.Show("Are you sure you want to exit before completing the information dump?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = true;
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
            if (searchrunning || dumpmode)
            {
                searchcancel = true;
                return;
            }

            SearchParams sp = new SearchParams();
            sp.ShowDialog();
            if (sp.canceled) { sp.Dispose(); return; }

            menuFileDump.Enabled = false;

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

                    try { instance.Search(); }
                    catch (Exception ex)
                    {
                        this.Invoke((Async.Action)delegate
                        {
                            node.Text += " (ERROR)";
                            sp.Attribs[i].Description = ex.ToString();
                        });
                    }

                    if(WinAPI.ISABOVEVISTA)
                        WinAPI.Taskbar.SetProgressValue(mainhandle, progress, taskbarcount);
                }

                searchrunning = false;

                this.Invoke((Async.Action)delegate
                {
                    sp.Dispose();
                    menuFileStart.Text = "Start search";
                    this.Text = "Clearbytes";

                    if(treeView.Nodes.Count > 0)
                        menuFileDump.Enabled = true;

                    if (WinAPI.ISABOVEVISTA)
                        WinAPI.Taskbar.SetProgressState(this.Handle, WinAPI.TaskbarStates.NoProgress);
                });

                searchcancel = false;
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

        bool dumpmode = false;
        private void menuFileDump_Click(object sender, EventArgs e)
        {
            if (searchrunning || dumpmode) return;
            if (treeView.Nodes.Count < 1) return;

            treeView.CheckBoxes = true;
            ToggleDump(true);
        }

        DumpForm dform = null;
        private void dumpmenuDump_Click(object sender, EventArgs e)
        {
            bool anychecked = false;
            foreach(TreeNode n in treeView.Nodes)
                if (n.Checked)
                {
                    anychecked = true;
                    break;
                }

            if (!anychecked)
            {
                MessageBox.Show("No information was selected to be dumped", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ToggleDump(false);

            string dir = null;
            long timestamp = 0;
            for (int i = 0; i < 5; i++)
            {
                timestamp = (long)Math.Round(Program.FormatUnixTimestamp(DateTime.UtcNow));
                dir = Path.Combine(Program.Temp, timestamp.ToString()) + @"\";

                if (Directory.Exists(dir)) //This should never happen, but I'll account for it just in case.
                {
                    if (i >= 4)
                    {
                        MessageBox.Show("Could not create temporary folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Thread.Sleep(1000);
                    continue;
                }

                break;
            }

            Directory.CreateDirectory(dir);

            dform = new DumpForm();
            dform.Load += delegate
            {
                Async.RunAsync((Async.Action)delegate
                {
                    dumpdir = dir;
                    html = new StringBuilder();
                    html.AppendLine("<!DOCTYPE html><html><head><style type=\"text/css\">a { text-decoration: none; color: #000AFF; }</style></head><body style=\"background-color: #FFFFFF; font-family: sans-serif; white-space: nowrap;\"><ul>");
                    for (int i = 0; i < treeView.Nodes.Count; i++)
                        if (treeView.Nodes[i].Checked)
                            DumpNode(treeView.Nodes[i], ref Modules[i].Nodes, new TitleInfo(Attribs[i].Name, Attribs[i].Description), InformationType.Title);
                    html.Append("</ul></body></html>");

                    FileStream fs = null;
                    try
                    {
                        fs = File.Open(dir + "index-tree.html", FileMode.Create, FileAccess.Write, FileShare.None);


                        byte[] data = Encoding.ASCII.GetBytes(html.ToString());
                        fs.Write(data, 0, data.Length);

                        File.WriteAllText(dir + "index.html", "<!DOCTYPE html><html><head><title>Clearbytes information dump</title><style type=\"text/css\">html, body { margin: 0; padding: 0; width: 100%; height: 100%; background-color: #CCC; font-family: sans-serif; overflow: hidden; } iframe { position: absolute; top: 0; height: 95%; border: none; background-color: #FFFFFF; }</style></head><body><div style=\"padding: 4px 8px; background-color: #F0F0F0; border-bottom: 1px solid #CCC; color: #888; font-size: 12px;\"><span style=\"font-weight: 600;\">Clearbytes info-dump</span><a href=\"../archives/" + timestamp.ToString() + ".zip\" style=\"float: right;\">Download as zip</a></div><div style=\"position: relative; height: 100%;\"><iframe src=\"index-tree.html\" style=\"left: 0; width: 25%;\"></iframe><iframe name=\"viewport\" src=\"\" style=\"right: 0; width: 73%; padding-left: 1.5%\"></iframe></div></body></html>");
                    }
                    finally
                    {
                        if (fs != null && fs.CanWrite)
                            fs.Dispose();

                        ZipFolder(dir, Program.Archives + timestamp.ToString() + ".zip");

                        string browser = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\http\shell\open\command", "", null);

                        if (browser != null)
                        {
                            string prog = browser.Split('"')[1];
                            Process.Start(prog, String.Format("\"file:///{0}\"", dir + "index.html"));
                        }

                        this.Invoke((Async.Action)delegate
                        {
                            treeView.CheckBoxes = false;
                            if (dform != null) dform.Dispose();
                        });
                    }
                });
            };

            dform.ShowDialog();
        }

        public static void ZipFolder(string sourceFolder, string zipFile)
        {
            if (!System.IO.Directory.Exists(sourceFolder))
                throw new ArgumentException("sourceDirectory");

            byte[] zipHeader = new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            using (System.IO.FileStream fs = System.IO.File.Create(zipFile))
                fs.Write(zipHeader, 0, zipHeader.Length);

            Type shelltype = Type.GetTypeFromProgID("Shell.Application");
            object shell = Activator.CreateInstance(shelltype);

            Shell32.Folder source = (Shell32.Folder)shelltype.InvokeMember("NameSpace", BindingFlags.InvokeMethod, null, shell, new object[1] { sourceFolder });
            Shell32.Folder destination = (Shell32.Folder)shelltype.InvokeMember("NameSpace", BindingFlags.InvokeMethod, null, shell, new object[1] { zipFile });

            destination.CopyHere(source.Items(), 0x14);
        }

        string dumpdir = null;
        StringBuilder html = null;
        //Not sure if Lists are cloned, so I'll use ref just in case.
        void DumpNode(TreeNode n, ref List<SearchNode> nodes, object data, InformationType type)
        {
            try
            {
                if (dumpdir == null || html == null) return;

                StringBuilder b = new StringBuilder(n.FullPath.Length);
                for (int i = 0; i < n.FullPath.Length; i++)
                {
                    char c = n.FullPath[i];
                    b.Append(c < 0x20 || c > 0x7A ||
                            c == 0x22 ||
                            c == 0x2A ||
                            c == 0x2F ||
                            c == 0x3A ||
                            c == 0x3C ||
                            c == 0x3E ||
                            c == 0x3F ? '_' : c);
                }
                string safepath = b.ToString();
                string safename = Path.GetFileName(safepath);

                dform.UpdateFile(safepath);

                bool isdir = false;
                string dir = null;
                if (n.Nodes.Count > 0)
                {
                    isdir = true;
                    dir = dumpdir + safepath + @"\";
                }
                else dir = dumpdir + Path.GetDirectoryName(safepath) + @"\";

                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                string file = (isdir ? dir + "index" : dir + safename);
                string loc = Path.GetDirectoryName(safepath) + @"\" + (isdir ? safename + @"\index" : safename);

                loc = loc.TrimStart('\\');

                FileStream fs = null;
                try
                {
                    byte[] bindata = null;
                    string liname = n.Text;
                DUMP_SWITCH:
                    switch (type)
                    {
                        case InformationType.None:
                            file = "";
                            break;
                        case InformationType.Title:
                            file += ".html";
                            loc += ".html";

                            TitleInfo title = (TitleInfo)data;
                            byte[] bdata = Encoding.ASCII.GetBytes(String.Format("<!DOCTYPE html><html><body style=\"font-family: sans-serif;\"><h2>{0}</h2><p>{1}</p></body></html>", title.Title, title.Description));
                            fs = OpenFile(file);
                            fs.Write(bdata, 0, bdata.Length);

                            break;
                        case InformationType.Text:
                            file += ".txt";
                            loc += ".txt";

                            byte[] text = Encoding.ASCII.GetBytes((string)data);
                            fs = OpenFile(file);
                            fs.Write(text, 0, text.Length);
                            break;
                        case InformationType.BinaryFile:
                            file += ".txt";
                            loc += ".txt";

                            if (!File.Exists((string)data)) bindata = Encoding.ASCII.GetBytes("File not found");
                            else bindata = File.ReadAllBytes((string)data);

                            bindata = HexView.ANSI.GetBytes(FormatHexDump(bindata));
                            fs = OpenFile(file);
                            fs.Write(bindata, 0, bindata.Length);
                            break;
                        case InformationType.Binary:
                            file += ".txt";
                            loc += ".txt";

                            bindata = HexView.ANSI.GetBytes(FormatHexDump((byte[])data));
                            fs = OpenFile(file);
                            fs.Write(bindata, 0, bindata.Length);
                            break;
                        case InformationType.ImageFile:
                            string path = (string)data;
                            if (!File.Exists(path))
                            {
                                file = "";
                                liname += " (Missing)";
                                break;
                            }

                            string ext = Path.GetExtension(path);

                            while (File.Exists(file + ext))
                            {
                                file += "~";
                                loc += "~";
                            }

                            file += ext;
                            loc += ext;

                            File.Copy(path, file);
                            break;
                        case InformationType.Image:
                            Image img = (Image)data;

                            ImageFormat format = ImageFormat.Png;
                            string iext = Path.GetExtension(safename);

                            switch (iext)
                            {
                                case ".jpeg":
                                case ".jpg":
                                    format = ImageFormat.Jpeg;
                                    break;
                                case ".gif":
                                    format = ImageFormat.Gif;
                                    break;
                                case ".png":
                                    break;
                                default:
                                    file += ".png";
                                    loc += ".png";
                                    break;
                            }

                            img.Save(file, format);
                            break;
                        case InformationType.Table:
                            file += ".html";
                            loc += ".html";

                            TableInfo tinfo = (TableInfo)data;

                            StringBuilder thead = new StringBuilder();
                            foreach (string col in tinfo.Columns)
                                thead.Append(String.Format("<th>{0}</th>", col));

                            StringBuilder tbody = new StringBuilder();
                            foreach (ListViewItem row in tinfo.Rows)
                            {
                                string add = "";
                                if (row.BackColor != Color.Transparent && row.BackColor != Color.White)
                                    add = String.Format(" style=\"background-color: #{0}{1}{2} !important;\"", row.BackColor.R.ToString("X2"), row.BackColor.G.ToString("X2"), row.BackColor.B.ToString("X2"));

                                tbody.Append("<tr>");
                                bool rowtoggle = false;
                                foreach (ListViewItem.ListViewSubItem col in row.SubItems)
                                {
                                    rowtoggle = !rowtoggle;
                                    tbody.Append(String.Format("<td class=\"col-{1}\"{2}>{0}</td>", col.Text, (rowtoggle ? 0 : 1), add));
                                }
                                tbody.Append("</tr>");
                            }

                            byte[] tdata = Encoding.ASCII.GetBytes(
                                String.Format("<!DOCTYPE html><html><head><style type=\"text/css\">html, body, table {{ margin: 0; padding: 0; width: 100%; white-space: nowrap !important; border-collapse: collapse; }} td, th {{ padding: 2px 5px;  }} tr:hover td {{ background-color: #CCC !important; }} .col-0 {{ background-color: #FFF; }} .col-1 {{ background-color: #F0F0F0; }}</style></head><body style=\"font-family: sans-serif;\"><table><tbody><tr style=\"background-color: #EEE; border-bottom: 1px solid #CCC;;\">{0}</tr>{1}</tbody></table></body></html>", thead.ToString(), tbody.ToString()));
                            fs = OpenFile(file);
                            fs.Write(tdata, 0, tdata.Length);
                            break;
                        case InformationType.Delegate:
                            Program.instance.dumpmode = true;
                            Action del = (Action)data;
                            del();
                            Program.instance.dumpmode = false;

                            type = Program.instance.dumptype;
                            data = Program.instance.dumpobj;

                            goto DUMP_SWITCH;
                    }

                    if (file != "")
                        html.AppendLine(String.Format("<li><a href=\"{0}\" target=\"viewport\">{1}</a></li>", loc, liname));
                    else
                        html.AppendLine(String.Format("<li>{0}</li>", safename));
                }
                finally
                {
                    if (fs != null && fs.CanWrite)
                        fs.Dispose();
                }

                if (nodes != null && nodes.Count > 0)
                {
                    html.AppendLine("<ul>");
                    for (int i = 0; i < n.Nodes.Count; i++)
                        if (n.Nodes[i].Checked)
                            DumpNode(n.Nodes[i], ref nodes[i].Nodes, nodes[i].Data, nodes[i].Type);
                    html.AppendLine("</ul>");
                }
            }
            catch (PathTooLongException) { }
        }

        FileStream OpenFile(string path) { return File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None); }

        string FormatHexDump(byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Hex dump  00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F");
            sb.AppendLine();

            int lines = (int)Math.Ceiling((float)data.Length / 0x10);
            int offset = 0;
            byte[] asciibytes = new byte[0x10];
            for (int i = 0; i < lines; i++, offset += 0x10)
            {
                sb.Append(i.ToString("X7") + "0  ");

                int j = 0;
                for (; j < 0x10 && offset + j < data.Length; j++)
                {
                    byte bt = data[offset + j];
                    sb.Append(bt.ToString("X2") + " ");

                    if (bt > 0x1F &&
                        bt != 0x7F &&
                        bt != 0x81 &&
                        bt != 0x8D &&
                        bt != 0x8F &&
                        bt != 0x90 &&
                        bt != 0x98 &&
                        bt != 0x9D &&
                        bt != 0xAD) asciibytes[j] = bt;//asciibyte = (char)bt;
                    else asciibytes[j] = 0x2E;
                }

                int missing = 0x10 - j;
                for (int k = 0; k < missing; k++) sb.Append("   ");

                sb.AppendLine(" " + HexView.ANSI.GetString(asciibytes, 0, j));
            }

            return sb.ToString();
        }

        private void dumpmenuCheck_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in treeView.Nodes) n.Checked = true;
        }

        private void dumpmenuUncheck_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in treeView.Nodes) n.Checked = false;
        }

        private void dumpmenuCancel_Click(object sender, EventArgs e)
        {
            ToggleDump(false);
            treeView.CheckBoxes = false;
        }

        void ToggleDump(bool state)
        {
            dumpmode = state;
            menuFileStart.Enabled = !state;
            menuFileDump.Enabled = !state;

            dumpmenu.Visible = state;
            menu.Visible = !state;
        }
    }
}
