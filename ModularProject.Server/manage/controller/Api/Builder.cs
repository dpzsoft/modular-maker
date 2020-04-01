using System;
using dpz3;
using dpz3.Modular;
using System.IO.Compression;
using System.Text;

namespace controller {

    [Modular(ModularTypes.SessionApi, "/Api/{ControllerName}")]
    public class Builder : JttpSessionControllerBase {

        [Modular(ModularTypes.Post, "BuildController")]
        public IResult BuildController() {
            dpz3.db.Connection dbc = Host.Connection;
            // 生成程序相关路径
            string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
            if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            string name = Request.Str["Name"];
            string pathXml = $"{hostWorkFolder}conf/projects.xml";
            string folderTemplete = $"{hostWorkFolder}template/{groupName}";
            // 读取项目配置
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            // 将路径进行处理
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathCfg = $"{workFolder}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                return Fail("未找到配置文件");
            }
            // 读取包配置文件
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                // 生成文件控制器
                try {
                    PageBuilder.BuildClass(json, folderTemplete, workFolder, name);
                    PageBuilder.BuildControllerClass(json, folderTemplete, workFolder, name);
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            return Success("构架类生成成功");
        }

        [Modular(ModularTypes.Post, "BuildAddForm")]
        public IResult BuildAddForm() {
            dpz3.db.Connection dbc = Host.Connection;
            // 生成程序相关路径
            string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
            if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            string name = Request.Str["Name"];
            string pathXml = $"{hostWorkFolder}conf/projects.xml";
            string folderTemplete = $"{hostWorkFolder}template/{groupName}";
            // 读取项目配置
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            // 将路径进行处理
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathCfg = $"{workFolder}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                return Fail("未找到配置文件");
            }
            // 读取包配置文件
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                // 生成文件控制器
                try {
                    PageBuilder.BuildFormPage(json, folderTemplete, workFolder, name, "Add");
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            return Success("添加表单生成成功");
        }

        [Modular(ModularTypes.Post, "BuildEditForm")]
        public IResult BuildEditForm() {
            dpz3.db.Connection dbc = Host.Connection;
            // 生成程序相关路径
            string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
            if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            string name = Request.Str["Name"];
            string pathXml = $"{hostWorkFolder}conf/projects.xml";
            string folderTemplete = $"{hostWorkFolder}template/{groupName}";
            // 读取项目配置
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            // 将路径进行处理
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathCfg = $"{workFolder}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                return Fail("未找到配置文件");
            }
            // 读取包配置文件
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                // 生成文件控制器
                try {
                    PageBuilder.BuildFormPage(json, folderTemplete, workFolder, name, "Edit");
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            return Success("修改表单生成成功");
        }

        [Modular(ModularTypes.Post, "BuildViewForm")]
        public IResult BuildViewForm() {
            dpz3.db.Connection dbc = Host.Connection;
            // 生成程序相关路径
            string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
            if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            string name = Request.Str["Name"];
            string pathXml = $"{hostWorkFolder}conf/projects.xml";
            string folderTemplete = $"{hostWorkFolder}template/{groupName}";
            // 读取项目配置
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            // 将路径进行处理
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathCfg = $"{workFolder}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                return Fail("未找到配置文件");
            }
            // 读取包配置文件
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                // 生成文件控制器
                try {
                    PageBuilder.BuildFormPage(json, folderTemplete, workFolder, name, "View");
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            return Success("视图生成成功");
        }

        [Modular(ModularTypes.Post, "BuildList")]
        public IResult BuildList() {
            dpz3.db.Connection dbc = Host.Connection;
            // 生成程序相关路径
            string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
            if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            string name = Request.Str["Name"];
            string pathXml = $"{hostWorkFolder}conf/projects.xml";
            string folderTemplete = $"{hostWorkFolder}template/{groupName}";
            // 读取项目配置
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            // 将路径进行处理
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathCfg = $"{workFolder}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                return Fail("未找到配置文件");
            }
            // 读取包配置文件
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                // 生成文件控制器
                try {
                    PageBuilder.BuildListPage(json, folderTemplete, workFolder, name, "List");
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            return Success("列表生成成功");
        }

        [Modular(ModularTypes.Post, "BuildSelector")]
        public IResult BuildSelector() {
            dpz3.db.Connection dbc = Host.Connection;
            // 生成程序相关路径
            string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
            if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            string name = Request.Str["Name"];
            string pathXml = $"{hostWorkFolder}conf/projects.xml";
            string folderTemplete = $"{hostWorkFolder}template/{groupName}";
            // 读取项目配置
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            // 将路径进行处理
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathCfg = $"{workFolder}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                return Fail("未找到配置文件");
            }
            // 读取包配置文件
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                // 生成文件控制器
                try {
                    PageBuilder.BuildListPage(json, folderTemplete, workFolder, name, "Selector");
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            return Success("选择器生成成功");
        }

        [Modular(ModularTypes.Post, "BuildMainPage")]
        public IResult BuildMainPage() {
            dpz3.db.Connection dbc = Host.Connection;
            // 生成程序相关路径
            string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
            if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            string path = Request.Str["Path"];
            string pathXml = $"{hostWorkFolder}conf/projects.xml";
            string folderTemplete = $"{hostWorkFolder}template/{groupName}";
            // 读取项目配置
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            string pathCfg = $"{workFolder}/modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                return Fail("未找到配置文件");
            }
            // 读取包配置文件
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                // 生成文件控制器
                try {
                    PageBuilder.BuildMainPage(folderTemplete, workFolder, $"{workFolder}{path}/src", $"{workFolder}{path}/src/main.htm", $"{workFolder}{path}/page.html");
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            return Success("主页面生成成功");
        }

    }
}
