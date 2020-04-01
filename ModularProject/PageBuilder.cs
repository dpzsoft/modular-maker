using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using dpz3;

namespace ModularProject {

    /// <summary>
    /// 页面构建器
    /// </summary>
    internal class PageBuilder {

        /// <summary>
        /// 构建表格页
        /// </summary>
        /// <param name="ListType"></param>
        internal static void BuildListPage(Form1 form, TreeNode node, string ListType) {
            if (node == null) return;

            // 关闭状态栏
            form.toolStripButtonBuildTable.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = it.WorkPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];

                // 拼接输出路径
                var rootFolder = $"{it.WorkPath}\\wwwroot";
                var mainFolder = $"{rootFolder}\\{tableName.ToLower()}\\{ListType.ToLower()}";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";

                // 自动建立模板路径
                var templateFolder = $"{it.LocalTemplateFolder}\\{ListType}";
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
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, form.Package, table));
                }

                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, form.Package, table));
                }

                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, form.Package, table));

                }

                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, form.Package, table));

                }

                // 从输出路径加载xml设定,达到兼容目的
                form.Say("正在加载现有ORM文件...");
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
                        form.Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }


                }

                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);

                // 重新开启工具
                form.toolStripButtonBuildTable.Enabled = true;
                form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格页
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        /// <param name="formType"></param>
        internal static void BuildFormPage(Form1 form, TreeNode node, string formType) {
            if (node == null) return;

            // 关闭状态栏
            form.toolStripButtonBuildTable.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = it.WorkPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var rootFolder = $"{it.WorkPath}\\wwwroot";
                var mainFolder = $"{rootFolder}\\{tableName.ToLower()}\\{formType.ToLower()}";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";

                // 自动建立模板路径
                var templateFolder = $"{it.LocalTemplateFolder}\\{formType}";
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
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, form.Package, table));
                }

                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, form.Package, table));
                }

                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, form.Package, table));

                }

                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, form.Package, table));

                }

                // 从输出路径加载xml设定,达到兼容目的
                form.Say("正在加载现有ORM文件...");
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
                        form.Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }


                }

                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);

                // 重新开启工具
                form.toolStripButtonBuildTable.Enabled = true;
                form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格页
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        internal static void BuildViewPage(Form1 form, TreeNode node) {
            if (node == null) return;

            // 关闭状态栏
            form.toolStripButtonBuildTable.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = it.WorkPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];

                // 拼接输出路径
                var rootFolder = $"{it.WorkPath}\\wwwroot";
                var mainFolder = $"{rootFolder}\\{tableName.ToLower()}\\view";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";

                // 自动建立模板路径
                var templateFolder = $"{it.LocalTemplateFolder}\\View";
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
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, form.Package, table));
                }

                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, form.Package, table));
                }

                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, form.Package, table));

                }

                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {

                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);

                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, form.Package, table));

                }

                // 从输出路径加载xml设定,达到兼容目的
                form.Say("正在加载现有ORM文件...");
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
                        form.Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }


                }

                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);

                // 重新开启工具
                form.toolStripButtonBuildTable.Enabled = true;
                form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格类
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        internal static void BuildClass(Form1 form, TreeNode node) {
            if (node == null) return;

            // 关闭状态栏
            form.toolStripButtonBuildClass.Enabled = false;

            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = it.WorkPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var outputFolder = $"{it.WorkPath}\\controller\\Xorm";
                var outputFile = $"{outputFolder}\\{tableName}.cs";
                var ns = $"Xorm";
                // 自动建立路径
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }
                // 加载模板文件
                string configTemplate = dpz3.File.UTF8File.ReadAllText($"{it.LocalTemplateFolder}\\Class.cs.tlp", true);
                // 输出处理后的模板内容
                dpz3.File.UTF8File.WriteAllText(outputFile, Template.FromString(configTemplate, form.Package, table));
                // 重新开启工具
                form.toolStripButtonBuildClass.Enabled = true;
                form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格Api控制器
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        internal static void BuildControllerClass(Form1 form, TreeNode node) {
            if (node == null) return;
            // 关闭状态栏
            form.toolStripButtonBuildClass.Enabled = false;
            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = it.WorkPath + "\\xorm";
            string tableName = node.Name;
            string workFolderTable = $"{workFolderXorm}\\{tableName}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var outputFolder = $"{it.WorkPath}\\controller\\Api";
                var outputFile = $"{outputFolder}\\{tableName}Controller.cs";
                var ns = $"control";
                // 自动建立路径
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }
                // 判断控制器文件是否已经存在
                if (!System.IO.File.Exists(outputFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{it.LocalTemplateFolder}\\Controller.cs.tlp", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(outputFile, Template.FromString(configTemplate, form.Package, table));
                }
                // 重新开启工具
                form.toolStripButtonBuildClass.Enabled = true;
                form.Say("完成");
            }
        }

        /// <summary>
        /// 构建页面
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="workFolder"></param>
        /// <param name="mainFile"></param>
        /// <param name="outputFile"></param>
        internal static void BuildMainPage(Form1 form, TreeNode node, string rootFolder, string workFolder, string mainFile, string outputFile) {
            if (node == null) return;
            // 加载页面内容
            form.Say($"读取文件 {mainFile} ...");
            string xml = dpz3.File.UTF8File.ReadAllText(mainFile);
            // Console.WriteLine(xml);
            using (dpz3.Html.HtmlDocument doc = dpz3.Html.Parser.GetDocument(xml)) {
                // Console.WriteLine(doc.InnerHTML);
                // 查找其中的include节点
                form.Say($"检测include元素 ...");
                var incXmls = doc.GetElementsByTagName("include");
                for (int i = 0; i < incXmls.Count; i++) {
                    var incXml = incXmls[i];
                    string incSrc = incXml.Attr["src"].Replace("/", "\\");
                    string incFilePath;
                    if (incSrc.StartsWith("\\")) {
                        incFilePath = rootFolder + incSrc;
                    } else {
                        incFilePath = workFolder + "\\" + incSrc;
                    }
                    // 读取页面内容并生效到当前文档
                    form.Say($"读取引入文件 {incFilePath} ...");
                    string incXmlText = dpz3.File.UTF8File.ReadAllText(incFilePath);
                    dpz3.Html.HtmlDocument incDoc = dpz3.Html.Parser.GetDocument(incXmlText);
                    if (incXml.Parent == null) {
                        int idx = -1;
                        // 查找当前节点的位置
                        for (int j = 0; j < doc.Nodes.Count; j++) {
                            if (doc.Nodes[j] == incXml) {
                                idx = j;
                            }
                        }
                        // 删除当前节点，并将引入文件内容插入到列表中
                        doc.Nodes.RemoveAt(idx);
                        for (int j = 0; j < incDoc.Nodes.Count; j++) {
                            doc.Nodes.Insert(idx + j, incDoc.Nodes[j]);
                        }
                    } else {
                        int idx = -1;
                        var parent = incXml.Parent;
                        // 查找当前节点的位置
                        for (int j = 0; j < parent.Nodes.Count; j++) {
                            if (parent.Nodes[j] == incXml) {
                                idx = j;
                            }
                        }
                        // 删除当前节点，并将引入文件内容插入到列表中
                        parent.Nodes.RemoveAt(idx);
                        for (int j = 0; j < incDoc.Nodes.Count; j++) {
                            parent.Nodes.Insert(idx + j, incDoc.Nodes[j]);
                        }
                    }
                }
                // 查找其中的xorm节点
                form.Say($"检测xorm元素 ...");
                var xorms = doc.GetElementsByTagName("xorm");
                for (int i = 0; i < xorms.Count; i++) {
                    var xorm = xorms[i];
                    string xormSrc = xorm.Attr["src"].Replace("/", "\\");
                    string xormType = xorm.Attr["type"]?.ToLower();

                    string xormFilePath;
                    if (xormSrc.StartsWith("\\")) {
                        xormFilePath = rootFolder + xormSrc;
                    } else {
                        xormFilePath = workFolder + "\\" + xormSrc;
                    }

                    // 建立输出X表单
                    dpz3.Html.HtmlDocument docx = new dpz3.Html.HtmlDocument();

                    // 读取页面内容并生效到当前文档
                    form.Say($"读取XOrm定义文件 {xormFilePath} ...");
                    string xormText = dpz3.File.UTF8File.ReadAllText(xormFilePath);
                    using (dpz3.Xml.XmlDocument xormDoc = dpz3.Xml.Parser.GetDocument(xormText)) {

                        switch (xormType) {
                            case "list":
                                #region [=====生成列表HTML=====]
                                // 界面定义节点
                                var xmlTable = xormDoc["table"];
                                // 界面定义节点
                                var xmlInterface = xmlTable["interface"];

                                // vue绑定节点
                                var xmlInterfaceVue = xmlInterface["vue"];
                                var xmlInterfaceVueList = xmlInterfaceVue?["list"];
                                string vueListItem = xmlInterfaceVueList?.Attr["item"];
                                var xmlInterfaceVueOrder = xmlInterfaceVue?["order"];

                                // 行操作节点
                                var xmlInterfaceRow = xmlInterface["row"];
                                // 单元格操作节点
                                var xmlInterfaceCell = xmlInterface["cell"];
                                // 排序控件操作节点
                                var xmlInterfaceOrder = xmlInterface["order"];
                                var xmlInterfaceOrderAsc = xmlInterfaceOrder?["asc"];
                                var xmlInterfaceOrderAscAct = xmlInterfaceOrder?["asc-act"];
                                var xmlInterfaceOrderDesc = xmlInterfaceOrder?["desc"];
                                var xmlInterfaceOrderDescAct = xmlInterfaceOrder?["desc-act"];
                                // 图像控件操作节点
                                var xmlInterfaceImg = xmlInterface["img"];
                                // 图像控件操作容器节点
                                var xmlInterfaceImgBox = xmlInterfaceImg?["box"];
                                // 图像控件操作节点
                                var xmlInterfaceCheck = xmlInterface["check"];
                                // 字段定义节点
                                var xmlFields = xmlTable["fields"];

                                // 建立表格节点
                                var table = docx.CreateElement("table");
                                docx.Nodes.Add(table);

                                // 建立表头行
                                var head = docx.CreateElement("tr");
                                table.Children.Add(head);

                                // 建立数据行
                                var data = docx.CreateElement("tr");
                                table.Children.Add(data);
                                data.Attr["v-for"] = xmlInterfaceVueList?.Attr["for"];
                                data.Attr["v-bind:key"] = xmlInterfaceVueList?.Attr["key"];

                                // 填充row定义内容
                                foreach (var key in xmlInterfaceRow.Attr.Keys) {
                                    data.Attr[key] = xmlInterfaceRow.Attr[key];
                                }

                                // 遍历字段
                                foreach (var xmlField in xmlFields.GetNodesByTagName("field")) {

                                    string fldName = xmlField.Attr["name"];
                                    string fldTitle = xmlField.Attr["title"];
                                    string fldType = xmlField.Attr["type"]?.ToLower();
                                    string fldOrder = xmlField.Attr["order"]?.ToLower();

                                    // 字段类型不为none时，才有输出
                                    if (fldType != "none") {

                                        string fldWidth = xmlField.Attr["width"]?.ToLower();
                                        if (fldWidth.IsNoneOrNull()) fldWidth = "100px";

                                        var th = docx.CreateElement("th");
                                        head.Children.Add(th);
                                        th.Attr["style"] = $"width:{fldWidth};min-width:{fldWidth};max-width:{fldWidth};";
                                        th.InnerHTML = fldTitle;

                                        // 判断是否需要支持排序
                                        if (fldOrder == "true") {
                                            // 添加对排序的支持
                                            var thOrder = docx.CreateElement(xmlInterfaceOrder.Attr["tag-name"]);
                                            th.Children.Add(thOrder);
                                            thOrder.Attr["v-on:click"] = $"{xmlInterfaceVueOrder.Attr["click"]}($event,'{fldName}')";

                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceOrder.Attr.Keys) {
                                                if (key != "tag-name")
                                                    thOrder.Attr[key] = xmlInterfaceOrder.Attr[key];
                                            }

                                            // 正序控件
                                            var thOrderAsc = docx.CreateElement(xmlInterfaceOrderAsc.Attr["tag-name"]);
                                            thOrder.Children.Add(thOrderAsc);
                                            thOrderAsc.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}!=='{fldName}'||{xmlInterfaceVueOrder.Attr["sort"]}!=='asc'";

                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceOrderAsc.Attr.Keys) {
                                                if (key != "tag-name")
                                                    thOrderAsc.Attr[key] = xmlInterfaceOrderAsc.Attr[key];
                                            }

                                            // 正序控件激活
                                            var thOrderAscAct = docx.CreateElement(xmlInterfaceOrderAscAct.Attr["tag-name"]);
                                            thOrder.Children.Add(thOrderAscAct);
                                            thOrderAscAct.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}==='{fldName}'&&{xmlInterfaceVueOrder.Attr["sort"]}==='asc'";

                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceOrderAscAct.Attr.Keys) {
                                                if (key != "tag-name")
                                                    thOrderAscAct.Attr[key] = xmlInterfaceOrderAscAct.Attr[key];
                                            }

                                            // 反序控件
                                            var thOrderDesc = docx.CreateElement(xmlInterfaceOrderDesc.Attr["tag-name"]);
                                            thOrder.Children.Add(thOrderDesc);
                                            thOrderDesc.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}!=='{fldName}'||{xmlInterfaceVueOrder.Attr["sort"]}!=='desc'";

                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceOrderDesc.Attr.Keys) {
                                                if (key != "tag-name")
                                                    thOrderDesc.Attr[key] = xmlInterfaceOrderDesc.Attr[key];
                                            }

                                            // 反序控件激活
                                            var thOrderDescAct = docx.CreateElement(xmlInterfaceOrderDescAct.Attr["tag-name"]);
                                            thOrder.Children.Add(thOrderDescAct);
                                            thOrderDescAct.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}==='{fldName}'&&{xmlInterfaceVueOrder.Attr["sort"]}==='desc'";

                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceOrderDescAct.Attr.Keys) {
                                                if (key != "tag-name")
                                                    thOrderDescAct.Attr[key] = xmlInterfaceOrderDescAct.Attr[key];
                                            }
                                        }

                                        var td = docx.CreateElement("td");
                                        data.Children.Add(td);
                                        // 填充cell定义内容
                                        foreach (var key in xmlInterfaceCell.Attr.Keys) {
                                            td.Attr[key] = xmlInterfaceCell.Attr[key];
                                        }

                                        switch (fldType) {
                                            // 呈现为纯文本
                                            case "text":
                                                td.Attr["v-html"] = $"{vueListItem}.{fldName}";
                                                break;
                                            // 呈现为索引
                                            case "index":
                                                td.InnerHTML = $"{{{{index+1+(rows.page-1)*rows.pageSize}}}}";
                                                break;
                                            // 呈现为图像
                                            case "img":
                                                #region [=====呈现为图像=====]
                                                var imgBox = docx.CreateElement(xmlInterfaceImgBox.Attr["tag-name"]);
                                                td.Children.Add(imgBox);

                                                // 填充图像容器定义内容
                                                foreach (var key in xmlInterfaceImgBox.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        imgBox.Attr[key] = xmlInterfaceImgBox.Attr[key];
                                                }

                                                var img = docx.CreateElement("img");
                                                imgBox.Children.Add(img);
                                                img.IsSingle = true;
                                                img.Attr["v-if"] = $"row.{fldName}!==''";
                                                img.Attr["v-bind:src"] = $"row.{fldName}";

                                                // 填充图像定义内容
                                                foreach (var key in xmlInterfaceImg.Attr.Keys) {
                                                    img.Attr[key] = xmlInterfaceImg.Attr[key];
                                                }

                                                break;
                                            #endregion
                                            // 呈现为勾选框
                                            case "check":
                                                #region [=====呈现为勾选框=====]
                                                var checkBox = docx.CreateElement(xmlInterfaceCheck.Attr["tag-name"]);
                                                td.Children.Add(checkBox);

                                                // 填充图像容器定义内容
                                                foreach (var key in xmlInterfaceCheck.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        checkBox.Attr[key] = xmlInterfaceCheck.Attr[key];
                                                }

                                                // 定义超链接
                                                var checkLink = docx.CreateElement("a");
                                                checkBox.Children.Add(checkLink);

                                                checkLink.Attr["href"] = "javascript:;";
                                                checkLink.Attr["v-on:click"] = $"onCheckChangeSave($event,'{fldName}',row)";

                                                // 定义勾选状态标签
                                                var checkSpan = docx.CreateElement("span");
                                                checkLink.Children.Add(checkSpan);
                                                checkSpan.Attr["v-if"] = $"row.{fldName}===1||row.{fldName}==='1'";
                                                checkSpan.InnerHTML = "√";

                                                break;
                                            #endregion
                                            // 呈现为状态勾
                                            case "tick":
                                                #region [=====呈现为勾选框=====]

                                                // 定义勾选状态标签
                                                checkSpan = docx.CreateElement("span");
                                                td.Children.Add(checkSpan);
                                                checkSpan.Attr["v-if"] = $"row.{fldName}===1||row.{fldName}==='1'";
                                                checkSpan.InnerHTML = "√";

                                                break;
                                            #endregion
                                            // 呈现为输入框
                                            case "input":
                                                #region [=====呈现为勾选框=====]

                                                // 定义勾选状态标签
                                                var input = docx.CreateElement("input");
                                                input.IsSingle = true;
                                                td.Children.Add(input);
                                                input.Attr["name"] = $"{fldName}";
                                                input.Attr["type"] = "text";
                                                input.Attr["v-model"] = $"row.{fldName}";

                                                break;
                                            #endregion
                                            // 呈现为自定义HTML
                                            case "html":
                                                // 读取字段html定义文件
                                                string fldHtmPath = workFolder + "\\field." + fldName + ".htm";
                                                form.Say($"读取字段HTML定义文件 {fldHtmPath} ...");
                                                string htmlContent = dpz3.File.UTF8File.ReadAllText(fldHtmPath, true);
                                                td.InnerHTML = htmlContent;
                                                break;
                                            default:
                                                throw new Exception("不支持的字段呈现类型");
                                        }
                                    }
                                }

                                break;
                            #endregion  
                            case "form":
                                #region [=====生成表单HTML=====]
                                // 界面定义节点
                                xmlTable = xormDoc["table"];
                                // 界面定义节点
                                xmlInterface = xmlTable["interface"];
                                // vue绑定节点
                                var xmlInterfaceLine = xmlInterface["line"];
                                // 行操作节点
                                var xmlInterfaceTitle = xmlInterface["title"];
                                // 单元格操作节点
                                var xmlInterfaceContent = xmlInterface["content"];
                                // 勾选框操作节点
                                xmlInterfaceCheck = xmlInterface["check"];
                                // 图像控件操作节点
                                xmlInterfaceImg = xmlInterface["img"];
                                // 图像控件操作操作条
                                var xmlInterfaceImgBar = xmlInterfaceImg?["bar"];
                                // 字段定义节点
                                xmlFields = xmlTable["fields"];

                                // 遍历字段
                                foreach (var xmlField in xmlFields.GetNodesByTagName("field")) {

                                    string fldName = xmlField.Attr["name"];
                                    string fldTitle = xmlField.Attr["title"];

                                    string fldType = xmlField.Attr["type"]?.ToLower();

                                    // 字段类型不为none时，才有输出
                                    if (fldType != "none") {

                                        string fldModel = xmlField.Attr["model"];

                                        // 处理行
                                        var eleLine = docx.CreateElement(xmlInterfaceLine.Attr["tag-name"]);
                                        docx.Nodes.Add(eleLine);

                                        // 填充line定义内容
                                        foreach (var key in xmlInterfaceLine.Attr.Keys) {
                                            if (key != "tag-name")
                                                eleLine.Attr[key] = xmlInterfaceLine.Attr[key];
                                        }

                                        // 填充专用的样式信息
                                        if (eleLine.Attr["class"].IsNoneOrNull()) {
                                            eleLine.Attr["class"] = $"xform-line-{fldName}";
                                        } else {
                                            eleLine.Attr["class"] += $" xform-line-{fldName}";
                                        }

                                        // 处理标题
                                        var eleTitle = docx.CreateElement(xmlInterfaceTitle.Attr["tag-name"]);
                                        eleLine.Children.Add(eleTitle);

                                        // 填充line定义内容
                                        foreach (var key in xmlInterfaceTitle.Attr.Keys) {
                                            if (key != "tag-name")
                                                eleTitle.Attr[key] = xmlInterfaceTitle.Attr[key];
                                        }

                                        // 填充专用的样式信息
                                        if (eleTitle.Attr["class"].IsNoneOrNull()) {
                                            eleTitle.Attr["class"] = $"xform-title-{fldName}";
                                        } else {
                                            eleTitle.Attr["class"] += $" xform-title-{fldName}";
                                        }

                                        eleTitle.InnerHTML = fldTitle;

                                        // 处理内容
                                        var eleContent = docx.CreateElement(xmlInterfaceContent.Attr["tag-name"]);
                                        eleLine.Children.Add(eleContent);

                                        // 填充line定义内容
                                        foreach (var key in xmlInterfaceContent.Attr.Keys) {
                                            if (key != "tag-name")
                                                eleContent.Attr[key] = xmlInterfaceContent.Attr[key];
                                        }

                                        // 填充专用的样式信息
                                        if (eleContent.Attr["class"].IsNoneOrNull()) {
                                            eleContent.Attr["class"] = $"xform-content-{fldName}";
                                        } else {
                                            eleContent.Attr["class"] += $" xform-content-{fldName}";
                                        }

                                        switch (fldType) {
                                            // 不呈现
                                            case "none": break;
                                            // 呈现为输入框
                                            case "text":
                                                #region [=====呈现为文本=====]

                                                // 处理内容
                                                var eleText = docx.CreateElement("span");
                                                eleContent.Children.Add(eleText);
                                                eleText.Attr["id"] = $"lab{fldName}";
                                                eleText.Attr["name"] = fldName;
                                                eleText.InnerHTML = $"{{{{form.{fldName}}}}}";

                                                break;
                                            #endregion 
                                            // 呈现为输入框
                                            case "input":
                                                #region [=====呈现为输入框=====]
                                                // 读取宽度
                                                string fldWidth = xmlField.Attr["width"];

                                                // 处理内容
                                                var eleInput = docx.CreateElement("input");
                                                eleContent.Children.Add(eleInput);
                                                eleInput.IsSingle = true;
                                                eleInput.Attr["id"] = $"txt{fldName}";
                                                eleInput.Attr["name"] = fldName;
                                                eleInput.Attr["type"] = "text";
                                                eleInput.Attr["style"] = $"width:{fldWidth};";
                                                eleInput.Attr["placeholder"] = $"输入{fldTitle}";
                                                eleInput.Attr["v-model"] = fldModel;

                                                // 只读处理
                                                if (!xmlField.Attr["readonly"].IsNoneOrNull()) {
                                                    var onlyRead = xmlField.Attr["readonly"].ToLower();
                                                    if (onlyRead == "yes" || onlyRead == "true" || onlyRead == "readonly") {
                                                        eleInput.Attr["readonly"] = "readonly";
                                                    }
                                                }

                                                break;
                                            #endregion 
                                            // 呈现为输入框
                                            case "textarea":
                                                #region [=====呈现为多行文本框=====]
                                                // 读取宽度
                                                fldWidth = xmlField.Attr["width"];

                                                // 读取高度
                                                string fldHeight = xmlField.Attr["height"];

                                                // 处理内容
                                                eleInput = docx.CreateElement("textarea");
                                                eleContent.Children.Add(eleInput);
                                                eleInput.Attr["id"] = $"txt{fldName}";
                                                eleInput.Attr["name"] = fldName;
                                                eleInput.Attr["style"] = $"width:{fldWidth};height:{fldHeight};";
                                                eleInput.Attr["placeholder"] = $"输入{fldTitle}";
                                                eleInput.Attr["v-model"] = fldModel;

                                                break;
                                            #endregion
                                            // 呈现为勾选框
                                            case "check":
                                                #region [=====呈现为勾选框=====]
                                                var checkBox = docx.CreateElement(xmlInterfaceCheck.Attr["tag-name"]);
                                                eleContent.Children.Add(checkBox);

                                                // 填充图像容器定义内容
                                                foreach (var key in xmlInterfaceCheck.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        checkBox.Attr[key] = xmlInterfaceCheck.Attr[key];
                                                }

                                                // 定义超链接
                                                var checkLink = docx.CreateElement("a");
                                                checkBox.Children.Add(checkLink);

                                                checkLink.Attr["href"] = "javascript:;";
                                                checkLink.Attr["v-on:click"] = $"onChecked($event,'{fldName}')";

                                                // 定义勾选状态标签
                                                var checkSpan = docx.CreateElement("span");
                                                checkLink.Children.Add(checkSpan);
                                                checkSpan.Attr["v-if"] = $"form.{fldName}===1||form.{fldName}==='1'";
                                                checkSpan.InnerHTML = "√";

                                                break;
                                            #endregion
                                            // 呈现为日期框
                                            case "day":
                                                #region [=====呈现为勾选框=====]
                                                var dateBox = docx.CreateElement("div");
                                                eleContent.Children.Add(dateBox);

                                                // 填充图像容器定义内容
                                                foreach (var key in xmlInterfaceCheck.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        checkBox.Attr[key] = xmlInterfaceCheck.Attr[key];
                                                }

                                                // 定义超链接
                                                var checkLink = docx.CreateElement("a");
                                                checkBox.Children.Add(checkLink);

                                                checkLink.Attr["href"] = "javascript:;";
                                                checkLink.Attr["v-on:click"] = $"onDayPicked($event,'{fldName}')";

                                                // 定义勾选状态标签
                                                var checkSpan = docx.CreateElement("span");
                                                checkLink.Children.Add(checkSpan);
                                                checkSpan.Attr["v-if"] = $"form.{fldName}===1||form.{fldName}==='1'";
                                                checkSpan.InnerHTML = "√";

                                                break;
                                            #endregion
                                            // 呈现为图片上传框
                                            case "img":
                                                #region [=====呈现为勾选框=====]
                                                var imgBox = docx.CreateElement(xmlInterfaceImg.Attr["tag-name"]);
                                                eleContent.Children.Add(imgBox);

                                                // 填充图像容器定义内容
                                                foreach (var key in xmlInterfaceImg.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        imgBox.Attr[key] = xmlInterfaceImg.Attr[key];
                                                }

                                                // 定义图片回显
                                                var imgEle = docx.CreateElement("img");
                                                imgBox.Children.Add(imgEle);
                                                imgEle.IsSingle = true;
                                                imgEle.Attr["v-bind:src"] = $"form.{fldName}";
                                                imgEle.Attr["v-on:click"] = $"onUpload($event,'{fldName}')";

                                                // 清除链接
                                                var imgClearLink = docx.CreateElement("a");
                                                imgBox.Children.Add(imgClearLink);
                                                imgClearLink.Attr["href"] = "javascript:;";
                                                imgClearLink.Attr["v-if"] = $"form.{fldName}!==img.{fldName}";
                                                imgClearLink.Attr["v-on:click"] = $"onUploadClear($event,'{fldName}')";
                                                imgClearLink.InnerHTML = "X";

                                                break;
                                            #endregion
                                            // 呈现为选择框
                                            case "select":
                                                #region [=====呈现为选择框=====]
                                                var select = xmlField["select"];
                                                var selectOptions = select.GetNodesByTagName("option");

                                                var eleSelect = docx.CreateElement("select");
                                                eleContent.Children.Add(eleSelect);

                                                eleSelect.Attr["name"] = fldName;
                                                eleSelect.Attr["v-model"] = fldModel;

                                                // 填充图像容器定义内容
                                                foreach (var key in select.Attr.Keys) {
                                                    eleSelect.Attr[key] = select.Attr[key];
                                                }

                                                foreach (var option in selectOptions) {
                                                    var eleOption = docx.CreateElement("option");
                                                    eleSelect.Children.Add(eleOption);

                                                    // 填充图像容器定义内容
                                                    foreach (var key in option.Attr.Keys) {
                                                        eleOption.Attr[key] = option.Attr[key];
                                                    }

                                                    eleOption.InnerHTML = option.InnerText;
                                                }

                                                break;
                                            #endregion
                                            case "html":
                                                #region [=====呈现为Html加载=====]
                                                // 读取字段html定义文件
                                                string fldHtmPath = workFolder + "\\field." + fldName + ".htm";
                                                form.Say($"读取字段HTML定义文件 {fldHtmPath} ...");
                                                string htmlContent = dpz3.File.UTF8File.ReadAllText(fldHtmPath, true);
                                                eleContent.InnerHTML = htmlContent;
                                                break;
                                            #endregion
                                            default:
                                                throw new Exception($"不支持的字段呈现类型\"{fldType}\"。");
                                        }

                                    }

                                }

                                break;
                            #endregion
                            default:
                                throw new Exception("尚未支持的XORM定义类型");
                        }

                        #region [=====生效到输出文档中=====]
                        // 生效到输出文档中
                        if (xorm.Parent == null) {
                            int idx = -1;
                            // 查找当前节点的位置
                            for (int j = 0; j < doc.Nodes.Count; j++) {
                                if (doc.Nodes[j] == xorm) {
                                    idx = j;
                                }
                            }
                            // 删除当前节点，并将引入文件内容插入到列表中
                            doc.Nodes.RemoveAt(idx);
                            for (int j = 0; j < docx.Nodes.Count; j++) {
                                doc.Nodes.Insert(idx + j, docx.Nodes[j]);
                            }
                        } else {
                            int idx = -1;
                            // 查找当前节点的位置
                            var parent = xorm.Parent;
                            for (int j = 0; j < parent.Nodes.Count; j++) {
                                if (parent.Nodes[j] == xorm) {
                                    idx = j;
                                }
                            }
                            // 删除当前节点，并将引入文件内容插入到列表中
                            parent.Nodes.RemoveAt(idx);
                            for (int j = 0; j < docx.Nodes.Count; j++) {
                                parent.Nodes.Insert(idx + j, docx.Nodes[j]);
                            }
                        }
                        #endregion

                    }
                }

                // 保存
                form.Say($"正在输出文件 {outputFile} ...");
                dpz3.File.UTF8File.WriteAllText(outputFile, doc.InnerHTML);
            }
        }

    }
}
