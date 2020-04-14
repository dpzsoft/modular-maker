using System;
using System.Collections.Generic;
using System.Text;
using dpz3;

namespace controller {
    internal static class PageBuilder {

        /// <summary>
        /// 构建表格页
        /// </summary>
        /// <param name="ListType"></param>
        internal static void BuildListPage(dpz3.Json.JsonUnit package, string folderTemplete, string path, string name, string ListType) {
            // 获取表定义
            string workFolderXorm = $"{path}/xorm";
            string workFolderTable = $"{workFolderXorm}\\{name}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var rootFolder = $"{path}/wwwroot";
                var mainFolder = $"{rootFolder}/{name.ToLower()}/{ListType.ToLower()}";
                var srcFolder = $"{mainFolder}/src";
                var pageFile = $"{mainFolder}/page.html";
                var cssFile = $"{mainFolder}/page.css";
                var jsFile = $"{mainFolder}/page.js";
                var configFile = $"{mainFolder}/config.json";
                var mainFile = $"{srcFolder}/main.htm";
                var ormFile = $"{srcFolder}/orm.xml";
                // 自动建立模板路径
                var templateFolder = $"{folderTemplete}/{ListType}";
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
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, package, table));
                }
                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, package, table));
                }
                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, package, table));
                }
                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, package, table));
                }
                // 从输出路径加载xml设定,达到兼容目的
                //form.Say("正在加载现有ORM文件...");
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
                        xmlTable.Attr["statistics"] = "false";
                        // 界面定义节点
                        var xmlInterface = xmlTable["interface"];
                        if (xmlInterface == null) xmlInterface = xmlTable.AddNode("interface");
                        // 字段定义节点
                        var xmlFields = xmlTable["fields"];
                        if (xmlFields == null) xmlFields = xmlTable.AddNode("fields");
                        var fields = table.GetNodesByTagName("field", false);
                        // 处理自动序号列
                        if (xmlFields.GetNodeByAttr("name", "AutoIndex") == null) {
                            var xmlField = xmlFields.AddNode("field");
                            xmlField.IsSingle = true;
                            xmlField.Attr["name"] = "AutoIndex";
                            xmlField.Attr["title"] = "序号";
                            xmlField.Attr["type"] = "index";
                            xmlField.Attr["width"] = "60px";
                            xmlField.Attr["order"] = "false";
                        }
                        // 处理所有字段
                        foreach (var fld in fields) {
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
                        // 处理操作列
                        if (xmlFields.GetNodeByAttr("name", "RowOperate") == null) {
                            var xmlField = xmlFields.AddNode("field");
                            xmlField.IsSingle = true;
                            xmlField.Attr["name"] = "RowOperate";
                            xmlField.Attr["title"] = "操作";
                            xmlField.Attr["type"] = "html";
                            xmlField.Attr["width"] = "100px";
                            xmlField.Attr["order"] = "false";
                        }
                        // 保存
                        //form.Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }
                }

                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);

                // 重新开启工具
                //form.toolStripButtonBuildTable.Enabled = true;
                //form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格页
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        /// <param name="formType"></param>
        internal static void BuildFormPage(dpz3.Json.JsonUnit package, string folderTemplete, string path, string name, string formType) {
            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = $"{path}/xorm";
            string workFolderTable = $"{workFolderXorm}\\{name}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var rootFolder = $"{path}\\wwwroot";
                var mainFolder = $"{rootFolder}\\{name.ToLower()}\\{formType.ToLower()}";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";
                // 自动建立模板路径
                var templateFolder = $"{folderTemplete}\\{formType}";
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
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, package, table));
                }
                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, package, table));
                }
                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, package, table));
                }
                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, package, table));
                }
                // 从输出路径加载xml设定,达到兼容目的
                // form.Say("正在加载现有ORM文件...");
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
                                xmlField.Attr["must"] = "false";
                                xmlField.Attr["readonly"] = "false";
                                xmlField.Attr["model"] = $"form.{fldName}";
                                string fldDataType = fld["data"].Attr["type"].ToLower();
                                int fldDataSize = fld["data"].Attr["size"].ToInteger();
                                switch (fldDataType) {
                                    case "text":
                                        // 文本类型的大块内容 默认不显示
                                        xmlField.Attr["type"] = "textarea";
                                        xmlField.Attr["width"] = "100%";
                                        xmlField.Attr["height"] = "80px";
                                        break;
                                    case "numeric":
                                    case "int":
                                        xmlField.Attr["type"] = "input";
                                        xmlField.Attr["width"] = "80px";
                                        break;
                                    case "string":
                                        xmlField.Attr["type"] = "input";
                                        if (fldDataSize < 50) {
                                            xmlField.Attr["width"] = "100px";
                                        } else if (fldDataSize > 150) {
                                            xmlField.Attr["width"] = "300px";
                                        } else {
                                            xmlField.Attr["width"] = $"{fldDataSize * 2}px";
                                        }
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
                        // form.Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }
                }
                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);
                // 重新开启工具
                // form.toolStripButtonBuildTable.Enabled = true;
                // form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格页
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        internal static void BuildViewPage(dpz3.Json.JsonUnit package, string folderTemplete, string path, string name) {
            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = $"{path}/xorm";
            string workFolderTable = $"{workFolderXorm}\\{name}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var rootFolder = $"{path}\\wwwroot";
                var mainFolder = $"{rootFolder}\\{name.ToLower()}\\view";
                var srcFolder = $"{mainFolder}\\src";
                var pageFile = $"{mainFolder}\\page.html";
                var cssFile = $"{mainFolder}\\page.css";
                var jsFile = $"{mainFolder}\\page.js";
                var configFile = $"{mainFolder}\\config.json";
                var mainFile = $"{srcFolder}\\main.htm";
                var ormFile = $"{srcFolder}\\orm.xml";
                // 自动建立模板路径
                var templateFolder = $"{folderTemplete}\\View";
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
                    dpz3.File.UTF8File.WriteAllText(configFile, Template.FromString(configTemplate, package, table));
                }
                // 判断主文件是否存在,不存在则创建
                if (!System.IO.File.Exists(mainFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\main.htm", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(mainFile, Template.FromString(configTemplate, package, table));
                }
                // 判断样式文件是否存在,不存在则创建
                if (!System.IO.File.Exists(cssFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.css", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(cssFile, Template.FromString(configTemplate, package, table));
                }
                // 判断脚本文件是否存在,不存在则创建
                if (!System.IO.File.Exists(jsFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{templateFolder}\\page.js", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(jsFile, Template.FromString(configTemplate, package, table));
                }
                // 从输出路径加载xml设定,达到兼容目的
                // form.Say("正在加载现有ORM文件...");
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
                        // form.Say("正在保存文件...");
                        dpz3.File.UTF8File.WriteAllText(ormFile, doc.InnerXml);
                    }
                }
                // 构建页面
                //BuildPage(rootFolder, srcFolder, mainFile, pageFile);
                // 重新开启工具
                // form.toolStripButtonBuildTable.Enabled = true;
                // form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格类
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        internal static void BuildClass(dpz3.Json.JsonUnit package, string folderTemplete, string path, string name) {
            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = $"{path}/xorm";
            string workFolderTable = $"{workFolderXorm}\\{name}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var outputFolder = $"{path}\\controller\\Xorm";
                var outputFile = $"{outputFolder}\\{name}.cs";
                var ns = $"Xorm";
                // 自动建立路径
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }
                // 加载模板文件
                string configTemplate = dpz3.File.UTF8File.ReadAllText($"{folderTemplete}\\Class.cs.tlp", true);
                // 输出处理后的模板内容
                dpz3.File.UTF8File.WriteAllText(outputFile, Template.FromString(configTemplate, package, table));
                // 重新开启工具
                // form.toolStripButtonBuildClass.Enabled = true;
                // form.Say("完成");
            }
        }

        /// <summary>
        /// 构建表格Api控制器
        /// </summary>
        /// <param name="form"></param>
        /// <param name="node"></param>
        internal static void BuildControllerClass(dpz3.Json.JsonUnit package, string folderTemplete, string path, string name) {
            // 获取表定义
            //var platform = setting[node.Parent.Name];
            string workFolderXorm = $"{path}/xorm";
            string workFolderTable = $"{workFolderXorm}\\{name}.xml";
            dpz3.Xml.XmlNode table;
            // 读取文件内容
            string content = dpz3.File.UTF8File.ReadAllText(workFolderTable);
            using (var docTable = dpz3.Xml.Parser.GetDocument(content)) {
                table = docTable["table"];
                // 拼接输出路径
                var outputFolder = $"{path}\\controller\\Api";
                var outputFile = $"{outputFolder}\\{name}Controller.cs";
                var ns = $"control";
                // 自动建立路径
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }
                // 判断控制器文件是否已经存在
                if (!System.IO.File.Exists(outputFile)) {
                    // 加载模板文件
                    string configTemplate = dpz3.File.UTF8File.ReadAllText($"{folderTemplete}\\Controller.cs.tlp", true);
                    // 输出处理后的模板内容
                    dpz3.File.UTF8File.WriteAllText(outputFile, Template.FromString(configTemplate, package, table));
                }
                // 重新开启工具
                // form.toolStripButtonBuildClass.Enabled = true;
                // form.Say("完成");
            }
        }

        /// <summary>
        /// 构建页面
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="workFolder"></param>
        /// <param name="mainFile"></param>
        /// <param name="outputFile"></param>
        internal static void BuildMainPage(string folderTemplete, string rootFolder, string workFolder, string mainFile, string outputFile) {
            // 读取控件定义
            string pathControls = $"{folderTemplete}\\controls.xml";
            string xmlControls = dpz3.File.UTF8File.ReadAllText(pathControls);
            using (var docControls = dpz3.Xml.Parser.GetDocument(xmlControls)) {
                bool needSave = false;
                if (docControls["controls"] == null) {
                    var node = new dpz3.Xml.XmlNode("controls");
                    docControls.Nodes.Add(node);
                    needSave = true;
                }
                // 读取所有的控件配置
                var controls = docControls["controls"];
                // 读取列表组
                var listGroup = controls.GetNodeByAttr("name", "list", false);
                if (listGroup == null) {
                    listGroup = controls.AddNode("group");
                    listGroup.Attr["name"] = "list";
                }
                // 读取表单组
                var formGroup = controls.GetNodeByAttr("name", "form", false);
                if (formGroup == null) {
                    formGroup = controls.AddNode("group");
                    formGroup.Attr["name"] = "form";
                }
                // 读取需要输出文件
                string xml = dpz3.File.UTF8File.ReadAllText(mainFile);
                // Console.WriteLine(xml);
                using (dpz3.Html.HtmlDocument doc = dpz3.Html.Parser.GetDocument(xml)) {
                    // Console.WriteLine(doc.InnerHTML);
                    // 查找其中的include节点
                    // form.Say($"检测include元素 ...");
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
                        // form.Say($"读取引入文件 {incFilePath} ...");
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
                    // form.Say($"检测xorm元素 ...");
                    var xorms = doc.GetElementsByTagName("xorm");
                    for (int i = 0; i < xorms.Count; i++) {
                        // 获取元素信息
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
                        // form.Say($"读取XOrm定义文件 {xormFilePath} ...");
                        string xormText = dpz3.File.UTF8File.ReadAllText(xormFilePath);
                        using (dpz3.Xml.XmlDocument xormDoc = dpz3.Xml.Parser.GetDocument(xormText)) {
                            switch (xormType) {
                                case "list":
                                    #region [=====生成列表HTML=====]
                                    // 界面定义节点
                                    var xmlTable = xormDoc["table"];
                                    bool hasStatistics = xmlTable.Attr["statistics"] == "true";
                                    // 界面定义节点
                                    var xmlInterface = xmlTable["interface"];
                                    if (xmlInterface == null) xmlInterface = xmlTable.AddNode("interface");
                                    #region [=====早期的控件定义模式=====]
                                    //// vue绑定节点
                                    //var xmlInterfaceVue = xmlInterface["vue"];
                                    //var xmlInterfaceVueList = xmlInterfaceVue?["list"];
                                    //string vueListItem = xmlInterfaceVueList?.Attr["item"];
                                    //var xmlInterfaceVueOrder = xmlInterfaceVue?["order"];
                                    //// 行操作节点
                                    //var xmlInterfaceRow = xmlInterface["row"];
                                    //// 单元格操作节点
                                    //var xmlInterfaceCell = xmlInterface["cell"];
                                    //// 排序控件操作节点
                                    //var xmlInterfaceOrder = xmlInterface["order"];
                                    //var xmlInterfaceOrderAsc = xmlInterfaceOrder?["asc"];
                                    //var xmlInterfaceOrderAscAct = xmlInterfaceOrder?["asc-act"];
                                    //var xmlInterfaceOrderDesc = xmlInterfaceOrder?["desc"];
                                    //var xmlInterfaceOrderDescAct = xmlInterfaceOrder?["desc-act"];
                                    //// 图像控件操作节点
                                    //var xmlInterfaceImg = xmlInterface["img"];
                                    //// 图像控件操作容器节点
                                    //var xmlInterfaceImgBox = xmlInterfaceImg?["box"];
                                    //// 图像控件操作节点
                                    //var xmlInterfaceCheck = xmlInterface["check"];
                                    #endregion
                                    // 字段定义节点
                                    var xmlFields = xmlTable["fields"];
                                    // 建立表格节点
                                    var table = docx.CreateElement("table");
                                    docx.Nodes.Add(table);
                                    #region [=====处理table组件相关内容=====]
                                    // 读取全局配置
                                    var listTable = listGroup["table"];
                                    if (listTable == null) {
                                        // 初始化全局配置
                                        listTable = listGroup.AddNode("table");
                                        listTable.IsSingle = true;
                                        needSave = true;
                                    }
                                    // 生效全局配置
                                    foreach (var key in listTable.Attr.Keys) {
                                        table.Attr[key] = listTable.Attr[key];
                                    }
                                    // 读取自定义配置
                                    var interfaceTable = xmlInterface["table"];
                                    if (interfaceTable == null) interfaceTable = xmlInterface.AddNode("interface");
                                    // 生效自定义配置
                                    foreach (var key in interfaceTable.Attr.Keys) {
                                        table.Attr[key] = interfaceTable.Attr[key];
                                    }
                                    #endregion
                                    // 建立表头行
                                    var head = docx.CreateElement("tr");
                                    table.Children.Add(head);
                                    // 建立数据行
                                    var data = docx.CreateElement("tr");
                                    table.Children.Add(data);
                                    // 建立统计行
                                    var trStatistics = docx.CreateElement("tr");
                                    if (hasStatistics) table.Children.Add(trStatistics);
                                    #region [=====处理vue组件相关内容=====]
                                    // 读取全局配置
                                    var listVue = listGroup["vue"];
                                    if (listVue == null) {
                                        listVue = listGroup.AddNode("vue");
                                        needSave = true;
                                    }
                                    // 读取vue配置中的列表配置
                                    var listVueList = listVue["list"];
                                    if (listVueList == null) {
                                        listVueList = listVue.AddNode("list");
                                        listVueList.IsSingle = true;
                                        listVueList.Attr["for"] = "(row,index) in list";
                                        listVueList.Attr["item"] = "row";
                                        listVueList.Attr["key"] = "row.ID";
                                        needSave = true;
                                    }
                                    data.Attr["v-for"] = listVueList.Attr["for"];
                                    data.Attr["v-bind:key"] = listVueList.Attr["key"];
                                    string vueListItem = listVueList?.Attr["item"];
                                    // 读取自定义配置
                                    var interfaceVue = xmlInterface["vue"];
                                    if (interfaceVue != null) {
                                        var interfaceVueList = interfaceVue["list"];
                                        if (interfaceVueList != null) {
                                            if (interfaceVueList.Attr.ContainsKey("for")) data.Attr["v-for"] = interfaceVueList.Attr["for"];
                                            if (interfaceVueList.Attr.ContainsKey("key")) data.Attr["v-bind:key"] = interfaceVueList.Attr["key"];
                                            if (interfaceVueList.Attr.ContainsKey("for")) vueListItem = interfaceVueList.Attr["item"];
                                        }
                                    }
                                    #endregion
                                    #region [=====处理row组件相关内容=====]
                                    // 读取row配置
                                    var listRow = listGroup["row"];
                                    if (listRow == null) {
                                        listRow = listGroup.AddNode("row");
                                        listRow.IsSingle = true;
                                        needSave = true;
                                    }
                                    // 填充row定义内容
                                    foreach (var key in listRow.Attr.Keys) {
                                        data.Attr[key] = listRow.Attr[key];
                                    }
                                    // 读取自定义配置
                                    var interfaceRow = xmlInterface["row"];
                                    if (interfaceRow == null) interfaceRow = xmlInterface.AddNode("row");
                                    // 生效自定义配置
                                    foreach (var key in interfaceRow.Attr.Keys) {
                                        data.Attr[key] = interfaceRow.Attr[key];
                                    }
                                    #endregion
                                    // 遍历字段
                                    foreach (var xmlField in xmlFields.GetNodesByTagName("field")) {
                                        string fldName = xmlField.Attr["name"];
                                        string fldTitle = xmlField.Attr["title"];
                                        string fldType = xmlField.Attr["type"]?.ToLower();
                                        string fldOrder = xmlField.Attr["order"]?.ToLower();
                                        // 字段类型不为none时，才有输出
                                        if (fldType != "none") {
                                            string fldWidth = xmlField.Attr["width"]?.ToLower();
                                            #region [=====处理标题行=====]
                                            var th = docx.CreateElement("th");
                                            head.Children.Add(th);
                                            if (!fldWidth.IsNoneOrNull()) {
                                                th.Attr["style"] = $"width:{fldWidth};min-width:{fldWidth};max-width:{fldWidth};";
                                            }
                                            th.InnerHTML = fldTitle;
                                            // 判断是否需要支持排序
                                            if (fldOrder == "true") {
                                                #region [=====处理vue/order组件相关内容=====]
                                                // vue中的order配置
                                                var xmlInterfaceVueOrder = listVue["order"];
                                                if (xmlInterfaceVueOrder == null) {
                                                    xmlInterfaceVueOrder = listVue.AddNode("order");
                                                    xmlInterfaceVueOrder.IsSingle = true;
                                                    xmlInterfaceVueOrder.Attr["field"] = "orderField";
                                                    xmlInterfaceVueOrder.Attr["sort"] = "orderSort";
                                                    xmlInterfaceVueOrder.Attr["click"] = "onOrder";
                                                    needSave = true;
                                                }
                                                // 读取自定义配置
                                                var interfaceVueOrder = interfaceVue["order"];
                                                if (interfaceVueOrder == null) interfaceVueOrder = interfaceVue.AddNode("order");
                                                #endregion
                                                #region [=====处理排序组件相关内容=====]
                                                // 定义排序控件操作
                                                var xmlInterfaceOrder = listGroup["order"];
                                                if (xmlInterfaceOrder == null) {
                                                    xmlInterfaceOrder = listGroup.AddNode("order");
                                                    xmlInterfaceOrder.Attr["tag-name"] = "div";
                                                    xmlInterfaceOrder.Attr["class"] = "x-order";
                                                    needSave = true;
                                                }
                                                // 读取自定义配置
                                                var interfaceOrder = xmlInterface["order"];
                                                if (interfaceOrder == null) interfaceOrder = xmlInterface.AddNode("order");
                                                // 添加对排序的支持
                                                string thOrderTagName = xmlInterfaceOrder.Attr["tag-name"];
                                                if (interfaceOrder.Attr.ContainsKey("tag-name")) thOrderTagName = interfaceOrder.Attr["tag-name"];
                                                var thOrder = docx.CreateElement(thOrderTagName);
                                                th.Children.Add(thOrder);
                                                thOrder.Attr["v-on:click"] = $"{xmlInterfaceVueOrder.Attr["click"]}($event,'{fldName}')";
                                                // 填充自定义内容
                                                if (interfaceVueOrder.Attr.ContainsKey("click")) thOrder.Attr["v-on:click"] = $"{interfaceVueOrder.Attr["click"]}($event,'{fldName}')";
                                                // 填充line定义内容
                                                foreach (var key in xmlInterfaceOrder.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrder.Attr[key] = xmlInterfaceOrder.Attr[key];
                                                }
                                                // 填充自定义内容
                                                foreach (var key in interfaceOrder.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrder.Attr[key] = interfaceOrder.Attr[key];
                                                }
                                                #endregion
                                                #region [=====处理正序组件相关内容=====]
                                                // asc
                                                var xmlInterfaceOrderAsc = xmlInterfaceOrder["asc"];
                                                if (xmlInterfaceOrderAsc == null) {
                                                    xmlInterfaceOrderAsc = xmlInterfaceOrder.AddNode("asc");
                                                    xmlInterfaceOrderAsc.IsSingle = true;
                                                    xmlInterfaceOrderAsc.Attr["tag-name"] = "div";
                                                    xmlInterfaceOrderAsc.Attr["class"] = "x-order-asc";
                                                    needSave = true;
                                                }
                                                // 读取自定义配置
                                                var interfaceOrderAsc = interfaceOrder["asc"];
                                                if (interfaceOrderAsc == null) interfaceOrderAsc = interfaceOrder.AddNode("asc");
                                                // 正序控件
                                                string thOrderAscTagName = xmlInterfaceOrderAsc.Attr["tag-name"];
                                                if (interfaceOrderAsc.Attr.ContainsKey("tag-name")) thOrderAscTagName = interfaceOrderAsc.Attr["tag-name"];
                                                var thOrderAsc = docx.CreateElement(thOrderAscTagName);
                                                thOrder.Children.Add(thOrderAsc);
                                                thOrderAsc.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}!=='{fldName}'||{xmlInterfaceVueOrder.Attr["sort"]}!=='asc'";
                                                // 填充自定义内容
                                                if (interfaceVueOrder.Attr.ContainsKey("field") && interfaceVueOrder.Attr.ContainsKey("sort")) thOrderAsc.Attr["v-if"] = $"{interfaceVueOrder.Attr["field"]}!=='{fldName}'||{interfaceVueOrder.Attr["sort"]}!=='asc'";
                                                // 填充line定义内容
                                                foreach (var key in xmlInterfaceOrderAsc.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderAsc.Attr[key] = xmlInterfaceOrderAsc.Attr[key];
                                                }
                                                // 填充自定义内容
                                                foreach (var key in interfaceOrderAsc.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderAsc.Attr[key] = interfaceOrderAsc.Attr[key];
                                                }
                                                #endregion
                                                #region [=====处理正序激活组件相关内容=====]
                                                // asc-act
                                                var xmlInterfaceOrderAscAct = xmlInterfaceOrder["asc-act"];
                                                if (xmlInterfaceOrderAscAct == null) {
                                                    xmlInterfaceOrderAscAct = xmlInterfaceOrder.AddNode("asc-act");
                                                    xmlInterfaceOrderAscAct.IsSingle = true;
                                                    xmlInterfaceOrderAscAct.Attr["tag-name"] = "div";
                                                    xmlInterfaceOrderAscAct.Attr["class"] = "x-order-asc-act";
                                                    needSave = true;
                                                }
                                                var interfaceOrderAscAct = interfaceOrder["asc-act"];
                                                if (interfaceOrderAscAct == null) interfaceOrderAscAct = interfaceOrder.AddNode("asc-act");
                                                // 正序控件激活
                                                string thOrderAscActTagName = xmlInterfaceOrderAscAct.Attr["tag-name"];
                                                if (interfaceOrderAscAct.Attr.ContainsKey("tag-name")) thOrderAscActTagName = interfaceOrderAscAct.Attr["tag-name"];
                                                var thOrderAscAct = docx.CreateElement(thOrderAscActTagName);
                                                thOrder.Children.Add(thOrderAscAct);
                                                thOrderAscAct.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}==='{fldName}'&&{xmlInterfaceVueOrder.Attr["sort"]}==='asc'";
                                                // 填充自定义内容
                                                if (interfaceVueOrder.Attr.ContainsKey("field") && interfaceVueOrder.Attr.ContainsKey("sort")) thOrderAscAct.Attr["v-if"] = $"{interfaceVueOrder.Attr["field"]}==='{fldName}'||{interfaceVueOrder.Attr["sort"]}==='asc'";
                                                // 填充line定义内容
                                                foreach (var key in xmlInterfaceOrderAscAct.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderAscAct.Attr[key] = xmlInterfaceOrderAscAct.Attr[key];
                                                }
                                                // 填充自定义内容
                                                foreach (var key in interfaceOrderAscAct.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderAscAct.Attr[key] = interfaceOrderAscAct.Attr[key];
                                                }
                                                #endregion
                                                #region [=====处理反序组件相关内容=====]
                                                // desc
                                                var xmlInterfaceOrderDesc = xmlInterfaceOrder["desc"];
                                                if (xmlInterfaceOrderDesc == null) {
                                                    xmlInterfaceOrderDesc = xmlInterfaceOrder.AddNode("desc");
                                                    xmlInterfaceOrderDesc.IsSingle = true;
                                                    xmlInterfaceOrderDesc.Attr["tag-name"] = "div";
                                                    xmlInterfaceOrderDesc.Attr["class"] = "x-order-desc";
                                                    needSave = true;
                                                }
                                                var interfaceOrderDesc = interfaceOrder["desc"];
                                                if (interfaceOrderDesc == null) interfaceOrderDesc = interfaceOrder.AddNode("desc");
                                                // 反序控件
                                                string thOrderDescTagName = xmlInterfaceOrderDesc.Attr["tag-name"];
                                                if (interfaceOrderDesc.Attr.ContainsKey("tag-name")) thOrderDescTagName = interfaceOrderDesc.Attr["tag-name"];
                                                var thOrderDesc = docx.CreateElement(thOrderDescTagName);
                                                thOrder.Children.Add(thOrderDesc);
                                                thOrderDesc.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}!=='{fldName}'||{xmlInterfaceVueOrder.Attr["sort"]}!=='desc'";
                                                // 填充自定义内容
                                                if (interfaceVueOrder.Attr.ContainsKey("field") && interfaceVueOrder.Attr.ContainsKey("sort")) thOrderDesc.Attr["v-if"] = $"{interfaceVueOrder.Attr["field"]}!=='{fldName}'||{interfaceVueOrder.Attr["sort"]}!=='desc'";
                                                // 填充line定义内容
                                                foreach (var key in xmlInterfaceOrderDesc.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderDesc.Attr[key] = xmlInterfaceOrderDesc.Attr[key];
                                                }
                                                // 填充line定义内容
                                                foreach (var key in interfaceOrderDesc.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderDesc.Attr[key] = interfaceOrderDesc.Attr[key];
                                                }
                                                #endregion
                                                #region [=====处理反序激活组件相关内容=====]
                                                // desc-act
                                                var xmlInterfaceOrderDescAct = xmlInterfaceOrder["desc-act"];
                                                if (xmlInterfaceOrderDescAct == null) {
                                                    xmlInterfaceOrderDescAct = xmlInterfaceOrder.AddNode("desc-act");
                                                    xmlInterfaceOrderDescAct.IsSingle = true;
                                                    xmlInterfaceOrderDescAct.Attr["tag-name"] = "div";
                                                    xmlInterfaceOrderDescAct.Attr["class"] = "x-order-desc-act";
                                                    needSave = true;
                                                }
                                                var interfaceOrderDescAct = interfaceOrder["desc-act"];
                                                if (interfaceOrderDescAct == null) interfaceOrderDescAct = interfaceOrder.AddNode("desc-act");
                                                // 反序控件激活
                                                string thOrderDescActTagName = xmlInterfaceOrderDescAct.Attr["tag-name"];
                                                if (interfaceOrderDescAct.Attr.ContainsKey("tag-name")) thOrderDescActTagName = interfaceOrderDescAct.Attr["tag-name"];
                                                var thOrderDescAct = docx.CreateElement(thOrderDescActTagName);
                                                thOrder.Children.Add(thOrderDescAct);
                                                thOrderDescAct.Attr["v-if"] = $"{xmlInterfaceVueOrder.Attr["field"]}==='{fldName}'&&{xmlInterfaceVueOrder.Attr["sort"]}==='desc'";
                                                // 填充自定义内容
                                                if (interfaceVueOrder.Attr.ContainsKey("field") && interfaceVueOrder.Attr.ContainsKey("sort")) thOrderDescAct.Attr["v-if"] = $"{interfaceVueOrder.Attr["field"]}==='{fldName}'||{interfaceVueOrder.Attr["sort"]}==='desc'";
                                                // 填充line定义内容
                                                foreach (var key in xmlInterfaceOrderDescAct.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderDescAct.Attr[key] = xmlInterfaceOrderDescAct.Attr[key];
                                                }
                                                // 填充line定义内容
                                                foreach (var key in interfaceOrderDescAct.Attr.Keys) {
                                                    if (key != "tag-name")
                                                        thOrderDescAct.Attr[key] = interfaceOrderDescAct.Attr[key];
                                                }
                                                #endregion
                                            }
                                            #endregion
                                            #region [=====处理数据行=====]
                                            var td = docx.CreateElement("td");
                                            data.Children.Add(td);
                                            #region [=====处理cell组件相关内容=====]
                                            // 读取cell配置
                                            var xmlInterfaceCell = listGroup["cell"];
                                            if (xmlInterfaceCell == null) {
                                                xmlInterfaceCell = listGroup.AddNode("cell");
                                                xmlInterfaceCell.IsSingle = true;
                                                needSave = true;
                                            }
                                            // 填充cell定义内容
                                            foreach (var key in xmlInterfaceCell.Attr.Keys) {
                                                td.Attr[key] = xmlInterfaceCell.Attr[key];
                                            }
                                            // 读取自定义配置
                                            var interfaceCell = xmlInterface["cell"];
                                            if (interfaceCell == null) interfaceCell = xmlInterface.AddNode("cell");
                                            // 生效自定义配置
                                            foreach (var key in interfaceCell.Attr.Keys) {
                                                td.Attr[key] = interfaceCell.Attr[key];
                                            }
                                            #endregion
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
                                                    // 定义图像控件操作
                                                    var xmlInterfaceImg = listGroup["img"];
                                                    if (xmlInterfaceImg == null) {
                                                        xmlInterfaceImg = listGroup.AddNode("img");
                                                        needSave = true;
                                                    }
                                                    // 读取自定义配置
                                                    var interfaceImg = xmlInterface["img"];
                                                    if (interfaceImg == null) interfaceImg = xmlInterface.AddNode("img");
                                                    // 定义图像控件容器
                                                    var xmlInterfaceImgBox = xmlInterfaceImg["box"];
                                                    if (xmlInterfaceImgBox == null) {
                                                        xmlInterfaceImgBox = xmlInterfaceImg.AddNode("box");
                                                        xmlInterfaceImgBox.IsSingle = true;
                                                        xmlInterfaceImgBox.Attr["tag-name"] = "s";
                                                        needSave = true;
                                                    }
                                                    // 读取自定义配置
                                                    var interfaceImgBox = interfaceImg["box"];
                                                    if (interfaceImgBox == null) interfaceImgBox = interfaceImg.AddNode("box");
                                                    // 定义标签
                                                    string imgBoxTagName = xmlInterfaceImgBox.Attr["tag-name"];
                                                    if (interfaceImgBox.Attr.ContainsKey("tag-name")) imgBoxTagName = interfaceImgBox.Attr["tag-name"];
                                                    var imgBox = docx.CreateElement(imgBoxTagName);
                                                    td.Children.Add(imgBox);
                                                    // 填充图像容器定义内容
                                                    foreach (var key in xmlInterfaceImgBox.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            imgBox.Attr[key] = xmlInterfaceImgBox.Attr[key];
                                                    }
                                                    // 填充图像容器定义内容
                                                    foreach (var key in interfaceImgBox.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            imgBox.Attr[key] = interfaceImgBox.Attr[key];
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
                                                    // 填充图像定义内容
                                                    foreach (var key in interfaceImg.Attr.Keys) {
                                                        img.Attr[key] = interfaceImg.Attr[key];
                                                    }
                                                    break;
                                                #endregion
                                                // 呈现为勾选框
                                                case "check":
                                                    #region [=====呈现为勾选框=====]
                                                    // 定义勾选框控件操作
                                                    var xmlInterfaceCheck = listGroup["check"];
                                                    if (xmlInterfaceCheck == null) {
                                                        xmlInterfaceCheck = listGroup.AddNode("check");
                                                        xmlInterfaceCheck.IsSingle = true;
                                                        xmlInterfaceCheck.Attr["tag-name"] = "div";
                                                        xmlInterfaceCheck.Attr["class"] = "x-check";
                                                        needSave = true;
                                                    }
                                                    // 读取自定义配置
                                                    var interfaceCheck = xmlInterface["check"];
                                                    if (interfaceCheck == null) interfaceCheck = xmlInterface.AddNode("check");
                                                    // 定义标签
                                                    string checkBoxTagName = xmlInterfaceCheck.Attr["tag-name"];
                                                    if (interfaceCheck.Attr.ContainsKey("tag-name")) checkBoxTagName = interfaceCheck.Attr["tag-name"];
                                                    var checkBox = docx.CreateElement(checkBoxTagName);
                                                    td.Children.Add(checkBox);
                                                    // 填充图像容器定义内容
                                                    foreach (var key in xmlInterfaceCheck.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            checkBox.Attr[key] = xmlInterfaceCheck.Attr[key];
                                                    }
                                                    // 填充图像容器定义内容
                                                    foreach (var key in interfaceCheck.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            checkBox.Attr[key] = interfaceCheck.Attr[key];
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
                                                    input.Attr["style"] = $"width:{fldWidth};";
                                                    input.Attr["v-model"] = $"row.{fldName}";
                                                    input.Attr["v-on:blur"] = $"onCellEdit($event,'{fldName}',row)";
                                                    break;
                                                #endregion
                                                // 呈现为自定义HTML
                                                case "html":
                                                    // 读取字段html定义文件
                                                    string fldHtmPath = workFolder + "\\field." + fldName + ".htm";
                                                    // form.Say($"读取字段HTML定义文件 {fldHtmPath} ...");
                                                    string htmlContent = dpz3.File.UTF8File.ReadAllText(fldHtmPath, true);
                                                    td.InnerHTML = htmlContent;
                                                    break;
                                                default:
                                                    throw new Exception("不支持的字段呈现类型");
                                            }
                                            #endregion
                                            #region [=====处理统计行=====]
                                            if (hasStatistics) {
                                                var tdStatistics = docx.CreateElement("td");
                                                trStatistics.Children.Add(tdStatistics);
                                                tdStatistics.Attr["v-html"] = $"statistics.{fldName}";
                                            }
                                            #endregion
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
                                    if (xmlInterface == null) xmlInterface = xmlTable.AddNode("interface");
                                    // 字段定义节点
                                    xmlFields = xmlTable["fields"];
                                    // 遍历字段
                                    foreach (var xmlField in xmlFields.GetNodesByTagName("field")) {
                                        string fldName = xmlField.Attr["name"];
                                        string fldTitle = xmlField.Attr["title"];
                                        string fldMust = xmlField.Attr["must"];
                                        string fldType = xmlField.Attr["type"]?.ToLower();
                                        // 字段类型不为none时，才有输出
                                        if (fldType != "none") {
                                            string fldModel = xmlField.Attr["model"];
                                            #region [=====处理line组件相关内容=====]
                                            // 定义line呈现
                                            var xmlInterfaceLine = formGroup["line"];
                                            if (xmlInterfaceLine == null) {
                                                xmlInterfaceLine = formGroup.AddNode("line");
                                                xmlInterfaceLine.IsSingle = true;
                                                xmlInterfaceLine.Attr["tag-name"] = "div";
                                                xmlInterfaceLine.Attr["class"] = "x-line";
                                                needSave = true;
                                            }
                                            // 读取自定义配置
                                            var interfaceLine = xmlInterface["line"];
                                            if (interfaceLine == null) interfaceLine = xmlInterface.AddNode("line");
                                            // 处理行
                                            string eleLineTagName = xmlInterfaceLine.Attr["tag-name"];
                                            if (interfaceLine.Attr.ContainsKey("tag-name")) eleLineTagName = interfaceLine.Attr["tag-name"];
                                            var eleLine = docx.CreateElement(eleLineTagName);
                                            docx.Nodes.Add(eleLine);
                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceLine.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleLine.Attr[key] = xmlInterfaceLine.Attr[key];
                                            }
                                            // 填充line定义内容
                                            foreach (var key in interfaceLine.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleLine.Attr[key] = interfaceLine.Attr[key];
                                            }
                                            // 填充专用的样式信息
                                            if (eleLine.Attr["class"].IsNoneOrNull()) {
                                                eleLine.Attr["class"] = $"xform-line-{fldName}";
                                            } else {
                                                eleLine.Attr["class"] += $" xform-line-{fldName}";
                                            }
                                            #endregion
                                            #region [=====处理liner组件相关内容=====]
                                            // 定义liner呈现
                                            var xmlInterfaceLiner = formGroup["liner"];
                                            if (xmlInterfaceLiner == null) {
                                                xmlInterfaceLiner = formGroup.AddNode("liner");
                                                xmlInterfaceLiner.IsSingle = true;
                                                xmlInterfaceLiner.Attr["tag-name"] = "div";
                                                xmlInterfaceLiner.Attr["class"] = "x-liner";
                                                needSave = true;
                                            }
                                            // 读取自定义配置
                                            var interfaceLiner = xmlInterface["liner"];
                                            if (interfaceLiner == null) interfaceLiner = xmlInterface.AddNode("liner");
                                            // 处理行伴随
                                            string eleLinerTagName = xmlInterfaceLiner.Attr["tag-name"];
                                            if (interfaceLiner.Attr.ContainsKey("tag-name")) eleLinerTagName = interfaceLiner.Attr["tag-name"];
                                            var eleLiner = docx.CreateElement(eleLinerTagName);
                                            docx.Nodes.Add(eleLiner);
                                            // 填充liner定义内容
                                            foreach (var key in xmlInterfaceLiner.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleLiner.Attr[key] = xmlInterfaceLiner.Attr[key];
                                            }
                                            // 填充liner定义内容
                                            foreach (var key in interfaceLiner.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleLiner.Attr[key] = interfaceLiner.Attr[key];
                                            }
                                            // 填充专用的样式信息
                                            if (eleLiner.Attr["class"].IsNoneOrNull()) {
                                                eleLiner.Attr["class"] = $"xform-liner-{fldName}";
                                            } else {
                                                eleLiner.Attr["class"] += $" xform-liner-{fldName}";
                                            }
                                            #endregion
                                            #region [=====处理title组件相关内容=====]
                                            // 定义title呈现
                                            var xmlInterfaceTitle = formGroup["title"];
                                            if (xmlInterfaceTitle == null) {
                                                xmlInterfaceTitle = formGroup.AddNode("title");
                                                xmlInterfaceTitle.IsSingle = true;
                                                xmlInterfaceTitle.Attr["tag-name"] = "div";
                                                xmlInterfaceTitle.Attr["class"] = "x-title";
                                                needSave = true;
                                            }
                                            // 读取自定义配置
                                            var interfaceTitle = xmlInterface["title"];
                                            if (interfaceTitle == null) interfaceTitle = xmlInterface.AddNode("title");
                                            // 处理标题
                                            string eleTitleTagName = xmlInterfaceTitle.Attr["tag-name"];
                                            if (interfaceTitle.Attr.ContainsKey("tag-name")) eleTitleTagName = interfaceTitle.Attr["tag-name"];
                                            var eleTitle = docx.CreateElement(eleTitleTagName);
                                            eleLine.Children.Add(eleTitle);
                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceTitle.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleTitle.Attr[key] = xmlInterfaceTitle.Attr[key];
                                            }
                                            // 填充line定义内容
                                            foreach (var key in interfaceTitle.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleTitle.Attr[key] = interfaceTitle.Attr[key];
                                            }
                                            // 填充专用的样式信息
                                            if (eleTitle.Attr["class"].IsNoneOrNull()) {
                                                eleTitle.Attr["class"] = $"xform-title-{fldName}";
                                            } else {
                                                eleTitle.Attr["class"] += $" xform-title-{fldName}";
                                            }
                                            if (fldMust == "true") {
                                                eleTitle.InnerHTML = $"{fldTitle}(<i>*</i>)";
                                            } else {
                                                eleTitle.InnerHTML = fldTitle;
                                            }
                                            #endregion
                                            #region [=====处理content组件相关内容=====]
                                            // 定义content呈现
                                            var xmlInterfaceContent = formGroup["content"];
                                            if (xmlInterfaceContent == null) {
                                                xmlInterfaceContent = formGroup.AddNode("content");
                                                xmlInterfaceContent.IsSingle = true;
                                                xmlInterfaceContent.Attr["tag-name"] = "div";
                                                xmlInterfaceContent.Attr["class"] = "x-content";
                                                needSave = true;
                                            }
                                            // 读取自定义配置
                                            var interfaceContent = xmlInterface["content"];
                                            if (interfaceContent == null) interfaceContent = xmlInterface.AddNode("content");
                                            // 处理内容
                                            string eleContentTagName = xmlInterfaceContent.Attr["tag-name"];
                                            if (interfaceContent.Attr.ContainsKey("tag-name")) eleContentTagName = interfaceContent.Attr["tag-name"];
                                            var eleContent = docx.CreateElement(eleContentTagName);
                                            eleLine.Children.Add(eleContent);
                                            // 填充line定义内容
                                            foreach (var key in xmlInterfaceContent.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleContent.Attr[key] = xmlInterfaceContent.Attr[key];
                                            }
                                            // 填充line定义内容
                                            foreach (var key in interfaceContent.Attr.Keys) {
                                                if (key != "tag-name")
                                                    eleContent.Attr[key] = interfaceContent.Attr[key];
                                            }
                                            // 填充专用的样式信息
                                            if (eleContent.Attr["class"].IsNoneOrNull()) {
                                                eleContent.Attr["class"] = $"xform-content-{fldName}";
                                            } else {
                                                eleContent.Attr["class"] += $" xform-content-{fldName}";
                                            }
                                            #endregion
                                            switch (fldType) {
                                                // 不呈现
                                                case "none": break;
                                                // 呈现为输入框
                                                case "text":
                                                    #region [=====呈现为文本=====]
                                                    // 处理内容
                                                    string fldWidth = xmlField.Attr["width"];
                                                    var eleText = docx.CreateElement("span");
                                                    eleContent.Children.Add(eleText);
                                                    eleText.Attr["id"] = $"lab{fldName}";
                                                    eleText.Attr["name"] = fldName;
                                                    eleText.Attr["style"] = $"width:{fldWidth};";
                                                    eleText.Attr["v-html"] = $"form.{fldName}";
                                                    //eleText.InnerHTML = $"{{{{form.{fldName}}}}}";
                                                    break;
                                                #endregion
                                                // 呈现为输入框
                                                case "input":
                                                    #region [=====呈现为输入框=====]
                                                    // 读取宽度
                                                    fldWidth = xmlField.Attr["width"];
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
                                                    // 定义勾选框控件呈现
                                                    var xmlInterfaceCheck = formGroup["check"];
                                                    if (xmlInterfaceCheck == null) {
                                                        xmlInterfaceCheck = formGroup.AddNode("check");
                                                        xmlInterfaceCheck.IsSingle = true;
                                                        xmlInterfaceCheck.Attr["tag-name"] = "div";
                                                        xmlInterfaceCheck.Attr["class"] = "x-check";
                                                        needSave = true;
                                                    }
                                                    // 读取自定义配置
                                                    var interfaceCheck = xmlInterface["check"];
                                                    if (interfaceCheck == null) interfaceCheck = xmlInterface.AddNode("check");
                                                    // 定义标签
                                                    string checkBoxTagName = xmlInterfaceCheck.Attr["tag-name"];
                                                    if (interfaceCheck.Attr.ContainsKey("tag-name")) checkBoxTagName = interfaceCheck.Attr["tag-name"];
                                                    var checkBox = docx.CreateElement(checkBoxTagName);
                                                    eleContent.Children.Add(checkBox);
                                                    // 填充图像容器定义内容
                                                    foreach (var key in xmlInterfaceCheck.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            checkBox.Attr[key] = xmlInterfaceCheck.Attr[key];
                                                    }
                                                    // 填充图像容器定义内容
                                                    foreach (var key in interfaceCheck.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            checkBox.Attr[key] = interfaceCheck.Attr[key];
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
                                                    // 读取宽度
                                                    fldWidth = xmlField.Attr["width"];
                                                    // 定义日期选择控件呈现
                                                    var formDay = formGroup["day-picker"];
                                                    if (formDay == null) {
                                                        formDay = formGroup.AddNode("day-picker");
                                                        formDay.IsSingle = true;
                                                        formDay.Attr["tag-name"] = "div";
                                                        formDay.Attr["class"] = "x-day-picker";
                                                        needSave = true;
                                                    }
                                                    // 读取自定义配置
                                                    var interfaceDay = xmlInterface["day-picker"];
                                                    if (interfaceDay == null) interfaceDay = xmlInterface.AddNode("day-picker");
                                                    // 定义标签
                                                    string dayBoxTagName = formDay.Attr["tag-name"];
                                                    if (interfaceDay.Attr.ContainsKey("tag-name")) dayBoxTagName = interfaceDay.Attr["tag-name"];
                                                    var dayBox = docx.CreateElement(dayBoxTagName);
                                                    eleContent.Children.Add(dayBox);
                                                    dayBox.Attr["style"] = $"width:{fldWidth};";
                                                    // 填充图像容器定义内容
                                                    foreach (var key in formDay.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            dayBox.Attr[key] = formDay.Attr[key];
                                                    }
                                                    // 填充图像容器定义内容
                                                    foreach (var key in interfaceDay.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            dayBox.Attr[key] = interfaceDay.Attr[key];
                                                    }
                                                    // 定义超链接
                                                    var dayLink = docx.CreateElement("a");
                                                    dayBox.Children.Add(dayLink);
                                                    dayLink.Attr["href"] = "javascript:;";
                                                    dayLink.Attr["v-on:click"] = $"onDayPicked($event,'{fldName}')";
                                                    dayLink.Attr["v-html"] = $"form.{fldName}";
                                                    //// 定义勾选状态标签
                                                    //var checkSpan = docx.CreateElement("span");
                                                    //checkLink.Children.Add(checkSpan);
                                                    //checkSpan.Attr["v-if"] = $"form.{fldName}===1||form.{fldName}==='1'";
                                                    //checkSpan.InnerHTML = "√";
                                                    break;
                                                #endregion
                                                // 呈现为选择框
                                                case "picker":
                                                    #region [=====呈现为选择框=====]
                                                    // 读取宽度
                                                    fldWidth = xmlField.Attr["width"];
                                                    // 定义日期选择控件呈现
                                                    var formPicker = formGroup["picker"];
                                                    if (formPicker == null) {
                                                        formPicker = formGroup.AddNode("picker");
                                                        formPicker.IsSingle = true;
                                                        formPicker.Attr["tag-name"] = "div";
                                                        formPicker.Attr["class"] = "x-picker";
                                                        needSave = true;
                                                    }
                                                    // 读取自定义配置
                                                    var interfacePicker = xmlInterface["picker"];
                                                    if (interfacePicker == null) interfacePicker = xmlInterface.AddNode("picker");
                                                    // 定义标签
                                                    string pickerBoxTagName = formPicker.Attr["tag-name"];
                                                    if (interfacePicker.Attr.ContainsKey("tag-name")) pickerBoxTagName = interfacePicker.Attr["tag-name"];
                                                    var pickerBox = docx.CreateElement(pickerBoxTagName);
                                                    eleContent.Children.Add(pickerBox);
                                                    pickerBox.Attr["style"] = $"width:{fldWidth};";
                                                    // 填充图像容器定义内容
                                                    foreach (var key in formPicker.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            pickerBox.Attr[key] = formPicker.Attr[key];
                                                    }
                                                    // 填充图像容器定义内容
                                                    foreach (var key in interfacePicker.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            pickerBox.Attr[key] = interfacePicker.Attr[key];
                                                    }
                                                    // 定义超链接
                                                    var pickerLink = docx.CreateElement("a");
                                                    pickerBox.Children.Add(pickerLink);
                                                    pickerLink.Attr["href"] = "javascript:;";
                                                    pickerLink.Attr["v-on:click"] = $"onPicked($event,'{fldName}','{fldTitle}')";
                                                    pickerLink.Attr["v-html"] = $"form.{fldName}";
                                                    break;
                                                #endregion
                                                // 呈现为图片上传框
                                                case "img":
                                                    #region [=====呈现为勾选框=====]
                                                    // 定义图片上传控件呈现
                                                    var xmlInterfaceImg = formGroup["img"];
                                                    if (xmlInterfaceImg == null) {
                                                        xmlInterfaceImg = formGroup.AddNode("img");
                                                        xmlInterfaceImg.IsSingle = true;
                                                        xmlInterfaceImg.Attr["tag-name"] = "div";
                                                        xmlInterfaceImg.Attr["class"] = "x-img";
                                                        needSave = true;
                                                    }
                                                    // 读取自定义配置
                                                    var interfaceImg = xmlInterface["img"];
                                                    if (interfaceImg == null) interfaceImg = xmlInterface.AddNode("img");
                                                    // 定义标签
                                                    string imgBoxTagName = xmlInterfaceImg.Attr["tag-name"];
                                                    if (interfaceImg.Attr.ContainsKey("tag-name")) imgBoxTagName = interfaceImg.Attr["tag-name"];
                                                    var imgBox = docx.CreateElement(imgBoxTagName);
                                                    eleContent.Children.Add(imgBox);
                                                    // 填充图像容器定义内容
                                                    foreach (var key in xmlInterfaceImg.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            imgBox.Attr[key] = xmlInterfaceImg.Attr[key];
                                                    }
                                                    // 填充图像容器定义内容
                                                    foreach (var key in interfaceImg.Attr.Keys) {
                                                        if (key != "tag-name")
                                                            imgBox.Attr[key] = interfaceImg.Attr[key];
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
                                                    // form.Say($"读取字段HTML定义文件 {fldHtmPath} ...");
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
                    // form.Say($"正在输出文件 {outputFile} ...");
                    dpz3.File.UTF8File.WriteAllText(outputFile, doc.InnerHTML);
                }
                // 保存配置
                if (needSave) dpz3.File.UTF8File.WriteAllText(pathControls, docControls.InnerXml);
            }
        }

    }
}
