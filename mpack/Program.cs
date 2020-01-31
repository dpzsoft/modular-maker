using System;
using System.IO.Compression;
using dpz3;

namespace mpack {
    class Program {

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
            string pathCfg = $"{path}modular.json";
            if (!System.IO.File.Exists(pathCfg)) {
                Console.WriteLine($"[!] 未找到配置文件");
                return;
            }
            // 读取配置文件
            Console.WriteLine($"[*] 读取配置文件 {pathCfg} ...");
            string szJson = dpz3.File.UTF8File.ReadAllText(pathCfg);
            string name;
            string version;
            using (var json = dpz3.Json.Parser.ParseJson(szJson)) {
                name = json.Str["name"];
                version = json.Str["version"];
                Console.WriteLine($"[*] 当前版本 {version}");
                var ver = new dpz3.TimeVersion(version);
                version = ver.GetNextVersion().ToString();
                Console.WriteLine($"[*] 新版本 {version}");
                json.Str["version"] = version;
                dpz3.File.UTF8File.WriteAllText(pathCfg, json.ToJsonString());
            }
            // 创建临时输出目录
            string pathOutput = $"{path}output";
            if (!System.IO.Directory.Exists(pathOutput)) {
                Console.WriteLine($"[+] 创建目录 {pathOutput} ...");
                System.IO.Directory.CreateDirectory(pathOutput);
            }
            // 复制配置文件
            string pathOutputCfg = $"{pathOutput}{it.SplitChar}modular.json";
            Console.WriteLine($"[+] 增加拷贝文件 {pathOutputCfg} ...");
            System.IO.File.Copy(pathCfg, pathOutputCfg, true);
            // 复制静态页面
            string pathRoot = $"{path}wwwroot";
            if (System.IO.Directory.Exists(pathRoot)) {
                string pathOutputRoot = $"{pathOutput}{it.SplitChar}wwwroot";
                if (!System.IO.Directory.Exists(pathOutputRoot)) {
                    Console.WriteLine($"[+] 创建目录 {pathOutputRoot} ...");
                    System.IO.Directory.CreateDirectory(pathOutputRoot);
                }
                // 复制需要打包的文件到临时文件夹中
                CopyFolder(pathRoot, pathOutputRoot);
            }
            // 复制数据库定义文件夹
            string pathXorm = $"{path}xorm";
            if (System.IO.Directory.Exists(pathXorm)) {
                string pathOutputXorm = $"{pathOutput}{it.SplitChar}xorm";
                if (!System.IO.Directory.Exists(pathOutputXorm)) {
                    Console.WriteLine($"[+] 创建目录 {pathOutputXorm} ...");
                    System.IO.Directory.CreateDirectory(pathOutputXorm);
                }
                // 复制需要打包的文件到临时文件夹中
                CopyFolder(pathXorm, pathOutputXorm);
            }
            // 复制控制器
            string pathController = $"{path}controller{it.SplitChar}bin{it.SplitChar}Debug{it.SplitChar}netcoreapp3.1";
            if (System.IO.Directory.Exists(pathController)) {
                // 复制需要打包的文件到临时文件夹中
                CopyFolder(pathController, pathOutput);
            }
            // 建立包文件夹
            string pathPackage = $"{path}package";
            if (!System.IO.Directory.Exists(pathPackage)) {
                Console.WriteLine($"[+] 创建目录 {pathPackage} ...");
                System.IO.Directory.CreateDirectory(pathPackage);
            }
            // 打包文件
            string pathPackageFile = $"{pathPackage}{it.SplitChar}{name}-{version}.zip";
            Console.WriteLine($"[+] 输出包文件 {pathPackageFile} ...");
            ZipFile.CreateFromDirectory(pathOutput, pathPackageFile);
            // 清理临时目录
            if (System.IO.Directory.Exists(pathOutput)) {
                Console.WriteLine($"[-] 清理目录 {pathOutput} ...");
                System.IO.Directory.Delete(pathOutput, true);
            }
        }

        static void Main(string[] args) {
            it.Initialize();
            MainProcess(args);
            //ZipFile.CreateFromDirectory(@"X:\Projects\modular\ecp\package\wwwroot", @"X:\Projects\modular\ecp\package\a.zip");
            //Console.WriteLine("Hello World!");
            //Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
