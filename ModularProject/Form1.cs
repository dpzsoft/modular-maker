using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dpz3;

namespace ModularProject {
    public partial class Form1 : Form {
        // 当前选中的Node
        private TreeNode node = null;
        // 页面信息缓存
        private dpz3.KeyValues<string> _pages;
        internal dpz3.Json.JsonUnit Package { get; private set; }

        public Form1(string path = null) {
            it.WorkPathInit = path;
            // it.WorkPath = path;
            InitializeComponent();
        }

        #region [=====界面显示=====]

        /// <summary>
        /// 在状态栏显示字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="wait"></param>
        public void Say(string content, int wait = 10) {
            this.toolStripStatusLabel1.Text = content;
            Application.DoEvents();
            System.Threading.Thread.Sleep(wait);
        }

        // 加载页面
        private void LoadPages(TreeNode node, string path) {
            // 判断当前目录是否存在配置文件
            if (!path.EndsWith("\\")) path += "\\";
            string jsonPath = $"{path}config.json";
            if (System.IO.File.Exists(jsonPath)) {
                // 读取文件内容
                string content = dpz3.File.UTF8File.ReadAllText(jsonPath);
                using (var json = dpz3.Json.Parser.ParseJson(content)) {
                    // 产生新的信息
                    string workFolderPages = it.WorkPath + "\\wwwroot\\";
                    string pathPart = path.Substring(workFolderPages.Length, path.Length - workFolderPages.Length - 1);
                    string guid = null;
                    do {
                        guid = Guid.NewGuid().ToString();
                    } while (_pages.ContainsKey(guid));
                    _pages[guid] = path;
                    // 设置表节点
                    TreeNode nodePage = new TreeNode();
                    nodePage.Name = guid;
                    nodePage.Text = $"{json.Str["Name"]} [{pathPart}]";
                    nodePage.ImageIndex = 4;
                    nodePage.SelectedImageIndex = 4;
                    nodePage.ContextMenuStrip = this.contextMenuPage;
                    node.Nodes.Add(nodePage);
                }
            } else {
                // 遍历子目录查询
                string[] dirs = System.IO.Directory.GetDirectories(path);
                foreach (var dir in dirs) LoadPages(node, dir);
            }
        }

        // 加载树状目录
        private void LoadTree() {

            Say("正在进行版本校验...");
            var group = it.WorkCfg["XOrm"];
            string workFolder = it.WorkPath + it.Path_WorkFolder;
            string workFolderXorm = it.WorkPath + "\\xorm";
            string workFolderPages = it.WorkPath + "\\wwwroot";
            string workFolderModular = it.WorkPath + "\\modular.json";
            string workVersionPath = workFolder + group["VersionPath"];
            string workSettingPath = workFolder + group["SettingPath"];
            string localVer = dpz3.XOrm.Setting.GetVersionCacheFromFile(workVersionPath);
            string remoteVer = dpz3.XOrm.Setting.GetVersionCacheFromUrl(group["Url"] + group["VersionPath"]);

            //// 对比数据
            //if (localVer != remoteVer) {
            //    Say("下载最新主配置文件...");
            //    dpz3.Net.HttpClient.Download(group["Url"] + group["SettingPath"], workSettingPath, (long total, long loaded) => {
            //        Say($"下载最新主配置文件({loaded}/{total})...");
            //    });
            //    setting = dpz3.XOrm.Setting.LoadFromFile(workSettingPath);
            //    setting.SaveVersionCacheToFile(workVersionPath);
            //    Say("下载成功");
            //} else {
            //    setting = dpz3.XOrm.Setting.LoadFromFile(workSettingPath);
            //}

            this.treeView1.Nodes.Clear();

            // 读取配置文件
            string content = dpz3.File.UTF8File.ReadAllText(workFolderModular);
            this.Package = dpz3.Json.Parser.ParseJson(content);
            // 读取包信息
            string proName = this.Package.Str["name"];
            string proVersion = this.Package.Str["version"];
            string proDescription = this.Package.Str["description"];
            // 设置根节点
            Say("载入树形列表...");
            TreeNode nodeRoot = new TreeNode();
            nodeRoot.Name = "Lv0_Root";
            nodeRoot.Text = $"{proDescription} [{proName}:{proVersion}]";
            nodeRoot.ImageIndex = 0;
            this.treeView1.Nodes.Add(nodeRoot);
            // 设置对象关系节点
            TreeNode nodeXorm = new TreeNode();
            nodeXorm.Name = "Lv1_Xorm";
            nodeXorm.Text = "对象关系映射(Xorm)";
            nodeXorm.ImageIndex = 1;
            nodeXorm.SelectedImageIndex = 1;
            nodeRoot.Nodes.Add(nodeXorm);
            // 设置页面节点
            TreeNode nodePage = new TreeNode();
            nodePage.Name = "Lv1_Page";
            nodePage.Text = "模块页面(Page)";
            nodePage.ImageIndex = 3;
            nodePage.SelectedImageIndex = 3;
            nodeRoot.Nodes.Add(nodePage);
            // 加载平台
            Say("载入映射表格列表...");
            LoadTables(nodeXorm, workFolderXorm);
            // 加载页面
            Say("载入页面列表...");
            _pages.Clear();
            nodePage.Nodes.Clear();
            LoadPages(nodePage, workFolderPages);
            // 展开根节点
            nodeRoot.Expand();
            Say("载入成功");
        }

        // 加载平台
        private void LoadPlatforms(TreeNode node, dpz3.XOrm.Setting setting) {

            node.Nodes.Clear();

            // 加载所有的平台
            foreach (var platform in setting.Platforms) {

                // 设置平台节点
                TreeNode nodePlatform = new TreeNode();
                nodePlatform.Name = platform.Name;
                nodePlatform.Text = $"{platform.Name} - {platform.Title}";
                nodePlatform.ImageIndex = 1;
                nodePlatform.SelectedImageIndex = 1;
                node.Nodes.Add(nodePlatform);

                // 加载表
                // LoadTables(nodePlatform, platform);

            }
        }

        // 加载表
        private void LoadTables(TreeNode node, string path) {

            node.Nodes.Clear();

            string[] files = System.IO.Directory.GetFiles(path, "*.xml");

            // 加载所有的表
            foreach (var file in files) {
                // 读取文件内容
                string content = dpz3.File.UTF8File.ReadAllText(file);
                using (var doc = dpz3.Xml.Parser.GetDocument(content)) {
                    // 读取配置
                    var table = doc["table"];
                    var tableName = table.Attr["name"];
                    var tableVersion = table.Attr["version"];
                    var tableTitle = table.Attr["title"];
                    // 设置表节点
                    TreeNode nodeTable = new TreeNode();
                    nodeTable.Name = tableName;
                    nodeTable.Text = $"{tableTitle} [{tableName}:{tableVersion}]";
                    nodeTable.ImageIndex = 2;
                    nodeTable.SelectedImageIndex = 2;
                    nodeTable.ContextMenuStrip = this.contextMenuTable;
                    node.Nodes.Add(nodeTable);
                }

            }
        }

        /// <summary>
        /// 设置工作目录
        /// </summary>
        /// <param name="path"></param>
        public void SetWorkPath(string path) {
            it.WorkPath = path;
            if (it.WorkPath.EndsWith("\\")) it.WorkPath = it.WorkPath.Substring(0, it.WorkPath.Length - 1);
            this.Text = $"{Application.ProductName} Ver:{Application.ProductVersion} - [{it.WorkPath}]";

            // 创建工作文件夹
            if (!System.IO.Directory.Exists(it.WorkPath + it.Path_WorkFolder)) {
                System.IO.Directory.CreateDirectory(it.WorkPath + it.Path_WorkFolder);
            }

            it.WorkCfg = new dpz3.File.ConfFile(it.WorkPath + it.Path_WorkFolder + it.Path_WorkConfig);
            var workCfg = it.WorkCfg;
            var workGroup = workCfg["XOrm"];
            if (workGroup["Url"].IsNoneOrNull()) {
                workGroup["Url"] = "https://dev.lywos.com/Orm/Xml2";
                workGroup["SettingPath"] = "/Setting.xml";
                workGroup["VersionPath"] = "/Version.xml";
                workCfg.Save();
            }

            var workBuildGroup = workCfg["Build"];
            if (workBuildGroup["PagePath"].IsNoneOrNull()) {
                workBuildGroup["PagePath"] = "\\wwwroot\\pages";
                workBuildGroup["ClassPath"] = "\\Orm";
                workBuildGroup["NameSpace"] = "Entity.Orm";
                workCfg.Save();
            }

            this.LoadTree();
        }

        // 打开一个目录进行编辑
        private void OpenFolder() {

            // 判断当前是否有项目已经打开
            if (!it.WorkPath.IsNoneOrNull()) {
                if (MessageBox.Show("是否退出当前的工作目录，并打开一个新的?", "工作目录切换提醒", MessageBoxButtons.YesNo) == DialogResult.No) {
                    return;
                }
            }

            // 加载历史记录
            string lastPath = null;
            using (dpz3.File.ConfFile conf = new dpz3.File.ConfFile(it.LocalPath + it.Path_Config)) {
                var group = conf["History"];
                lastPath = group["it.WorkPath"];


                if (lastPath.IsNoneOrNull()) lastPath = it.LocalPath;

                using (FolderBrowserDialog f = new FolderBrowserDialog()) {
                    f.SelectedPath = lastPath;
                    if (f.ShowDialog() == DialogResult.OK) {

                        // 存储历史记录
                        group["it.WorkPath"] = f.SelectedPath;
                        conf.Save();

                        // 设置工作目录
                        this.SetWorkPath(f.SelectedPath);
                        //it.WorkPath = f.SelectedPath;

                        // 设置资源管理器
                        //frmSetting.LoadSetting(it.WorkPath);
                    } else {
                        return;
                    }
                }

            }

        }

        #endregion

        private void Form1_Load(object sender, EventArgs e) {

            // 获取本地路径
            it.LocalPath = Application.StartupPath;
            if (!it.LocalPath.EndsWith("\\")) it.LocalPath += "\\";

            // 模板目录
            it.LocalTemplateFolder = it.LocalPath + it.Path_TemplateFolder;
            if (!System.IO.Directory.Exists(it.LocalTemplateFolder)) System.IO.Directory.CreateDirectory(it.LocalTemplateFolder);

            // 设置标题
            this.Text = $"{Application.ProductName} Ver:{Application.ProductVersion}";

            // 工具栏初始化
            this.toolStripMenuItemBuildAdd.Enabled = false;
            this.toolStripMenuItemBuildEdit.Enabled = false;
            this.toolStripMenuItemBuildTable.Enabled = false;
            this.toolStripMenuItemBuildSelector.Enabled = false;
            this.toolStripMenuItemBuildView.Enabled = false;
            this.toolStripMenuItemBuildClass.Enabled = false;

            this.toolStripButtonBuildAdd.Enabled = false;
            this.toolStripButtonBuildEdit.Enabled = false;
            this.toolStripButtonBuildTable.Enabled = false;
            this.toolStripButtonBuildSelector.Enabled = false;
            this.toolStripButtonBuildView.Enabled = false;
            this.toolStripButtonBuildClass.Enabled = false;

            // 初始化变量
            _pages = new KeyValues<string>();

            this.Visible = true;
            Application.DoEvents();

            if (!it.WorkPathInit.IsNoneOrNull()) {
                this.SetWorkPath(it.WorkPathInit);
            }
        }

        private void 选项OToolStripMenuItem_Click(object sender, EventArgs e) {

            if (it.WorkPath.IsNoneOrNull()) {
                MessageBox.Show("尚未指定工作目录");
                return;
            }

            //using (FrmSetting f = new FrmSetting()) {
            //    f.SetConfigGroup(workCfg["XOrm"]);
            //    f.SetConfigIitem("Url");
            //    f.SetConfigIitem("SettingPath");
            //    f.SetConfigIitem("VersionPath");
            //    f.ShowDialog();
            //    workCfg.Save();
            //}
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e) {
            this.OpenFolder();
        }

        private void 打开OToolStripButton_Click(object sender, EventArgs e) {
            this.OpenFolder();
        }

        // 刷新
        private void toolRefresh_Click(object sender, EventArgs e) {
            // 加载树形列表
            this.LoadTree();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void ToolStripButtonBuildTable_Click(object sender, EventArgs e) {
            PageBuilder.BuildListPage(this, node, "List");
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            node = e.Node;

            switch (node.ImageIndex) {
                case 2:
                    // 表
                    this.toolStripMenuItemBuildAdd.Enabled = true;
                    this.toolStripMenuItemBuildEdit.Enabled = true;
                    this.toolStripMenuItemBuildTable.Enabled = true;
                    this.toolStripMenuItemBuildSelector.Enabled = true;
                    this.toolStripMenuItemBuildView.Enabled = true;
                    this.toolStripMenuItemBuildClass.Enabled = true;

                    this.toolStripButtonBuildAdd.Enabled = true;
                    this.toolStripButtonBuildEdit.Enabled = true;
                    this.toolStripButtonBuildTable.Enabled = true;
                    this.toolStripButtonBuildSelector.Enabled = true;
                    this.toolStripButtonBuildView.Enabled = true;
                    this.toolStripButtonBuildClass.Enabled = true;

                    break;
                default:
                    this.toolStripMenuItemBuildAdd.Enabled = false;
                    this.toolStripMenuItemBuildEdit.Enabled = false;
                    this.toolStripMenuItemBuildTable.Enabled = false;
                    this.toolStripMenuItemBuildSelector.Enabled = false;
                    this.toolStripMenuItemBuildView.Enabled = false;
                    this.toolStripMenuItemBuildClass.Enabled = false;

                    this.toolStripButtonBuildAdd.Enabled = false;
                    this.toolStripButtonBuildEdit.Enabled = false;
                    this.toolStripButtonBuildTable.Enabled = false;
                    this.toolStripButtonBuildSelector.Enabled = false;
                    this.toolStripButtonBuildView.Enabled = false;
                    this.toolStripButtonBuildClass.Enabled = false;
                    break;
            }
        }

        private void ToolStripMenuItemBuild_Click(object sender, EventArgs e) {
            if (it.WorkPath.IsNoneOrNull()) {
                MessageBox.Show("尚未指定工作目录");
                return;
            }

            //using (FrmSetting f = new FrmSetting()) {
            //    f.SetConfigGroup(workCfg["Build"]);
            //    f.SetConfigIitem("PagePath");
            //    f.SetConfigIitem("ClassPath");
            //    f.SetConfigIitem("ControllerPath");
            //    f.SetConfigIitem("NameSpace");
            //    f.ShowDialog();
            //    workCfg.Save();
            //}
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e) {
            PageBuilder.BuildListPage(this, node, "List");
        }

        private void ToolStripMenuItemBuildClass_Click(object sender, EventArgs e) {
            PageBuilder.BuildClass(this, node);
            PageBuilder.BuildControllerClass(this, node);
        }

        private void ToolStripButtonBuildClass_Click(object sender, EventArgs e) {
            PageBuilder.BuildClass(this, node);
            PageBuilder.BuildControllerClass(this, node);
        }

        private void ToolStripButtonBuildAdd_Click(object sender, EventArgs e) {
            PageBuilder.BuildFormPage(this, node, "Add");
        }

        private void ToolStripMenuItemBuildAdd_Click(object sender, EventArgs e) {
            PageBuilder.BuildFormPage(this, node, "Add");
        }

        // 弹出菜单
        private void MenuBuildAdd_Click(object sender, System.EventArgs e) {
            PageBuilder.BuildFormPage(this, node, "Add");
        }

        private void ToolStripMenuItemBuildEdit_Click(object sender, EventArgs e) {
            PageBuilder.BuildFormPage(this, node, "Edit");
        }

        private void ToolStripButtonBuildEdit_Click(object sender, EventArgs e) {
            PageBuilder.BuildFormPage(this, node, "Edit");
        }

        private void ToolStripButtonBuildView_Click(object sender, EventArgs e) {
            PageBuilder.BuildViewPage(this, node);
        }

        private void ToolStripMenuItemBuildView_Click(object sender, EventArgs e) {
            PageBuilder.BuildViewPage(this, node);
        }

        private void toolStripButtonBuildSelector_Click(object sender, EventArgs e) {
            PageBuilder.BuildListPage(this, node, "Selector");
        }

        private void toolStripMenuItemBuildSelector_Click(object sender, EventArgs e) {
            PageBuilder.BuildListPage(this, node, "Selector");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        // 构建页面
        private void menuPageBuild_Click(object sender, System.EventArgs e) {
            //PageBuilder.BuildFormPage(this, node, "Add");
            if (node == null) return;
            string dir = _pages[node.Name];
            // MessageBox.Show(dir);
            string mainFilePath = dir + "src\\main.htm";
            if (System.IO.File.Exists(mainFilePath)) {
                PageBuilder.BuildMainPage(this, node, it.WorkPath, dir + "src", mainFilePath, dir + "page.html");
                this.Say($"完成");
            } else {
                this.Say($"未发现main.htm文件");
            }
        }

    }
}
