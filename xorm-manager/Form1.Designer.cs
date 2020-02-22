namespace XormManager {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.退出XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工具TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBuildAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBuildEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemBuildTable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBuildSelector = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemBuildView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemBuildClass = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.选项OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.打开OToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBuildAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBuildEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBuildTable = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBuildSelector = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBuildView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBuildClass = new System.Windows.Forms.ToolStripButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem,
            this.工具TToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开OToolStripMenuItem,
            this.toolStripSeparator,
            this.退出XToolStripMenuItem});
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            this.文件FToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.文件FToolStripMenuItem.Text = "文件(&F)";
            // 
            // 打开OToolStripMenuItem
            // 
            this.打开OToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripMenuItem.Image")));
            this.打开OToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripMenuItem.Name = "打开OToolStripMenuItem";
            this.打开OToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.打开OToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.打开OToolStripMenuItem.Text = "打开(&O)";
            this.打开OToolStripMenuItem.Click += new System.EventHandler(this.打开OToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(162, 6);
            // 
            // 退出XToolStripMenuItem
            // 
            this.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem";
            this.退出XToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.退出XToolStripMenuItem.Text = "退出(&X)";
            this.退出XToolStripMenuItem.Click += new System.EventHandler(this.退出XToolStripMenuItem_Click);
            // 
            // 工具TToolStripMenuItem
            // 
            this.工具TToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemBuildAdd,
            this.toolStripMenuItemBuildEdit,
            this.toolStripSeparator2,
            this.toolStripMenuItemBuildTable,
            this.toolStripMenuItemBuildSelector,
            this.toolStripSeparator11,
            this.toolStripMenuItemBuildView,
            this.toolStripSeparator7,
            this.toolStripMenuItemBuildClass,
            this.toolStripSeparator1,
            this.选项OToolStripMenuItem,
            this.toolStripMenuItemBuild});
            this.工具TToolStripMenuItem.Name = "工具TToolStripMenuItem";
            this.工具TToolStripMenuItem.Size = new System.Drawing.Size(59, 21);
            this.工具TToolStripMenuItem.Text = "工具(&T)";
            // 
            // toolStripMenuItemBuildAdd
            // 
            this.toolStripMenuItemBuildAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemBuildAdd.Image")));
            this.toolStripMenuItemBuildAdd.Name = "toolStripMenuItemBuildAdd";
            this.toolStripMenuItemBuildAdd.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemBuildAdd.Text = "构建添加表单页";
            this.toolStripMenuItemBuildAdd.Click += new System.EventHandler(this.ToolStripMenuItemBuildAdd_Click);
            // 
            // toolStripMenuItemBuildEdit
            // 
            this.toolStripMenuItemBuildEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemBuildEdit.Image")));
            this.toolStripMenuItemBuildEdit.Name = "toolStripMenuItemBuildEdit";
            this.toolStripMenuItemBuildEdit.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemBuildEdit.Text = "构建修改表单页";
            this.toolStripMenuItemBuildEdit.Click += new System.EventHandler(this.ToolStripMenuItemBuildEdit_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(164, 6);
            // 
            // toolStripMenuItemBuildTable
            // 
            this.toolStripMenuItemBuildTable.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemBuildTable.Image")));
            this.toolStripMenuItemBuildTable.Name = "toolStripMenuItemBuildTable";
            this.toolStripMenuItemBuildTable.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemBuildTable.Text = "构建表格页";
            this.toolStripMenuItemBuildTable.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // toolStripMenuItemBuildSelector
            // 
            this.toolStripMenuItemBuildSelector.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemBuildSelector.Image")));
            this.toolStripMenuItemBuildSelector.Name = "toolStripMenuItemBuildSelector";
            this.toolStripMenuItemBuildSelector.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemBuildSelector.Text = "构建选择器";
            this.toolStripMenuItemBuildSelector.Click += new System.EventHandler(this.toolStripMenuItemBuildSelector_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(164, 6);
            // 
            // toolStripMenuItemBuildView
            // 
            this.toolStripMenuItemBuildView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemBuildView.Image")));
            this.toolStripMenuItemBuildView.Name = "toolStripMenuItemBuildView";
            this.toolStripMenuItemBuildView.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemBuildView.Text = "构建视图页";
            this.toolStripMenuItemBuildView.Click += new System.EventHandler(this.ToolStripMenuItemBuildView_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(164, 6);
            // 
            // toolStripMenuItemBuildClass
            // 
            this.toolStripMenuItemBuildClass.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemBuildClass.Image")));
            this.toolStripMenuItemBuildClass.Name = "toolStripMenuItemBuildClass";
            this.toolStripMenuItemBuildClass.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemBuildClass.Text = "构建主类";
            this.toolStripMenuItemBuildClass.Click += new System.EventHandler(this.ToolStripMenuItemBuildClass_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // 选项OToolStripMenuItem
            // 
            this.选项OToolStripMenuItem.Name = "选项OToolStripMenuItem";
            this.选项OToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.选项OToolStripMenuItem.Text = "XOrm 选项(&X) ...";
            this.选项OToolStripMenuItem.Click += new System.EventHandler(this.选项OToolStripMenuItem_Click);
            // 
            // toolStripMenuItemBuild
            // 
            this.toolStripMenuItemBuild.Name = "toolStripMenuItemBuild";
            this.toolStripMenuItemBuild.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemBuild.Text = "构建 选项(&B) ...";
            this.toolStripMenuItemBuild.Click += new System.EventHandler(this.ToolStripMenuItemBuild_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开OToolStripButton,
            this.toolStripSeparator6,
            this.toolStripButtonBuildAdd,
            this.toolStripButtonBuildEdit,
            this.toolStripSeparator8,
            this.toolStripButtonBuildTable,
            this.toolStripButtonBuildSelector,
            this.toolStripSeparator10,
            this.toolStripButtonBuildView,
            this.toolStripSeparator9,
            this.toolStripButtonBuildClass});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // 打开OToolStripButton
            // 
            this.打开OToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打开OToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripButton.Image")));
            this.打开OToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripButton.Name = "打开OToolStripButton";
            this.打开OToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.打开OToolStripButton.Text = "打开(&O)";
            this.打开OToolStripButton.Click += new System.EventHandler(this.打开OToolStripButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonBuildAdd
            // 
            this.toolStripButtonBuildAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildAdd.Image")));
            this.toolStripButtonBuildAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildAdd.Name = "toolStripButtonBuildAdd";
            this.toolStripButtonBuildAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildAdd.Text = "构建添加表单页";
            this.toolStripButtonBuildAdd.Click += new System.EventHandler(this.ToolStripButtonBuildAdd_Click);
            // 
            // toolStripButtonBuildEdit
            // 
            this.toolStripButtonBuildEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildEdit.Image")));
            this.toolStripButtonBuildEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildEdit.Name = "toolStripButtonBuildEdit";
            this.toolStripButtonBuildEdit.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildEdit.Text = "构建修改表单页";
            this.toolStripButtonBuildEdit.Click += new System.EventHandler(this.ToolStripButtonBuildEdit_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonBuildTable
            // 
            this.toolStripButtonBuildTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildTable.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildTable.Image")));
            this.toolStripButtonBuildTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildTable.Name = "toolStripButtonBuildTable";
            this.toolStripButtonBuildTable.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildTable.Text = "构建表格页";
            this.toolStripButtonBuildTable.Click += new System.EventHandler(this.ToolStripButtonBuildTable_Click);
            // 
            // toolStripButtonBuildSelector
            // 
            this.toolStripButtonBuildSelector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildSelector.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildSelector.Image")));
            this.toolStripButtonBuildSelector.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildSelector.Name = "toolStripButtonBuildSelector";
            this.toolStripButtonBuildSelector.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildSelector.Text = "构建选择器";
            this.toolStripButtonBuildSelector.Click += new System.EventHandler(this.toolStripButtonBuildSelector_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonBuildView
            // 
            this.toolStripButtonBuildView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildView.Image")));
            this.toolStripButtonBuildView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildView.Name = "toolStripButtonBuildView";
            this.toolStripButtonBuildView.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildView.Text = "构建视图页";
            this.toolStripButtonBuildView.Click += new System.EventHandler(this.ToolStripButtonBuildView_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonBuildClass
            // 
            this.toolStripButtonBuildClass.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBuildClass.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBuildClass.Image")));
            this.toolStripButtonBuildClass.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuildClass.Name = "toolStripButtonBuildClass";
            this.toolStripButtonBuildClass.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBuildClass.Text = "构建主类";
            this.toolStripButtonBuildClass.Click += new System.EventHandler(this.ToolStripButtonBuildClass_Click);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.ItemHeight = 20;
            this.treeView1.Location = new System.Drawing.Point(0, 50);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(784, 489);
            this.treeView1.TabIndex = 4;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "database_16.png");
            this.imageList1.Images.SetKeyName(1, "platform_16.png");
            this.imageList1.Images.SetKeyName(2, "table_16.png");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.测试ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // 测试ToolStripMenuItem
            // 
            this.测试ToolStripMenuItem.Name = "测试ToolStripMenuItem";
            this.测试ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.测试ToolStripMenuItem.Text = "测试";
            this.测试ToolStripMenuItem.Click += new System.EventHandler(this.测试ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开OToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem 退出XToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工具TToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选项OToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton 打开OToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton toolStripButtonBuildTable;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuildTable;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuild;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuildClass;
        private System.Windows.Forms.ToolStripButton toolStripButtonBuildClass;
        private System.Windows.Forms.ToolStripButton toolStripButtonBuildAdd;
        private System.Windows.Forms.ToolStripButton toolStripButtonBuildEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuildAdd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuildEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuildView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton toolStripButtonBuildView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton toolStripButtonBuildSelector;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuildSelector;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 测试ToolStripMenuItem;
    }
}

