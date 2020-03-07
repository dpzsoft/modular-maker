using System;
using System.Collections.Generic;
using System.IO.Compression;
using dpz3;

namespace mpack {
    class Program {

        // 复制文件夹
        private static void CopyFolder(string pathSource, string pathTarget) {

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

        // 安装或更新数据库
        private static void UpdateDatabase(dpz3.db.Connection dbc, dpz3.XOrm.StandaloneTable table) {
            // 定义表映射
            var Xorms = dpz3.db.OrmMapper.Table("Xorms");
            if (!dbc.CheckTable("Xorms")) {
                if (table.Name == "Xorms") {
                    Console.WriteLine($"[+] 安装数据表 {table.Name} ...");
                    // 生成字段定义集合
                    List<dpz3.db.SqlUnits.FieldDefine> list = new List<dpz3.db.SqlUnits.FieldDefine>();
                    foreach (var field in table.Fields) {
                        dpz3.db.SqlUnits.FieldDefine fieldDefine = new dpz3.db.SqlUnits.FieldDefine();
                        fieldDefine.Name = field.Name;
                        fieldDefine.Type = field.DataType;
                        fieldDefine.Size = field.DataSize;
                        fieldDefine.Float = field.DataFloat;
                        list.Add(fieldDefine);
                    }
                    // 添加表格
                    dbc.CreateTable(table.Name, list.ToArray());
                    // 更新缓存信息
                    var rowInsert = new dpz3.db.Row();
                    rowInsert["Name"] = table.Name;
                    rowInsert["Guid"] = Guid.NewGuid().ToString();
                    rowInsert["Version"] = table.Version;
                    rowInsert["Title"] = table.Title;
                    rowInsert["Description"] = table.Description;
                    dbc.Insert(Xorms, rowInsert);
                } else {
                    Console.WriteLine($"[!] 尚未找到Xorms表，请先安装xorm-basic包。");
                    return;
                }
            } else {
                // 判断表信息是否存在
                var row = dbc.Select(Xorms).Where(Xorms["Name"] == table.Name).GetRow();
                string rowVersion = "";
                if (!row.IsEmpty) rowVersion = row["Version"];
                if (rowVersion != table.Version) {
                    // 判断表是否存在
                    if (dbc.CheckTable(table.Name)) {
                        Console.WriteLine($"[+] 更新数据表 {table.Name} ...");
                        // 生成字段定义集合
                        foreach (var field in table.Fields) {
                            dpz3.db.SqlUnits.FieldDefine fieldDefine = new dpz3.db.SqlUnits.FieldDefine();
                            fieldDefine.Name = field.Name;
                            fieldDefine.Type = field.DataType;
                            fieldDefine.Size = field.DataSize;
                            fieldDefine.Float = field.DataFloat;
                            // 判断字段是否存在
                            if (dbc.CheckTableFiled(table.Name, field.Name)) {
                                // 更新字段
                                dbc.UpdateTableFiled(table.Name, field.Name, fieldDefine);
                            } else {
                                // 添加字段
                                dbc.AddTableFiled(table.Name, fieldDefine);
                            }
                        }
                    } else {
                        Console.WriteLine($"[+] 创建数据表 {table.Name} ...");
                        // 生成字段定义集合
                        List<dpz3.db.SqlUnits.FieldDefine> list = new List<dpz3.db.SqlUnits.FieldDefine>();
                        foreach (var field in table.Fields) {
                            dpz3.db.SqlUnits.FieldDefine fieldDefine = new dpz3.db.SqlUnits.FieldDefine();
                            fieldDefine.Name = field.Name;
                            fieldDefine.Type = field.DataType;
                            fieldDefine.Size = field.DataSize;
                            fieldDefine.Float = field.DataFloat;
                            list.Add(fieldDefine);
                        }
                        // 添加表格
                        dbc.CreateTable(table.Name, list.ToArray());
                    }
                    // 操作表缓存信息
                    if (row.IsEmpty) {
                        // 添加表缓存信息
                        var rowInsert = new dpz3.db.Row();
                        rowInsert["Name"] = table.Name;
                        rowInsert["Guid"] = Guid.NewGuid().ToString();
                        rowInsert["Version"] = table.Version;
                        rowInsert["Title"] = table.Title;
                        rowInsert["Description"] = table.Description;
                        dbc.Insert(Xorms, rowInsert).Exec();
                    } else {
                        // 更新表缓存信息
                        var rowUpdate = new dpz3.db.Row();
                        rowUpdate["Version"] = table.Version;
                        rowUpdate["Title"] = table.Title;
                        rowUpdate["Description"] = table.Description;
                        dbc.Update(Xorms, rowUpdate).Where(Xorms["ID"] == row["ID"]).Exec();
                    }
                }
            }
        }

        // 安装包
        private static void InstallPack(string path, string name, string version, string url = null) {

            // 检测参数完整性
            if (name.IsNoneOrNull()) {
                Console.WriteLine("[!] 缺少包名称");
                return;
            }
            if (version.IsNoneOrNull()) {
                Console.WriteLine("[!] 缺少包版本");
                return;
            }

            Console.WriteLine("[*] 初始化包管理 ...");

            string folderRoot = $"{path}wwwroot";
            string folderDown = $"{path}downloads";
            string folderPackage = $"{path}packages";
            if (!System.IO.Directory.Exists(folderRoot)) System.IO.Directory.CreateDirectory(folderRoot);
            if (!System.IO.Directory.Exists(folderDown)) System.IO.Directory.CreateDirectory(folderDown);
            if (!System.IO.Directory.Exists(folderPackage)) System.IO.Directory.CreateDirectory(folderPackage);

            // 判断并创建配置文件
            string fileXml = $"{folderPackage}{it.SplitChar}packages.xml";
            if (!System.IO.File.Exists(fileXml)) {
                Console.WriteLine("[*] 正在创建包管理文件 ...");
                using (var doc = new dpz3.Xml.XmlDocument()) {
                    var xml = new dpz3.Xml.XmlNode("xml");
                    doc.Nodes.Add(xml);
                    var packages = xml.AddNode("packages");
                    dpz3.File.UTF8File.WriteAllText(fileXml, doc.InnerXml);
                }
            }

            // 读取配置文件
            string szXml = dpz3.File.UTF8File.ReadAllText(fileXml);
            bool isNew = true;
            bool isUpdate = false;
            using (var doc = dpz3.Xml.Parser.GetDocument(szXml)) {
                var xml = doc["xml"];
                var packages = xml["packages"];
                foreach (var package in packages.GetNodesByTagName("package", false)) {
                    string packageName = package.Attr["name"];
                    if (packageName == name) {
                        isNew = false;
                        //string packageDownload = package.Attr["download"];
                        string packageInstall = package.Attr["version"];
                        string folderInstall = $"{folderPackage}{it.SplitChar}{packageName}{it.SplitChar}{packageInstall}";
                        Console.WriteLine($"[*] 读取包版本 {packageName} 待安装版本:{version} 安装版本:{packageInstall}");
                        if (packageInstall != name) {
                            // 进行包的解压
                            Console.WriteLine($"[+] 包安装 {packageName} 版本:{version} ...");
                            string fileDown = $"{folderDown}{it.SplitChar}{packageName}-{version}.zip";
                            string folderInstallBefore = folderInstall;
                            folderInstall = $"{folderPackage}{it.SplitChar}{packageName}{it.SplitChar}{version}";
                            if (!System.IO.Directory.Exists(folderDown)) System.IO.Directory.CreateDirectory(folderDown);
                            ZipFile.ExtractToDirectory(fileDown, folderInstall, true);
                            //// 进行包的安装
                            //string folderInstallRoot = $"{folderInstall}{it.SplitChar}wwwroot";
                            //if (System.IO.Directory.Exists(folderInstallRoot)) {
                            //    // 进行包内静态文件的复制
                            //    Console.WriteLine("[*] 正在复制 wwwroot 文件夹 ...");
                            //    CopyFolder(folderInstallRoot, folderRoot);
                            //}
                            // 进行Xorm数据库安装更新
                            string folderInstallXorm = $"{folderInstall}{it.SplitChar}xorm";
                            if (System.IO.Directory.Exists(folderInstallXorm)) {
                                // 进行包内静态文件的复制
                                Console.WriteLine("[*] 正在进行数据库升级 ...");
                                string folderConf = $"{path}conf";
                                using (var db = dpz3.db.Database.LoadFromConf($"{folderConf}{it.SplitChar}db.cfg", "entity")) {
                                    using (var dbc = new dpz3.db.Connection(db)) {
                                        string[] fileXmls = System.IO.Directory.GetFiles(folderInstallXorm, "*.xml");
                                        foreach (var file in fileXmls) {
                                            Console.WriteLine($"[*] 正在检测数据库配置文件 {file} ...");
                                            string txtXml = dpz3.File.UTF8File.ReadAllText(file, false);
                                            using (var table = new dpz3.XOrm.StandaloneTable(txtXml)) {
                                                UpdateDatabase(dbc, table);
                                            }
                                        }
                                    }
                                }
                            }
                            try {
                                // 判断是否为重新安装
                                if (folderInstall != folderInstallBefore) {
                                    // 判断之前安装目录是否存在
                                    if (System.IO.Directory.Exists(folderInstallBefore)) {
                                        // 删除已安装目录
                                        Console.WriteLine($"[-] 清理已安装目录 {folderInstallBefore} ...");
                                        System.IO.Directory.Delete(folderInstallBefore, true);
                                    }
                                }
                            } catch (Exception ex) {
                                Console.WriteLine($"[!] 清理发生异常 {ex.Message}");
                            }
                            try {
                                // 判断安装文件是否存在
                                if (System.IO.File.Exists(fileDown)) {
                                    // 删除安装文件
                                    Console.WriteLine($"[-] 清理安装文件 {fileDown} ...");
                                    System.IO.File.Delete(fileDown);
                                }
                            } catch (Exception ex) {
                                Console.WriteLine($"[!] 清理发生异常 {ex.Message}");
                            }
                            // 更新版本号
                            package.Attr["version"] = version;
                            isUpdate = true;
                        }
                    }
                }
                // 当包信息不存在时，新增包信息
                if (isNew) {
                    // 建立新的节点
                    var package = packages.AddNode("package");
                    package.Attr["name"] = name;
                    package.Attr["version"] = version;
                    string folderInstall = $"{folderPackage}{it.SplitChar}{name}{it.SplitChar}{version}";
                    // 进行包的解压
                    Console.WriteLine($"[+] 包安装 {name} 版本:{version} ...");
                    string fileDown = $"{folderDown}{it.SplitChar}{name}-{version}.zip";
                    folderInstall = $"{folderPackage}{it.SplitChar}{name}{it.SplitChar}{version}";
                    if (!System.IO.Directory.Exists(folderDown)) System.IO.Directory.CreateDirectory(folderDown);
                    // 加压到指定文件夹
                    ZipFile.ExtractToDirectory(fileDown, folderInstall, true);
                    // 进行包的安装
                    // string folderInstallRoot = $"{folderInstall}{it.SplitChar}wwwroot";
                    //if (System.IO.Directory.Exists(folderInstallRoot)) {
                    //    // 进行包内静态文件的复制
                    //    // CopyFolder(folderInstallRoot, folderRoot);
                    //}
                    // 进行Xorm数据库安装更新
                    string folderInstallXorm = $"{folderInstall}{it.SplitChar}xorm";
                    if (System.IO.Directory.Exists(folderInstallXorm)) {
                        // 进行包内静态文件的复制
                        Console.WriteLine("[*] 正在进行数据库升级 ...");
                        string folderConf = $"{path}conf";
                        using (var db = dpz3.db.Database.LoadFromConf($"{folderConf}{it.SplitChar}db.cfg", "entity")) {
                            using (var dbc = new dpz3.db.Connection(db)) {
                                string[] fileXmls = System.IO.Directory.GetFiles(folderInstallXorm, "*.xml");
                                foreach (var file in fileXmls) {
                                    Console.WriteLine($"[*] 正在检测数据库配置文件 {file} ...");
                                    string txtXml = dpz3.File.UTF8File.ReadAllText(file, false);
                                    using (var table = new dpz3.XOrm.StandaloneTable(txtXml)) {
                                        UpdateDatabase(dbc, table);
                                    }
                                }
                            }
                        }
                    }
                    try {
                        // 判断安装文件是否存在
                        if (System.IO.File.Exists(fileDown)) {
                            // 删除安装文件
                            Console.WriteLine($"[-] 清理安装文件 {fileDown} ...");
                            System.IO.File.Delete(fileDown);
                        }
                    } catch(Exception ex) {
                        Console.WriteLine($"[!] 清理发生异常 {ex.Message}");
                    }
                    isUpdate = true;
                }
                // 保存配置
                if (isUpdate) dpz3.File.UTF8File.WriteAllText(fileXml, doc.InnerXml);
            }
        }

        // 卸载包
        private static void UninstallPack(string path, string name) {

            // 检测参数完整性
            if (name.IsNoneOrNull()) {
                Console.WriteLine("[!] 缺少包名称");
                return;
            }

            Console.WriteLine("[*] 初始化包管理 ...");

            string folderRoot = $"{path}wwwroot";
            string folderPackage = $"{path}packages";
            if (!System.IO.Directory.Exists(folderRoot)) System.IO.Directory.CreateDirectory(folderRoot);
            if (!System.IO.Directory.Exists(folderPackage)) System.IO.Directory.CreateDirectory(folderPackage);

            // 判断并创建配置文件
            string fileXml = $"{folderPackage}{it.SplitChar}packages.xml";
            if (!System.IO.File.Exists(fileXml)) {
                Console.WriteLine("[*] 正在创建包管理文件 ...");
                using (var doc = new dpz3.Xml.XmlDocument()) {
                    var xml = new dpz3.Xml.XmlNode("xml");
                    doc.Nodes.Add(xml);
                    var packages = xml.AddNode("packages");
                    dpz3.File.UTF8File.WriteAllText(fileXml, doc.InnerXml);
                }
            }

            // 读取配置文件
            string szXml = dpz3.File.UTF8File.ReadAllText(fileXml);
            bool isUpdate = false;
            using (var doc = dpz3.Xml.Parser.GetDocument(szXml)) {
                var xml = doc["xml"];
                var packages = xml["packages"];
                foreach (var package in packages.GetNodesByTagName("package", false)) {
                    string packageName = package.Attr["name"];
                    if (packageName == name) {
                        string packageInstall = package.Attr["version"];
                        string folderInstall = $"{folderPackage}{it.SplitChar}{packageName}{it.SplitChar}{packageInstall}";
                        Console.WriteLine($"[*] 读取包版本 {packageName} 安装版本:{packageInstall}");
                        // 进行包的解压
                        Console.WriteLine($"[-] 包信息删除 {packageName} ...");
                        package.Parent.Nodes.Remove(package);
                        try {
                            // 判断安装目录是否存在
                            if (System.IO.Directory.Exists(folderInstall)) {
                                // 删除已安装目录
                                Console.WriteLine($"[-] 清理已安装目录 {folderInstall} ...");
                                System.IO.Directory.Delete(folderInstall, true);
                            }
                        } catch (Exception ex) {
                            Console.WriteLine($"[!] 清理发生异常 {ex.Message}");
                        }
                        isUpdate = true;
                        break;
                    }
                }
                // 保存配置
                if (isUpdate) dpz3.File.UTF8File.WriteAllText(fileXml, doc.InnerXml);
            }
        }

        // 卸载包
        private static void ListPacks(string path) {

            Console.WriteLine("[*] 初始化包管理 ...");

            string folderRoot = $"{path}wwwroot";
            string folderPackage = $"{path}packages";
            if (!System.IO.Directory.Exists(folderRoot)) System.IO.Directory.CreateDirectory(folderRoot);
            if (!System.IO.Directory.Exists(folderPackage)) System.IO.Directory.CreateDirectory(folderPackage);

            // 判断并创建配置文件
            string fileXml = $"{folderPackage}{it.SplitChar}packages.xml";
            if (!System.IO.File.Exists(fileXml)) {
                Console.WriteLine("[*] 正在创建包管理文件 ...");
                using (var doc = new dpz3.Xml.XmlDocument()) {
                    var xml = new dpz3.Xml.XmlNode("xml");
                    doc.Nodes.Add(xml);
                    var packages = xml.AddNode("packages");
                    dpz3.File.UTF8File.WriteAllText(fileXml, doc.InnerXml);
                }
            }

            // 读取配置文件
            string szXml = dpz3.File.UTF8File.ReadAllText(fileXml);
            Console.WriteLine($"[*] 读取包信息 ...");
            using (var doc = dpz3.Xml.Parser.GetDocument(szXml)) {
                var xml = doc["xml"];
                var packages = xml["packages"];
                foreach (var package in packages.GetNodesByTagName("package", false)) {
                    string packageName = package.Attr["name"];
                    string packageInstall = package.Attr["version"];
                    Console.WriteLine($"[*] 包 {packageName} 安装版本:{packageInstall}");
                }
            }
        }

        // 重建数据库
        private static void BuildDatabase(string path) {

            Console.WriteLine("[*] 初始化包管理 ...");

            string folderRoot = $"{path}wwwroot";
            string folderPackage = $"{path}packages";
            if (!System.IO.Directory.Exists(folderRoot)) System.IO.Directory.CreateDirectory(folderRoot);
            if (!System.IO.Directory.Exists(folderPackage)) System.IO.Directory.CreateDirectory(folderPackage);

            // 判断并创建配置文件
            string fileXml = $"{folderPackage}{it.SplitChar}packages.xml";
            if (!System.IO.File.Exists(fileXml)) {
                Console.WriteLine("[*] 正在创建包管理文件 ...");
                using (var doc = new dpz3.Xml.XmlDocument()) {
                    var xml = new dpz3.Xml.XmlNode("xml");
                    doc.Nodes.Add(xml);
                    var packages = xml.AddNode("packages");
                    dpz3.File.UTF8File.WriteAllText(fileXml, doc.InnerXml);
                }
            }

            // 读取配置文件
            string szXml = dpz3.File.UTF8File.ReadAllText(fileXml);
            Console.WriteLine($"[*] 读取包信息 ...");
            using (var doc = dpz3.Xml.Parser.GetDocument(szXml)) {
                var xml = doc["xml"];
                var packages = xml["packages"];
                foreach (var package in packages.GetNodesByTagName("package", false)) {
                    string packageName = package.Attr["name"];
                    string packageVersion = package.Attr["version"];
                    string folderInstall = $"{folderPackage}{it.SplitChar}{packageName}{it.SplitChar}{packageVersion}";
                    // 进行Xorm数据库安装更新
                    string folderInstallXorm = $"{folderInstall}{it.SplitChar}xorm";
                    if (System.IO.Directory.Exists(folderInstallXorm)) {
                        // 进行包内静态文件的复制
                        Console.WriteLine("[*] 正在进行数据库升级 ...");
                        string folderConf = $"{path}conf";
                        using (var db = dpz3.db.Database.LoadFromConf($"{folderConf}{it.SplitChar}db.cfg", "entity")) {
                            using (var dbc = new dpz3.db.Connection(db)) {
                                string[] fileXmls = System.IO.Directory.GetFiles(folderInstallXorm, "*.xml");
                                foreach (var file in fileXmls) {
                                    Console.WriteLine($"[*] 正在检测数据库配置文件 {file} ...");
                                    string txtXml = dpz3.File.UTF8File.ReadAllText(file, false);
                                    using (var table = new dpz3.XOrm.StandaloneTable(txtXml)) {
                                        UpdateDatabase(dbc, table);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 输出帮助
        private static void SayHelp() {
            Console.WriteLine();
            Console.WriteLine("/help /h      显示所有已安装包");
            Console.WriteLine("/list /l      列表模式显示所有已安装包");
            Console.WriteLine("/build /b     数据库对象重建模式");
            Console.WriteLine("/uninstall /u 卸载模式");
            Console.WriteLine("/mode <模式>  指定模式 list/build/install/uninstall");
            Console.WriteLine("/name         包名称");
            Console.WriteLine("/version      包版本");
            Console.WriteLine("/path         指定工作目录");
            Console.WriteLine();
        }

        static void MainProcess(string[] args) {
            var arg = new dpz3.Console.Arguments(args);
            // 帮助模式
            if (arg.ContainsKey("help") || arg.ContainsKey("h")) {
                SayHelp();
                return;
            }
            // 读取模式
            string mode = arg["mode"];
            if (arg.ContainsKey("list") || arg.ContainsKey("l")) {
                mode = "list";
            } else if (arg.ContainsKey("build") || arg.ContainsKey("b")) {
                mode = "build";
            } else if (arg.ContainsKey("uninstall") || arg.ContainsKey("u")) {
                mode = "uninstall";
            } else {
                mode = "install";
            }
            // 读取路径
            string path = arg["path"];
            // 判断路径是否指定
            if (path.IsNoneOrNull()) path = it.WorkPath;
            // 将路径进行处理
            if (!path.EndsWith(it.SplitChar)) path += it.SplitChar;
            Console.WriteLine($"[*] 工作目录 {path}");
            // 卸载模式
            switch (mode.ToLower()) {
                case "list":
                    ListPacks(path);
                    break;
                case "build":
                    BuildDatabase(path);
                    break;
                case "uninstall":
                    UninstallPack(path, arg["name"]);
                    break;
                default:
                    InstallPack(path, arg["name"], arg["version"], arg["url"]);
                    break;
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
