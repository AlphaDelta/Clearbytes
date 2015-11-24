namespace Clearbytes
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menu = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileStart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileDump = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.split = new System.Windows.Forms.SplitContainer();
            this.panelTable = new System.Windows.Forms.Panel();
            this.panelBinary = new System.Windows.Forms.Panel();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.lblTitlecontent = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelImage = new System.Windows.Forms.Panel();
            this.imgData = new System.Windows.Forms.PictureBox();
            this.panelText = new System.Windows.Forms.Panel();
            this.txtData = new System.Windows.Forms.TextBox();
            this.dumpmenu = new System.Windows.Forms.MenuStrip();
            this.dumpmenuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpmenuUncheck = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpmenuCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpmenuDump = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView = new Clearbytes.TreeViewExtended();
            this.tableData = new Clearbytes.ListViewExtended();
            this.hexData = new Clearbytes.HexView();
            this.menu.SuspendLayout();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.panelTable.SuspendLayout();
            this.panelBinary.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.panelImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgData)).BeginInit();
            this.panelText.SuspendLayout();
            this.dumpmenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuHelp});
            this.menu.Location = new System.Drawing.Point(0, 24);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(863, 24);
            this.menu.TabIndex = 0;
            this.menu.Text = "menu";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileStart,
            this.menuFileDump,
            this.toolStripSeparator1,
            this.menuFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // menuFileStart
            // 
            this.menuFileStart.Name = "menuFileStart";
            this.menuFileStart.Size = new System.Drawing.Size(144, 22);
            this.menuFileStart.Text = "Start search";
            this.menuFileStart.Click += new System.EventHandler(this.menuFileStart_Click);
            // 
            // menuFileDump
            // 
            this.menuFileDump.Enabled = false;
            this.menuFileDump.Name = "menuFileDump";
            this.menuFileDump.Size = new System.Drawing.Size(144, 22);
            this.menuFileDump.Text = "Dump results";
            this.menuFileDump.Click += new System.EventHandler(this.menuFileDump_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(141, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size(144, 22);
            this.menuFileExit.Text = "Exit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(44, 20);
            this.menuHelp.Text = "Help";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(107, 22);
            this.menuHelpAbout.Text = "About";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.split.Location = new System.Drawing.Point(-1, 24);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.split.Panel1.Controls.Add(this.treeView);
            this.split.Panel1MinSize = 75;
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.SystemColors.Window;
            this.split.Panel2.Controls.Add(this.panelTable);
            this.split.Panel2.Controls.Add(this.panelBinary);
            this.split.Panel2.Controls.Add(this.panelTitle);
            this.split.Panel2.Controls.Add(this.panelImage);
            this.split.Panel2.Controls.Add(this.panelText);
            this.split.Size = new System.Drawing.Size(865, 379);
            this.split.SplitterDistance = 212;
            this.split.TabIndex = 0;
            this.split.TabStop = false;
            // 
            // panelTable
            // 
            this.panelTable.Controls.Add(this.tableData);
            this.panelTable.Location = new System.Drawing.Point(3, 215);
            this.panelTable.Name = "panelTable";
            this.panelTable.Size = new System.Drawing.Size(100, 100);
            this.panelTable.TabIndex = 4;
            // 
            // panelBinary
            // 
            this.panelBinary.Controls.Add(this.hexData);
            this.panelBinary.Location = new System.Drawing.Point(215, 109);
            this.panelBinary.Name = "panelBinary";
            this.panelBinary.Size = new System.Drawing.Size(100, 100);
            this.panelBinary.TabIndex = 2;
            // 
            // panelTitle
            // 
            this.panelTitle.Controls.Add(this.lblTitlecontent);
            this.panelTitle.Controls.Add(this.lblTitle);
            this.panelTitle.Location = new System.Drawing.Point(3, 3);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(100, 100);
            this.panelTitle.TabIndex = 3;
            // 
            // lblTitlecontent
            // 
            this.lblTitlecontent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitlecontent.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitlecontent.Location = new System.Drawing.Point(7, 28);
            this.lblTitlecontent.Name = "lblTitlecontent";
            this.lblTitlecontent.Size = new System.Drawing.Size(90, 72);
            this.lblTitlecontent.TabIndex = 1;
            this.lblTitlecontent.Text = "Content";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(3, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(47, 19);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            // 
            // panelImage
            // 
            this.panelImage.Controls.Add(this.imgData);
            this.panelImage.Location = new System.Drawing.Point(109, 109);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(100, 100);
            this.panelImage.TabIndex = 1;
            // 
            // imgData
            // 
            this.imgData.BackColor = System.Drawing.SystemColors.Control;
            this.imgData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgData.Location = new System.Drawing.Point(0, 0);
            this.imgData.Name = "imgData";
            this.imgData.Size = new System.Drawing.Size(100, 100);
            this.imgData.TabIndex = 2;
            this.imgData.TabStop = false;
            // 
            // panelText
            // 
            this.panelText.Controls.Add(this.txtData);
            this.panelText.Location = new System.Drawing.Point(3, 109);
            this.panelText.Name = "panelText";
            this.panelText.Padding = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.panelText.Size = new System.Drawing.Size(100, 100);
            this.panelText.TabIndex = 0;
            // 
            // txtData
            // 
            this.txtData.BackColor = System.Drawing.SystemColors.Window;
            this.txtData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtData.Location = new System.Drawing.Point(5, 3);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ReadOnly = true;
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(90, 94);
            this.txtData.TabIndex = 1;
            // 
            // dumpmenu
            // 
            this.dumpmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpmenuCancel,
            this.dumpmenuUncheck,
            this.dumpmenuCheck,
            this.dumpmenuDump});
            this.dumpmenu.Location = new System.Drawing.Point(0, 0);
            this.dumpmenu.Name = "dumpmenu";
            this.dumpmenu.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dumpmenu.Size = new System.Drawing.Size(863, 24);
            this.dumpmenu.TabIndex = 1;
            this.dumpmenu.Text = "dumpmenu";
            this.dumpmenu.Visible = false;
            // 
            // dumpmenuCancel
            // 
            this.dumpmenuCancel.Name = "dumpmenuCancel";
            this.dumpmenuCancel.Size = new System.Drawing.Size(55, 20);
            this.dumpmenuCancel.Text = "Cancel";
            this.dumpmenuCancel.Click += new System.EventHandler(this.dumpmenuCancel_Click);
            // 
            // dumpmenuUncheck
            // 
            this.dumpmenuUncheck.Name = "dumpmenuUncheck";
            this.dumpmenuUncheck.Size = new System.Drawing.Size(80, 20);
            this.dumpmenuUncheck.Text = "Uncheck all";
            this.dumpmenuUncheck.Click += new System.EventHandler(this.dumpmenuUncheck_Click);
            // 
            // dumpmenuCheck
            // 
            this.dumpmenuCheck.Name = "dumpmenuCheck";
            this.dumpmenuCheck.Size = new System.Drawing.Size(67, 20);
            this.dumpmenuCheck.Text = "Check all";
            this.dumpmenuCheck.Click += new System.EventHandler(this.dumpmenuCheck_Click);
            // 
            // dumpmenuDump
            // 
            this.dumpmenuDump.Name = "dumpmenuDump";
            this.dumpmenuDump.Size = new System.Drawing.Size(52, 20);
            this.dumpmenuDump.Text = "Dump";
            this.dumpmenuDump.Click += new System.EventHandler(this.dumpmenuDump_Click);
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(210, 377);
            this.treeView.TabIndex = 1;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // tableData
            // 
            this.tableData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableData.FullRowSelect = true;
            this.tableData.Location = new System.Drawing.Point(0, 0);
            this.tableData.Name = "tableData";
            this.tableData.Size = new System.Drawing.Size(100, 100);
            this.tableData.TabIndex = 0;
            this.tableData.UseCompatibleStateImageBehavior = false;
            this.tableData.View = System.Windows.Forms.View.Details;
            // 
            // hexData
            // 
            this.hexData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexData.Location = new System.Drawing.Point(0, 0);
            this.hexData.Name = "hexData";
            this.hexData.Size = new System.Drawing.Size(100, 100);
            this.hexData.TabIndex = 0;
            this.hexData.Text = "hexView1";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(863, 402);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.dumpmenu);
            this.Controls.Add(this.split);
            this.MainMenuStrip = this.menu;
            this.Name = "Main";
            this.Text = "Clearbytes";
            this.Load += new System.EventHandler(this.Main_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            this.split.ResumeLayout(false);
            this.panelTable.ResumeLayout(false);
            this.panelBinary.ResumeLayout(false);
            this.panelTitle.ResumeLayout(false);
            this.panelTitle.PerformLayout();
            this.panelImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgData)).EndInit();
            this.panelText.ResumeLayout(false);
            this.panelText.PerformLayout();
            this.dumpmenu.ResumeLayout(false);
            this.dumpmenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.SplitContainer split;
        private TreeViewExtended treeView;
        private System.Windows.Forms.Panel panelText;
        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.Panel panelBinary;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.ToolStripMenuItem menuFileStart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.Panel panelTable;
        internal System.Windows.Forms.Label lblTitle;
        internal System.Windows.Forms.Label lblTitlecontent;
        internal System.Windows.Forms.PictureBox imgData;
        internal HexView hexData;
        internal System.Windows.Forms.TextBox txtData;
        internal ListViewExtended tableData;
        private System.Windows.Forms.ToolStripMenuItem menuFileDump;
        private System.Windows.Forms.MenuStrip dumpmenu;
        private System.Windows.Forms.ToolStripMenuItem dumpmenuCancel;
        private System.Windows.Forms.ToolStripMenuItem dumpmenuUncheck;
        private System.Windows.Forms.ToolStripMenuItem dumpmenuCheck;
        private System.Windows.Forms.ToolStripMenuItem dumpmenuDump;
    }
}

