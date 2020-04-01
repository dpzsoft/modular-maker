using System;
using dpz3;
using dpz3.Modular;
using System.IO.Compression;
using System.Text;

namespace controller {

    [Modular(ModularTypes.SessionApi, "/Api/{ControllerName}")]
    public class Maker : JttpSessionControllerBase {

        [Modular(ModularTypes.Post, "BuildController")]
        public IResult BuildController() {
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
            // 生成文件控制器
            string pathController = $"{workFolder}/controller/bin/Debug/netcoreapp3.1/controller.dll";
            if (System.IO.File.Exists(pathController)) {
                try {
                    System.IO.File.Delete(pathController);
                } catch (Exception ex) {
                    return Error(ex.Message, 0, ex.ToString());
                }
            }
            // 存储登录信息
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WorkingDirectory = $"{workFolder}/controller";
            info.FileName = "dotnet";
            info.Arguments = "build /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary";
            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            pro.StartInfo = info;
            pro.Start();
            pro.WaitForExit();
            if (!System.IO.File.Exists(pathController)) return Fail("编译失败");
            return Success("控制器编译成功");
        }

        // 复制文件夹
        void CopyFolder(string pathSource, string pathTarget) {

            // 复制子文件夹
            string[] dirs = System.IO.Directory.GetDirectories(pathSource);
            foreach (var dir in dirs) {
                string name = System.IO.Path.GetFileName(dir);
                if (!name.StartsWith(".")) {
                    string pathNew = $"{pathTarget}/{name}";
                    //Console.WriteLine($"[+] 创建目录 {pathNew} ...");
                    if (!System.IO.Directory.Exists(pathNew)) System.IO.Directory.CreateDirectory(pathNew);
                    CopyFolder(dir, pathNew);
                }
            }

            // 复制文件
            string[] files = System.IO.Directory.GetFiles(pathSource);
            foreach (var file in files) {
                string name = System.IO.Path.GetFileName(file);
                if (!name.StartsWith(".")) {
                    string pathNew = $"{pathTarget}/{name}";
                    //Console.WriteLine($"[+] 增加拷贝文件 {pathNew} ...");
                    System.IO.File.Copy(file, pathNew, true);
                }
            }
        }

        // 复制文件夹
        void MakerController(string pathSource, string pathTarget, string className, string route) {

            Console.WriteLine($"[*] 查找目录 {pathSource} ...");

            // 生成控制器代码
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;\r\n");
            sb.Append("using System.Text;\r\n");
            sb.Append("using dpz3;\r\n");
            sb.Append("using dpz3.Modular;\r\n");
            sb.Append("\r\n");
            sb.Append($"namespace {className} {{\r\n");
            sb.Append("\r\n");
            sb.Append($"    [Modular(ModularTypes.Session, \"{route}\")]\r\n");
            sb.Append($"    public class _Page : SessionControllerBase {{\r\n");
            sb.Append("\r\n");

            // 复制文件
            string[] files = System.IO.Directory.GetFiles(pathSource, "*.aspx");
            foreach (var file in files) {
                Console.WriteLine($"[*] 处理文件 {file} ...");
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                string html = dpz3.File.UTF8File.ReadAllText(file);
                string code = dpz3.Modular.Parser.ParseAspxToCode(html, name);
                sb.Append($"        [Modular(ModularTypes.Get, \"{name}\")]\r\n");
                sb.Append(code);
                sb.Append("\r\n");
            }

            sb.Append($"    }}\r\n");
            sb.Append("}\r\n");

            // 输出文件内容
            string pathClass = $"{pathTarget}/{className}.cs";
            Console.WriteLine($"[+] 创建控制器 {pathClass} ...");
            dpz3.File.UTF8File.WriteAllText(pathClass, sb.ToString());

            // 处理子文件夹
            string[] dirs = System.IO.Directory.GetDirectories(pathSource);
            foreach (var dir in dirs) {
                string name = System.IO.Path.GetFileName(dir);
                bool canScan = true;
                // 排除一些工程文件
                if (name.StartsWith(".")) canScan = false;
                if (name == "bin") canScan = false;
                if (name == "obj") canScan = false;
                if (name == "packages") canScan = false;
                if (name == "Properties") canScan = false;
                if (canScan) {
                    string nameNew = $"{className}.{name}";
                    string routeNew = $"{route}/{name}";
                    //Console.WriteLine($"[+] 创建目录 {pathNew} ...");
                    //if (!System.IO.Directory.Exists(pathNew)) System.IO.Directory.CreateDirectory(pathNew);
                    MakerController(dir, pathTarget, nameNew, routeNew);
                }
            }

        }

        [Modular(ModularTypes.Post, "BuildPages")]
        public IResult BuildPages() {
            dpz3.db.Connection dbc = Host.Connection;
            string workFolder = Host.WorkFolder.Replace("\\", "/");
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathXml = $"{workFolder}conf/projects.xml";
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            // string workFolder = item.Attr["path"];
            string path = item.Attr["path"].Replace("\\", "/");
            // 判断路径是否指定
            if (path.IsNoneOrNull()) {
                //Console.WriteLine($"[!] 未指定路径");
                return Fail("未指定路径");
            }
            // 将路径进行处理
            if (!path.EndsWith("/")) path += "/";
            // 判断页面文件夹
            string pathPages = $"{path}pages";
            if (!System.IO.Directory.Exists(pathPages)) {
                //Console.WriteLine($"[!] 未找到pages文件夹");
                return Fail("未找到pages文件夹");
            }
            // 判断并创建控制器工程文件夹
            string pathController = $"{path}controller/pages";
            if (!System.IO.Directory.Exists(pathController)) {
                Console.WriteLine($"[+] 创建文件夹 {pathController} ...");
                System.IO.Directory.CreateDirectory(pathController);
                return Fail("未找到pages文件夹");
            }
            MakerController(pathPages, pathController, "pages", "");
            return Success("控制器页面代码生成成功");
        }

        [Modular(ModularTypes.Post, "BuildPackage")]
        public IResult BuildPackage() {
            dpz3.db.Connection dbc = Host.Connection;
            string workFolder = Host.WorkFolder.Replace("\\", "/");
            // 获取参数
            string groupName = Request.Str["Group"];
            string itemName = Request.Str["Item"];
            if (!workFolder.EndsWith("/")) workFolder += "/";
            string pathXml = $"{workFolder}conf/projects.xml";
            string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
            dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
            var xml = doc["xml"];
            dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
            dpz3.Xml.XmlNode item = null;
            if (group != null) item = group.GetNodeByAttr("name", itemName, false);
            if (item == null) return Fail("未发现项目信息");
            // 获取路径信息
            // string workFolder = item.Attr["path"];
            string path = item.Attr["path"].Replace("\\", "/");
            // 判断路径是否指定
            if (path.IsNoneOrNull()) {
                // Console.WriteLine($"[!] 未指定路径");
                return Fail("未指定路径");
            }
            // 将路径进行处理
            if (!path.EndsWith("/")) path += "/";
            string pathCfg = $"{path}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                //Console.WriteLine($"[!] 未找到配置文件");
                return Fail("未找到配置文件");
            }
            // 读取配置文件
            // Console.WriteLine($"[*] 读取配置文件 {pathCfg} ...");
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            string name;
            string version;
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                name = json.Str["name"];
                version = json.Str["version"];
                // Console.WriteLine($"[*] 当前版本 {version}");
                var ver = new dpz3.TimeVersion(version);
                version = ver.GetNextVersion().ToString();
                // Console.WriteLine($"[*] 新版本 {version}");
                json.Str["version"] = version;
                dpz3.File.UTF8File.WriteAllText(pathCfg, json.ToJsonString());
            }
            // 创建临时输出目录
            string pathOutput = $"{path}output";
            if (!System.IO.Directory.Exists(pathOutput)) {
                // Console.WriteLine($"[+] 创建目录 {pathOutput} ...");
                System.IO.Directory.CreateDirectory(pathOutput);
            }
            // 复制配置文件
            string pathOutputCfg = $"{pathOutput}/modular.json";
            Console.WriteLine($"[+] 增加拷贝文件 {pathOutputCfg} ...");
            System.IO.File.Copy(pathCfg, pathOutputCfg, true);
            // 复制静态页面
            string pathRoot = $"{path}wwwroot";
            if (System.IO.Directory.Exists(pathRoot)) {
                string pathOutputRoot = $"{pathOutput}/wwwroot";
                if (!System.IO.Directory.Exists(pathOutputRoot)) {
                    //Console.WriteLine($"[+] 创建目录 {pathOutputRoot} ...");
                    System.IO.Directory.CreateDirectory(pathOutputRoot);
                }
                // 复制需要打包的文件到临时文件夹中
                CopyFolder(pathRoot, pathOutputRoot);
            }
            // 复制数据库定义文件夹
            string pathXorm = $"{path}xorm";
            if (System.IO.Directory.Exists(pathXorm)) {
                string pathOutputXorm = $"{pathOutput}/xorm";
                if (!System.IO.Directory.Exists(pathOutputXorm)) {
                    Console.WriteLine($"[+] 创建目录 {pathOutputXorm} ...");
                    System.IO.Directory.CreateDirectory(pathOutputXorm);
                }
                // 复制需要打包的文件到临时文件夹中
                CopyFolder(pathXorm, pathOutputXorm);
            }
            // 复制控制器
            string pathController = $"{path}controller/bin/Debug/netcoreapp3.1";
            if (System.IO.Directory.Exists(pathController)) {
                // 复制需要打包的文件到临时文件夹中
                CopyFolder(pathController, pathOutput);
            }
            // 建立包文件夹
            string pathPackage = $"{path}package";
            if (!System.IO.Directory.Exists(pathPackage)) {
                //Console.WriteLine($"[+] 创建目录 {pathPackage} ...");
                System.IO.Directory.CreateDirectory(pathPackage);
            }
            // 打包文件
            string pathPackageFile = $"{pathPackage}/{name}-{version}.zip";
            //Console.WriteLine($"[+] 输出包文件 {pathPackageFile} ...");
            ZipFile.CreateFromDirectory(pathOutput, pathPackageFile);
            // 清理临时目录
            if (System.IO.Directory.Exists(pathOutput)) {
                //Console.WriteLine($"[-] 清理目录 {pathOutput} ...");
                System.IO.Directory.Delete(pathOutput, true);
            }
            // 生成脚本文件
            string pathBatFile = $"{pathPackage}/{name}.bat";
            string cmd = $"@echo off\r\n" +
                $"cd ..\r\n" +
                $".\\pm /name {name} /version {version}\r\n" +
                $"cd downloads\r\n" +
                $"del {name}.bat\r\n" +
                $"pause";
            dpz3.File.TextFile.WriteAllBytes(pathBatFile, System.Text.Encoding.ASCII.GetBytes(cmd));
            return Success("生成MP包成功");
        }
    }
}
