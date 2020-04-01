using System;
using dpz3;
using dpz3.Modular;

namespace controller {

    [Modular(ModularTypes.SessionApi, "/Api/{ControllerName}")]
    public class Tools : JttpSessionControllerBase {

        [Modular(ModularTypes.Post, "OpenPages")]
        public IResult OpenPages() {
            dpz3.db.Connection dbc = Host.Connection;
            string path = Host.WorkFolder.Replace("\\", "/");
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            if (!path.EndsWith("/")) path += "/";
            string pathXml = $"{path}conf/projects.xml";
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            string slnPath = $"{workFolder}/pages/pages.sln";
            // 读取工具配置
            pathXml = $"{path}conf/tools.xml";
            szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument docTools = dpz3.Xml.Parser.GetDocument(szXml);
            var tools = docTools["tools"];
            if (tools == null) return Fail("未发现visualstudio配置信息");
            var tool = tools.GetNodeByAttr("name", "visualstudio", false);
            if (tool == null) return Fail("未发现visualstudio配置信息");
            string toolPath = tool.Attr["path"];
            if (toolPath.IsNoneOrNull()) return Fail("未发现visualstudio配置信息");
            // 存储登录信息
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = toolPath;
            info.Arguments = slnPath;
            System.Diagnostics.Process.Start(info);
            return Success();
        }

        [Modular(ModularTypes.Post, "OpenController")]
        public IResult OpenController() {
            dpz3.db.Connection dbc = Host.Connection;
            string path = Host.WorkFolder.Replace("\\", "/");
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            if (!path.EndsWith("/")) path += "/";
            string pathXml = $"{path}conf/projects.xml";
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            string workFolder = item.Attr["path"];
            string slnPath = $"{workFolder}/controller/controller.sln";
            // 读取工具配置
            pathXml = $"{path}conf/tools.xml";
            szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument docTools = dpz3.Xml.Parser.GetDocument(szXml);
            var tools = docTools["tools"];
            if (tools == null) return Fail("未发现visualstudio配置信息");
            var tool = tools.GetNodeByAttr("name", "visualstudio", false);
            if (tool == null) return Fail("未发现visualstudio配置信息");
            string toolPath = tool.Attr["path"];
            if (toolPath.IsNoneOrNull()) return Fail("未发现visualstudio配置信息");
            // 存储登录信息
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = toolPath;
            info.Arguments = slnPath;
            System.Diagnostics.Process.Start(info);
            return Success();
        }
    }
}
