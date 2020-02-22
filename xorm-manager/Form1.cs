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

namespace XormManager {
    public partial class Form1 : Form {

        // 配置文件
        private const string Path_WorkFolder = "\\.XormManager";
        private const string Path_WorkConfig = "\\XormManager.conf";
        private const string Path_Config = "manager.conf";
        private const string Path_TemplateFolder = "template";

        // 工作配置
        private dpz3.File.ConfFile workCfg;

        // 工作目录
        private string workPath = null;
        private string workPathInit = null;

        // 当前目录
        private string localPath = null;
        private string localTemplateFolder = null;

        // 配置信息
        private dpz3.XOrm.Setting setting = null;

        // 当前选中的Node
        private TreeNode node = null;

        public Form1(string path = null) {
            workPathInit = path;
            // workPath = path;
            InitializeComponent();
        }

        #region [=====各类构建器=====]

        // 构建表格页
        private void BuildListPage(string ListType) {
            if (node == null) return;

            // 关闭状态栏
            this.toolStripButtonBuildTable.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = workPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];

                // 拼接输出路径
                var rootFolder = $"{workPath}\\wwwroot";
                var mainFolder = $"{rootFolder}\\{tableName.ToLower()}\\{ListType.ToLower()}";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";

                // 自动建立模板路径
                var templateFolder = $"{localTemplateFolder}\\{ListType}";
                if (!System.IO.Directory.Exists(templateFolder)) {
                    System.IO.Directory.CreateDirectory(templateFolder);
                }

                // 自动建立路径
                if (!System.IO.Directory.Exists(srcFolder)) {
                    System.IO.Directory.CreateDirectory(srcFolder);
                }

                // 判断是否存在配置文件，不存在则创建
                if (!System.IO.File.Exists(configFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\config.json", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, table));
                }

                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, table));
                }

                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, table));

                }

                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, table));

                }

                // 从输出路径加载xml设定,达到兼容目的
                Say("正在加载现有ORM文件...");
                string xml = dpz3.File.UTF8File.ReadAllText(ormFile);
                using (dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(xml)) {
                    // 判断代表是否存在
                    var xmlTable = doc["table"];
                    if (xmlTable == null) {
                        xmlTable = new dpz3.Xml.XmlNode("table");
                        xmlTable.Attr["md5"] = "";
                        doc.Nodes.Add(xmlTable);
                    }

                    // 校验文件缓存文件是否存在变更
                    var tabMD5 = xmlTable.Attr["md5"];
                    var md5 = dpz3.File.BinaryFile.GetMD5(workFolderTable);

                    if (tabMD5 != md5) {
                        // 生成页面文件
                        xmlTable.Attr["name"] = table.Attr["name"];
                        xmlTable.Attr["title"] = table.Attr["title"];
                        xmlTable.Attr["md5"] = md5;

                        // 界面定义节点
                        var xmlInterface = xmlTable["interface"];
                        if (xmlInterface == null) xmlInterface = xmlTable.AddNode("interface");

                        // 定义vue绑定
                        var xmlInterfaceVue = xmlInterface["vue"];
                        if (xmlInterfaceVue == null) {
                            xmlInterfaceVue = xmlInterface.AddNode("vue");

                            // list配置
                            var xmlInterfaceVueList = xmlInterfaceVue.AddNode("list");
                            xmlInterfaceVueList.IsSingle = true;
                            xmlInterfaceVueList.Attr["for"] = "(row,index) in list";
                            xmlInterfaceVueList.Attr["item"] = "row";
                            xmlInterfaceVueList.Attr["key"] = "row.ID";

                            // order配置
                            var xmlInterfaceVueOrder = xmlInterfaceVue.AddNode("order");
                            xmlInterfaceVueOrder.IsSingle = true;
                            xmlInterfaceVueOrder.Attr["field"] = "orderField";
                            xmlInterfaceVueOrder.Attr["sort"] = "orderSort";
                            xmlInterfaceVueOrder.Attr["click"] = "onOrder";
                        }

                        // 定义行操作
                        var xmlInterfaceRow = xmlInterface["row"];
                        if (xmlInterfaceRow == null) {
                            xmlInterfaceRow = xmlInterface.AddNode("row");
                            xmlInterfaceRow.IsSingle = true;
                        }

                        // 定义行操作
                        var xmlInterfaceCell = xmlInterface["cell"];
                        if (xmlInterfaceCell == null) {
                            xmlInterfaceCell = xmlInterface.AddNode("cell");
                            xmlInterfaceCell.IsSingle = true;
                        }

                        // 定义图像控件操作
                        var xmlInterfaceImg = xmlInterface["img"];
                        if (xmlInterfaceImg == null) {
                            xmlInterfaceImg = xmlInterface.AddNode("img");
                        }

                        // 定义图像控件容器
                        var xmlInterfaceImgBox = xmlInterfaceImg["box"];
                        if (xmlInterfaceImgBox == null) {
                            xmlInterfaceImgBox = xmlInterfaceImg.AddNode("box");
                            xmlInterfaceImgBox.IsSingle = true;
                            xmlInterfaceImgBox.Attr["tag-name"] = "s";
                        }

                        // 定义勾选框控件操作
                        var xmlInterfaceCheck = xmlInterface["check"];
                        if (xmlInterfaceCheck == null) {
                            xmlInterfaceCheck = xmlInterface.AddNode("check");
                            xmlInterfaceCheck.IsSingle = true;
                            xmlInterfaceCheck.Attr["tag-name"] = "div";
                            xmlInterfaceCheck.Attr["class"] = "x-check";
                        }

                        // 定义排序控件操作
                        var xmlInterfaceOrder = xmlInterface["order"];
                        if (xmlInterfaceOrder == null) {
                            xmlInterfaceOrder = xmlInterface.AddNode("order");
                            xmlInterfaceOrder.Attr["tag-name"] = "div";
                            xmlInterfaceOrder.Attr["class"] = "x-order";

                            var xmlInterfaceOrderAsc = xmlInterfaceOrder.AddNode("asc");
                            xmlInterfaceOrderAsc.IsSingle = true;
                            xmlInterfaceOrderAsc.Attr["tag-name"] = "div";
                            xmlInterfaceOrderAsc.Attr["class"] = "x-order-asc";

                            var xmlInterfaceOrderAscAct = xmlInterfaceOrder.AddNode("asc-act");
                            xmlInterfaceOrderAscAct.IsSingle = true;
                            xmlInterfaceOrderAscAct.Attr["tag-name"] = "div";
                            xmlInterfaceOrderAscAct.Attr["class"] = "x-order-asc-act";

                            var xmlInterfaceOrderDesc = xmlInterfaceOrder.AddNode("desc");
                            xmlInterfaceOrderDesc.IsSingle = true;
                            xmlInterfaceOrderDesc.Attr["tag-name"] = "div";
                            xmlInterfaceOrderDesc.Attr["class"] = "x-order-desc";

                            var xmlInterfaceOrderDescAct = xmlInterfaceOrder.AddNode("desc-act");
                            xmlInterfaceOrderDescAct.IsSingle = true;
                            xmlInterfaceOrderDescAct.Attr["tag-name"] = "div";
                            xmlInterfaceOrderDescAct.Attr["class"] = "x-order-desc-act";
                        }

                        // 字段定义节点
                        var xmlFields = xmlTable["fields"];
                        if (xmlFields == null) xmlFields = xmlTable.AddNode("fields");

                        foreach (var fld in table.GetNodesByTagName("field", false)) {
                            // 判断标题中是否存在字段定义，不存在则创建
                            string fldName = fld.Attr["name"];
                            if (xmlFields.GetNodeByAttr("name", fldName) == null) {
                                var xmlField = xmlFields.AddNode("field");
                                xmlField.IsSingle = true;
                                xmlField.Attr["name"] = fldName;
                                xmlField.Attr["title"] = fld.Attr["title"];
                                string fldDataType = fld["data"].Attr["type"].ToLower();
                                switch (fldDataType) {
                                    case "text":
                                        // 文本类型的大块内容 默认不显示
                                        xmlField.Attr["type"] = "none";
                                        break;
                                    case "numeric":
                                    case "int":
                                        xmlField.Attr["type"] = "text";
                                        xmlField.Attr["width"] = "60px";
                                        xmlField.Attr["order"] = "false";
                                        break;
                                    default:
                                        xmlField.Attr["type"] = "text";
                                        xmlField.Attr["width"] = "100px";
                                        xmlField.Attr["order"] = "false";
                                        break;
                                }
                                xmlField.InnerXml = fld.Attr["title"];
                            }
                        }
                        // 保存
                        Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }


                }

                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);

                // 重新开启工具
                this.toolStripButtonBuildTable.Enabled = true;
                Say("完成");
            }
        }

        // 构建表格页
        private void BuildFormPage(string formType) {
            if (node == null) return;

            // 关闭状态栏
            this.toolStripButtonBuildTable.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = workPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var workBuildGroup = workCfg["Build"];
                var rootFolder = $"{workPath}{workBuildGroup["PagePath"]}";
                var mainFolder = $"{rootFolder}\\wwwroot\\{tableName}\\{formType.ToLower()}";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";

                // 自动建立模板路径
                var templateFolder = $"{localTemplateFolder}\\{formType}";
                if (!System.IO.Directory.Exists(templateFolder)) {
                    System.IO.Directory.CreateDirectory(templateFolder);
                }

                // 自动建立路径
                if (!System.IO.Directory.Exists(srcFolder)) {
                    System.IO.Directory.CreateDirectory(srcFolder);
                }

                // 判断是否存在配置文件，不存在则创建
                if (!System.IO.File.Exists(configFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\config.json", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, table));
                }

                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, table));
                }

                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, table));

                }

                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, table));

                }

                // 从输出路径加载xml设定,达到兼容目的
                Say("正在加载现有ORM文件...");
                string xml = dpz3.File.UTF8File.ReadAllText(ormFile);
                using (dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(xml)) {
                    // 判断代表是否存在
                    var xmlTable = doc["table"];
                    if (xmlTable == null) {
                        xmlTable = new dpz3.Xml.XmlNode("table");
                        xmlTable.Attr["md5"] = "";
                        doc.Nodes.Add(xmlTable);
                    }

                    // 校验文件缓存文件是否存在变更
                    var tabMD5 = xmlTable.Attr["md5"];
                    var md5 = dpz3.File.BinaryFile.GetMD5(workFolderTable);

                    if (tabMD5 != md5) {
                        // 生成页面文件
                        xmlTable.Attr["name"] = table.Attr["name"];
                        xmlTable.Attr["title"] = table.Attr["title"];
                        xmlTable.Attr["md5"] = md5;

                        // 界面定义节点
                        var xmlInterface = xmlTable["interface"];
                        if (xmlInterface == null) xmlInterface = xmlTable.AddNode("interface");

                        // 定义line呈现
                        var xmlInterfaceLine = xmlInterface["line"];
                        if (xmlInterfaceLine == null) {
                            xmlInterfaceLine = xmlInterface.AddNode("line");
                            xmlInterfaceLine.IsSingle = true;
                            xmlInterfaceLine.Attr["tag-name"] = "div";
                            xmlInterfaceLine.Attr["class"] = "x-line";
                        }

                        // 定义title呈现
                        var xmlInterfaceTitle = xmlInterface["title"];
                        if (xmlInterfaceTitle == null) {
                            xmlInterfaceTitle = xmlInterface.AddNode("title");
                            xmlInterfaceTitle.IsSingle = true;
                            xmlInterfaceTitle.Attr["tag-name"] = "div";
                            xmlInterfaceTitle.Attr["class"] = "x-title";
                        }

                        // 定义content呈现
                        var xmlInterfaceContent = xmlInterface["content"];
                        if (xmlInterfaceContent == null) {
                            xmlInterfaceContent = xmlInterface.AddNode("content");
                            xmlInterfaceContent.IsSingle = true;
                            xmlInterfaceContent.Attr["tag-name"] = "div";
                            xmlInterfaceContent.Attr["class"] = "x-content";
                        }

                        // 定义勾选框控件呈现
                        var xmlInterfaceCheck = xmlInterface["check"];
                        if (xmlInterfaceCheck == null) {
                            xmlInterfaceCheck = xmlInterface.AddNode("check");
                            xmlInterfaceCheck.IsSingle = true;
                            xmlInterfaceCheck.Attr["tag-name"] = "div";
                            xmlInterfaceCheck.Attr["class"] = "x-check";
                        }

                        // 定义图片上传控件呈现
                        var xmlInterfaceImg = xmlInterface["img"];
                        if (xmlInterfaceImg == null) {
                            xmlInterfaceImg = xmlInterface.AddNode("img");
                            xmlInterfaceImg.IsSingle = true;
                            xmlInterfaceImg.Attr["tag-name"] = "div";
                            xmlInterfaceImg.Attr["class"] = "x-img";
                        }

                        // 字段定义节点
                        var xmlFields = xmlTable["fields"];
                        if (xmlFields == null) xmlFields = xmlTable.AddNode("fields");

                        foreach (var fld in table.GetNodesByTagName("field", false)) {
                            // 判断标题中是否存在字段定义，不存在则创建
                            string fldName = fld.Attr["name"];
                            if (xmlFields.GetNodeByAttr("name", fldName) == null) {
                                var xmlField = xmlFields.AddNode("field");
                                xmlField.IsSingle = true;
                                xmlField.Attr["name"] = fldName;
                                xmlField.Attr["title"] = fld.Attr["title"];
                                xmlField.Attr["model"] = $"form.{fldName}";
                                string fldDataType = fld["data"].Attr["type"].ToLower();
                                switch (fldDataType) {
                                    case "text":
                                        // 文本类型的大块内容 默认不显示
                                        xmlField.Attr["type"] = "textarea";
                                        xmlField.Attr["width"] = "100%";
                                        xmlField.Attr["height"] = "60px";
                                        break;
                                    case "numeric":
                                    case "int":
                                        xmlField.Attr["type"] = "input";
                                        xmlField.Attr["width"] = "60px";
                                        break;
                                    default:
                                        xmlField.Attr["type"] = "input";
                                        xmlField.Attr["width"] = "100px";
                                        break;
                                }
                                xmlField.InnerXml = fld.Attr["title"];
                            }
                        }
                        // 保存
                        Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }


                }

                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);

                // 重新开启工具
                this.toolStripButtonBuildTable.Enabled = true;
                Say("完成");
            }
        }

        // 构建表格页
        private void BuildViewPage() {
            if (node == null) return;

            // 关闭状态栏
            this.toolStripButtonBuildTable.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = workPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];

                // 拼接输出路径
                var workBuildGroup = workCfg["Build"];
                var rootFolder = $"{workPath}{workBuildGroup["PagePath"]}";
                var mainFolder = $"{rootFolder}\\wwwroot\\{tableName}\\view";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";

                // 自动建立模板路径
                var templateFolder = $"{localTemplateFolder}\\View";
                if (!System.IO.Directory.Exists(templateFolder)) {
                    System.IO.Directory.CreateDirectory(templateFolder);
                }

                // 自动建立路径
                if (!System.IO.Directory.Exists(srcFolder)) {
                    System.IO.Directory.CreateDirectory(srcFolder);
                }

                // 判断是否存在配置文件，不存在则创建
                if (!System.IO.File.Exists(configFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\config.json", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, table));
                }

                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, table));
                }

                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, table));

                }

                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, table));

                }

                // 从输出路径加载xml设定,达到兼容目的
                Say("正在加载现有ORM文件...");
                string xml = dpz3.File.UTF8File.ReadAllText(ormFile);
                using (dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(xml)) {
                    // 判断代表是否存在
                    var xmlTable = doc["table"];
                    if (xmlTable == null) {
                        xmlTable = new dpz3.Xml.XmlNode("table");
                        xmlTable.Attr["md5"] = "";
                        doc.Nodes.Add(xmlTable);
                    }

                    // 校验文件缓存文件是否存在变更
                    var tabMD5 = xmlTable.Attr["md5"];
                    var md5 = dpz3.File.BinaryFile.GetMD5(workFolderTable);

                    if (tabMD5 != md5) {
                        // 生成页面文件
                        xmlTable.Attr["name"] = table.Attr["name"];
                        xmlTable.Attr["title"] = table.Attr["title"];
                        xmlTable.Attr["md5"] = md5;

                        // 界面定义节点
                        var xmlInterface = xmlTable["interface"];
                        if (xmlInterface == null) xmlInterface = xmlTable.AddNode("interface");

                        // 定义line呈现
                        var xmlInterfaceLine = xmlInterface["line"];
                        if (xmlInterfaceLine == null) {
                            xmlInterfaceLine = xmlInterface.AddNode("line");
                            xmlInterfaceLine.IsSingle = true;
                            xmlInterfaceLine.Attr["tag-name"] = "div";
                        }

                        // 定义title呈现
                        var xmlInterfaceTitle = xmlInterface["title"];
                        if (xmlInterfaceTitle == null) {
                            xmlInterfaceTitle = xmlInterface.AddNode("title");
                            xmlInterfaceTitle.IsSingle = true;
                            xmlInterfaceTitle.Attr["tag-name"] = "i";
                        }

                        // 定义content呈现
                        var xmlInterfaceContent = xmlInterface["content"];
                        if (xmlInterfaceContent == null) {
                            xmlInterfaceContent = xmlInterface.AddNode("content");
                            xmlInterfaceContent.IsSingle = true;
                            xmlInterfaceContent.Attr["tag-name"] = "s";
                        }

                        // 字段定义节点
                        var xmlFields = xmlTable["fields"];
                        if (xmlFields == null) xmlFields = xmlTable.AddNode("fields");

                        foreach (var fld in table.GetNodesByTagName("field", false)) {
                            // 判断标题中是否存在字段定义，不存在则创建
                            string fldName = fld.Attr["name"];
                            if (xmlFields.GetNodeByAttr("name", fldName) == null) {
                                var xmlField = xmlFields.AddNode("field");
                                xmlField.IsSingle = true;
                                xmlField.Attr["name"] = fldName;
                                xmlField.Attr["title"] = fld.Attr["title"];
                                xmlField.Attr["type"] = "text";
                                xmlField.InnerXml = fld.Attr["title"];
                            }
                        }
                        // 保存
                        Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }


                }

                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);

                // 重新开启工具
                this.toolStripButtonBuildTable.Enabled = true;
                Say("完成");
            }
        }

        // 构建表格类
        private void BuildClass() {
            if (node == null) return;

            // 关闭状态栏
            this.toolStripButtonBuildClass.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = workPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];

                // 拼接输出路径
                var workBuildGroup = workCfg["Build"];
                var outputFolder = $"{workPath}\\controller\\Xorm";
                var outputFile = $"{outputFolder}\\{tableName}.cs";
                var ns = $"Xorm";

                // 自动建立路径
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }

                // 加载模板文件
                string configTemplate = dpz3.File.UTF8File.ReadAllText($"{localTemplateFolder}\\Class.cs.tlp", true);

                // 输出处理后的模板内容
                dpz3.File.UTF8File.WriteAllText(outputFile, Template.FromString(configTemplate, table));

                // 重新开启工具
                this.toolStripButtonBuildClass.Enabled = true;
                Say("完成");
            }
        }

        // 构建表格Api控制器
        private void BuildControllerClass() {
            if (node == null) return;

            // 关闭状态栏
            this.toolStripButtonBuildClass.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = workPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];

                // 拼接输出路径
                var workBuildGroup = workCfg["Build"];
                var outputFolder = $"{workPath}\\controller\\Api";
                var outputFile = $"{outputFolder}\\{tableName}Controller.cs";
                var ns = $"control";

                // 自动建立路径
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }

                // 判断控制器文件是否已经存在
                if (!System.IO.File.Exists(outputFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{localTemplateFolder}\\Controller.cs.tlp", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(outputFile, Template.FromString(configTemplate, table));

                }

                // 重新开启工具
                this.toolStripButtonBuildClass.Enabled = true;
                Say("完成");
            }
        }

        #endregion

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

        // 加载树状目录
        private void LoadTree() {

            Say("正在进行版本校验...");
            var group = workCfg["XOrm"];
            string workFolder = workPath + Path_WorkFolder;
            string workFolderXorm = workPath + "\\xorm";
            string workFolderModular = workPath + "\\modular.json";
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
            string proName = null;
            string proVersion = null;
            string proDescription = null;
            using (var json = dpz3.Json.Parser.ParseJson(content)) {
                proName = json.String("name").Value;
                proVersion = json.String("version").Value;
                proDescription = json.String("description").Value;
            }

            // 设置根节点
            Say("载入列表...");
            TreeNode nodeRoot = new TreeNode();
            nodeRoot.Name = "Root";
            nodeRoot.Text = $"{proDescription} [{proName}:{proVersion}]";
            nodeRoot.ImageIndex = 0;
            this.treeView1.Nodes.Add(nodeRoot);

            // 加载平台
            // LoadPlatforms(nodeRoot, setting);
            LoadTables(nodeRoot, workFolderXorm);

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
                    node.Nodes.Add(nodeTable);
                }

            }
        }

        /// <summary>
        /// 设置工作目录
        /// </summary>
        /// <param name="path"></param>
        public void SetWorkPath(string path) {
            workPath = path;
            if (workPath.EndsWith("\\")) workPath = workPath.Substring(0, workPath.Length - 1);
            this.Text = $"{Application.ProductName} Ver:{Application.ProductVersion} - [{workPath}]";

            // 创建工作文件夹
            if (!System.IO.Directory.Exists(workPath + Path_WorkFolder)) {
                System.IO.Directory.CreateDirectory(workPath + Path_WorkFolder);
            }

            workCfg = new dpz3.File.ConfFile(workPath + Path_WorkFolder + Path_WorkConfig);
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
            if (!workPath.IsNoneOrNull()) {
                if (MessageBox.Show("是否退出当前的工作目录，并打开一个新的?", "工作目录切换提醒", MessageBoxButtons.YesNo) == DialogResult.No) {
                    return;
                }
            }

            // 加载历史记录
            string lastPath = null;
            using (dpz3.File.ConfFile conf = new dpz3.File.ConfFile(localPath + Path_Config)) {
                var group = conf["History"];
                lastPath = group["WorkPath"];


                if (lastPath.IsNoneOrNull()) lastPath = localPath;

                using (FolderBrowserDialog f = new FolderBrowserDialog()) {
                    f.SelectedPath = lastPath;
                    if (f.ShowDialog() == DialogResult.OK) {

                        // 存储历史记录
                        group["WorkPath"] = f.SelectedPath;
                        conf.Save();

                        // 设置工作目录
                        this.SetWorkPath(f.SelectedPath);
                        //workPath = f.SelectedPath;

                        // 设置资源管理器
                        //frmSetting.LoadSetting(workPath);
                    } else {
                        return;
                    }
                }

            }

        }

        #endregion


        private void Form1_Load(object sender, EventArgs e) {

            // 获取本地路径
            this.localPath = Application.StartupPath;
            if (!this.localPath.EndsWith("\\")) this.localPath += "\\";

            // 模板目录
            localTemplateFolder = localPath + Path_TemplateFolder;
            if (!System.IO.Directory.Exists(localTemplateFolder)) System.IO.Directory.CreateDirectory(localTemplateFolder);

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

            this.Visible = true;
            Application.DoEvents();

            if (!workPathInit.IsNoneOrNull()) {
                this.SetWorkPath(workPathInit);
            }
        }

        private void 选项OToolStripMenuItem_Click(object sender, EventArgs e) {

            if (workPath.IsNoneOrNull()) {
                MessageBox.Show("尚未指定工作目录");
                return;
            }

            using (FrmSetting f = new FrmSetting()) {
                f.SetConfigGroup(workCfg["XOrm"]);
                f.SetConfigIitem("Url");
                f.SetConfigIitem("SettingPath");
                f.SetConfigIitem("VersionPath");
                f.ShowDialog();
                workCfg.Save();
            }
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e) {
            this.OpenFolder();
        }

        private void 打开OToolStripButton_Click(object sender, EventArgs e) {
            this.OpenFolder();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void ToolStripButtonBuildTable_Click(object sender, EventArgs e) {
            this.BuildListPage("List");
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
            if (workPath.IsNoneOrNull()) {
                MessageBox.Show("尚未指定工作目录");
                return;
            }

            using (FrmSetting f = new FrmSetting()) {
                f.SetConfigGroup(workCfg["Build"]);
                f.SetConfigIitem("PagePath");
                f.SetConfigIitem("ClassPath");
                f.SetConfigIitem("ControllerPath");
                f.SetConfigIitem("NameSpace");
                f.ShowDialog();
                workCfg.Save();
            }
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e) {
            this.BuildListPage("List");
        }

        private void ToolStripMenuItemBuildClass_Click(object sender, EventArgs e) {
            this.BuildClass();
            this.BuildControllerClass();
        }

        private void ToolStripButtonBuildClass_Click(object sender, EventArgs e) {
            this.BuildClass();
            this.BuildControllerClass();
        }

        private void ToolStripButtonBuildAdd_Click(object sender, EventArgs e) {
            this.BuildFormPage("Add");
        }

        private void ToolStripMenuItemBuildAdd_Click(object sender, EventArgs e) {
            this.BuildFormPage("Add");
        }

        private void ToolStripMenuItemBuildEdit_Click(object sender, EventArgs e) {
            this.BuildFormPage("Edit");
        }

        private void ToolStripButtonBuildEdit_Click(object sender, EventArgs e) {
            this.BuildFormPage("Edit");
        }

        private void ToolStripButtonBuildView_Click(object sender, EventArgs e) {
            this.BuildViewPage();
        }

        private void ToolStripMenuItemBuildView_Click(object sender, EventArgs e) {
            this.BuildViewPage();
        }

        private void toolStripButtonBuildSelector_Click(object sender, EventArgs e) {
            this.BuildListPage("Selector");
        }

        private void toolStripMenuItemBuildSelector_Click(object sender, EventArgs e) {
            this.BuildListPage("Selector");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {

        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e) {

        }
    }
}
