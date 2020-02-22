using System;
using System.IO.Compression;
using dpz3;
using System.Text;

namespace cmaker {
    class Program {

        // 复制文件夹
        static void MakerController(string pathSource, string pathTarget, string className, string route) {

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
            string pathClass = $"{pathTarget}{it.SplitChar}{className}.cs";
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

        // 复制文件夹
        static void CopyFolder(string pathSource, string pathTarget) {

            // 复制子文件夹
            string[] dirs = System.IO.Directory.GetDirectories(pathSource);
            foreach (var dir in dirs) {
                string name = System.IO.Path.GetFileName(dir);
                if (!name.StartsWith(".")) {
                    string pathNew = $"{pathTarget}{it.SplitChar}{name}";
                    Console.WriteLine($"[+] 创建目录 {pathNew} ...");
                    if (!System.IO.Directory.Exists(pathNew)) System.IO.Directory.CreateDirectory(pathNew);
                    CopyFolder(dir, pathNew);
                }
            }

            // 复制文件
            string[] files = System.IO.Directory.GetFiles(pathSource);
            foreach (var file in files) {
                string name = System.IO.Path.GetFileName(file);
                if (!name.StartsWith(".")) {
                    string pathNew = $"{pathTarget}{it.SplitChar}{name}";
                    Console.WriteLine($"[+] 增加拷贝文件 {pathNew} ...");
                    System.IO.File.Copy(file, pathNew, true);
                }
            }
        }

        static void MainProcess(string[] args) {
            var arg = new dpz3.Console.Arguments(args);
            string path = arg["path"];
            // 判断路径是否指定
            if (path.IsNoneOrNull()) {
                Console.WriteLine($"[!] 未指定路径");
                return;
            }
            // 将路径进行处理
            if (!path.EndsWith(it.SplitChar)) path += it.SplitChar;
            // 判断页面文件夹
            string pathPages = $"{path}pages";
            if (!System.IO.Directory.Exists(pathPages)) {
                Console.WriteLine($"[!] 未找到pages文件夹");
                return;
            }
            // 判断并创建控制器工程文件夹
            string pathController = $"{path}controller{it.SplitChar}pages";
            if (!System.IO.Directory.Exists(pathController)) {
                Console.WriteLine($"[+] 创建文件夹 {pathController} ...");
                System.IO.Directory.CreateDirectory(pathController);
                return;
            }
            MakerController(pathPages, pathController, "pages", "");
        }

        static void Main(string[] args) {
            it.Initialize();
            MainProcess(args);
            //Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
