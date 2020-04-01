using System;
using System.Text;
using dpz3;
using dpz3.Modular;

namespace pages {

    [Modular(ModularTypes.Session, "")]
    public class _Page : SessionControllerBase {

        [Modular(ModularTypes.Get, "Dir")]
public IResult Dir() {
    StringBuilder sb = new StringBuilder();
    dpz3.db.Connection dbc = Host.Connection;
    var session = Host.Session;
    var request = Host.Context.Request;
    var response = Host.Context.Response;
    string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
    // 获取参数
    string groupName = $"{request.Query["group"]}";
    string itemName = $"{request.Query["item"]}";
    string path = $"{request.Query["path"]}";
    path = path.Replace("//", "/");
    if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
    string pathXml = $"{hostWorkFolder}conf/projects.xml";
    string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
    dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
    var xml = doc["xml"];
    dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
    dpz3.Xml.XmlNode item = null;
    if (group != null) item = group.GetNodeByAttr("name", itemName, false);
    sb.Append("<!DOCTYPE html>\n");
    sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">\n");
    sb.Append("<head>\n");
    sb.Append("<meta charset=\"utf-8\" />\n");
    sb.Append("<title>模块化网站项目管理器</title>\n");
    sb.Append("<link rel=\"icon\" href=\"/modular-project-common/manager.ico\" type=\"image/x-icon\" />\n");
    sb.Append("<script src=\"/modular-project-common/js/jquery-3.4.1.min.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/vue.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/dpz2.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/host.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/jttp.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-manage/dir/page.js\"></script>\n");
    sb.Append("<link rel=\"stylesheet\" href=\"/modular-project-manage/dir/page.css\" />\n");
    sb.Append("</head>\n");
    sb.Append("<body>\n");
    sb.Append("<div class=\"info\" style=\"display: none;\">正在处理,请稍等...</div>\n");
    if (item != null) { 
    // 获取路径段
    string[] paths = path.Split('/');
    // 获取工作目录
    string workFolder = item.Attr["path"];
    string workFolderXorm = $"{workFolder}/xorm";
    string workFolderModular = $"{workFolder}/modular.json";
    sb.Append("<div class=\"list\">\n");
    sb.Append("<dl>\n");
    sb.Append("<dd>\n");
    sb.Append("<s><a href=\"/modular-project-manage/index?group=");
    sb.Append(groupName);
    sb.Append("&item=");
    sb.Append(itemName);
    sb.Append("\">返回</a></s>&nbsp;\n");
    sb.Append("<i><a href=\"/modular-project-manage/dir?group=");
    sb.Append(groupName);
    sb.Append("&item=");
    sb.Append(itemName);
    sb.Append("&path=/\">根目录</a></i>\n");
    string pathPart = ""; 
    for (int i = 1; i < paths.Length; i++) {
    pathPart += $"/{paths[i]}"; 
    sb.Append("<i>/</i>\n");
    sb.Append("<i><a href=\"/modular-project-manage/dir?group=");
    sb.Append(groupName);
    sb.Append("&item=");
    sb.Append(itemName);
    sb.Append("&path=");
    sb.Append(pathPart);
    sb.Append("\">");
    sb.Append(paths[i]);
    sb.Append("</a></i>\n");
    } 
    sb.Append("</dd>\n");
    // 获取所有文件夹
    string[] dirs = System.IO.Directory.GetDirectories($"{workFolder}{path}");
    for (int i = 0; i < dirs.Length; i++) {
    string fileName = System.IO.Path.GetFileName(dirs[i]); 
    sb.Append("<dd>\n");
    sb.Append("<img src=\"/modular-project-manage/dir/image/folder.png\" /><i><a href=\"/modular-project-manage/dir?group=");
    sb.Append(groupName);
    sb.Append("&item=");
    sb.Append(itemName);
    sb.Append("&path=");
    sb.Append(path);
    sb.Append("/");
    sb.Append(fileName);
    sb.Append("\">");
    sb.Append(fileName);
    sb.Append("</a></i></dd>\n");
    } 
    // 获取所有文件
    string[] files = System.IO.Directory.GetFiles($"{workFolder}{path}");
    for (int i = 0; i < files.Length; i++) {
    string fileName = System.IO.Path.GetFileName(files[i]); 
    if (fileName == "config.json") { 
    sb.Append("<dd>\n");
    sb.Append("<img src=\"/modular-project-manage/dir/image/file.png\" /><i>");
    sb.Append(fileName);
    sb.Append("</i>&nbsp;|&nbsp;<i><a href=\"javascript:;\" onclick=\"pg.buildMainPage('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("','");
    sb.Append(path);
    sb.Append("');\">构建页面</a></i></dd>\n");
    } else { 
    sb.Append("<dd>\n");
    sb.Append("<img src=\"/modular-project-manage/dir/image/file.png\" /><i>");
    sb.Append(fileName);
    sb.Append("</i></dd>\n");
    } 
    } 
    sb.Append("</dl>\n");
    sb.Append("</div>\n");
    } else { 
    sb.Append("<div>未发现项目信息</div>\n");
    }
    sb.Append("</body>\n");
    sb.Append("</html>\n");
    return Html(sb.ToString());
}

        [Modular(ModularTypes.Get, "index")]
public IResult index() {
    StringBuilder sb = new StringBuilder();
    dpz3.db.Connection dbc = Host.Connection;
    var session = Host.Session;
    var request = Host.Context.Request;
    var response = Host.Context.Response;
    string path = Host.WorkFolder.Replace("\\", "/");
    // 获取参数
    string groupName = $"{request.Query["group"]}";
    string itemName = $"{request.Query["item"]}";
    if (!path.EndsWith("/")) path += "/";
    string pathXml = $"{path}conf/projects.xml";
    string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
    dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
    var xml = doc["xml"];
    dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
    dpz3.Xml.XmlNode item = null;
    if (group != null) item = group.GetNodeByAttr("name", itemName, false);
    sb.Append("<!DOCTYPE html>\n");
    sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">\n");
    sb.Append("<head>\n");
    sb.Append("<meta charset=\"utf-8\" />\n");
    sb.Append("<title>模块化网站项目管理器</title>\n");
    sb.Append("<link rel=\"icon\" href=\"/modular-project-common/manager.ico\" type=\"image/x-icon\" />\n");
    sb.Append("<script src=\"/modular-project-common/js/jquery-3.4.1.min.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/vue.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/dpz2.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/host.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/jttp.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-manage/index/page.js\"></script>\n");
    sb.Append("<link rel=\"stylesheet\" href=\"/modular-project-manage/index/page.css\" />\n");
    sb.Append("</head>\n");
    sb.Append("<body>\n");
    sb.Append("<div class=\"info\" style=\"display: none;\">正在处理,请稍等...</div>\n");
    if (item != null) { 
    string workFolder = item.Attr["path"];
    string workFolderXorm = $"{workFolder}/xorm";
    string workFolderModular = $"{workFolder}/modular.json";
    // 读取包信息
    string content = dpz3.File.UTF8File.ReadAllText(workFolderModular);
    var package = dpz3.Json.Parser.ParseJson(content);
    // 读取所有的配置文件
    string[] files = null;
    if (System.IO.Directory.Exists(workFolderXorm)) {
    files = System.IO.Directory.GetFiles(workFolderXorm, "*.xml");
    } else {
    files = new string[0];
    }
    sb.Append("<div class=\"list\">\n");
    sb.Append("<dl>\n");
    sb.Append("<dt>包信息</dt>\n");
    sb.Append("<dd>名称：");
    sb.Append(package.Str["name"]);
    sb.Append("</dd>\n");
    sb.Append("<dd>版本：");
    sb.Append(package.Str["version"]);
    sb.Append("</dd>\n");
    sb.Append("<dd>描述：");
    sb.Append(package.Str["description"]);
    sb.Append("</dd>\n");
    sb.Append("<dt>快捷操作</dt>\n");
    sb.Append("<dd>\n");
    if (System.IO.File.Exists($"{workFolder}/pages/pages.sln")) {
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildPages('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("');\">生成页面控制器</a></s>&nbsp;\n");
    } 
    if (System.IO.File.Exists($"{workFolder}/controller/controller.sln")) {
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildController('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("');\">编译控制器</a></s>&nbsp;\n");
    } 
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildPackage('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("');\">生成MP包</a></s>&nbsp;\n");
    sb.Append("</dd>\n");
    sb.Append("<dd>\n");
    if (System.IO.File.Exists($"{workFolder}/pages/pages.sln")) {
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.openPages('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("');\">使用VS编辑Pages</a></s>&nbsp;\n");
    } 
    if (System.IO.File.Exists($"{workFolder}/controller/controller.sln")) {
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.openController('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("');\">使用VS编辑Controller</a></s>&nbsp;\n");
    } 
    sb.Append("<s><a href=\"/modular-project-manage/dir?group=");
    sb.Append(groupName);
    sb.Append("&item=");
    sb.Append(itemName);
    sb.Append("&path=/\">浏览文件</a></s>&nbsp;\n");
    sb.Append("</dd>\n");
    sb.Append("<dt>对象映射信息</dt>\n");
    if (files.Length > 0) { 
    foreach (var file in files) { 
    string fileContent = dpz3.File.UTF8File.ReadAllText(file);
    using (var fileDoc = dpz3.Xml.Parser.GetDocument(fileContent)) {
    var table = fileDoc["table"];
    if (table != null) { 
    sb.Append("<dd><i><a href=\"/modular-project-manage/Xorm?group=");
    sb.Append(groupName);
    sb.Append("&item=");
    sb.Append(itemName);
    sb.Append("&name=");
    sb.Append(table.Attr["name"]);
    sb.Append("\">");
    sb.Append(table.Attr["title"]);
    sb.Append("&nbsp;[");
    sb.Append(table.Attr["name"]);
    sb.Append(":");
    sb.Append(table.Attr["version"]);
    sb.Append("]</a></i></dd>\n");
    } else { 
    sb.Append("<dd>文件");
    sb.Append(file);
    sb.Append("读取错误</dd>\n");
    } 
    } 
    } 
    } else { 
    sb.Append("<dd>未发现对象映射信息</dd>\n");
    }
    sb.Append("</dl>\n");
    sb.Append("</div>\n");
    } else { 
    sb.Append("<div>未发现项目信息</div>\n");
    }
    sb.Append("</body>\n");
    sb.Append("</html>\n");
    return Html(sb.ToString());
}

        [Modular(ModularTypes.Get, "Xorm")]
public IResult Xorm() {
    StringBuilder sb = new StringBuilder();
    dpz3.db.Connection dbc = Host.Connection;
    var session = Host.Session;
    var request = Host.Context.Request;
    var response = Host.Context.Response;
    string hostWorkFolder = Host.WorkFolder.Replace("\\", "/");
    // 获取参数
    string groupName = $"{request.Query["group"]}";
    string itemName = $"{request.Query["item"]}";
    string name = $"{request.Query["name"]}";
    if (!hostWorkFolder.EndsWith("/")) hostWorkFolder += "/";
    string pathXml = $"{hostWorkFolder}conf/projects.xml";
    string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
    dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
    var xml = doc["xml"];
    dpz3.Xml.XmlNode group = xml.GetNodeByAttr("name", groupName, false);
    dpz3.Xml.XmlNode item = null;
    if (group != null) item = group.GetNodeByAttr("name", itemName, false);
    sb.Append("<!DOCTYPE html>\n");
    sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">\n");
    sb.Append("<head>\n");
    sb.Append("<meta charset=\"utf-8\" />\n");
    sb.Append("<title>模块化网站项目管理器</title>\n");
    sb.Append("<link rel=\"icon\" href=\"/modular-project-common/manager.ico\" type=\"image/x-icon\" />\n");
    sb.Append("<script src=\"/modular-project-common/js/jquery-3.4.1.min.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/vue.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/dpz2.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/host.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-common/js/jttp.js\"></script>\n");
    sb.Append("<script src=\"/modular-project-manage/xorm/page.js\"></script>\n");
    sb.Append("<link rel=\"stylesheet\" href=\"/modular-project-manage/xorm/page.css\" />\n");
    sb.Append("</head>\n");
    sb.Append("<body>\n");
    sb.Append("<div class=\"info\" style=\"display: none;\">正在处理,请稍等...</div>\n");
    if (item != null) { 
    // 获取工作目录
    string workFolder = item.Attr["path"];
    string workFolderXorm = $"{workFolder}/xorm";
    string pathXorm = $"{workFolderXorm}/{name}.xml";
    string folderTemplete = $"{hostWorkFolder}template/{groupName}";
    if (!System.IO.Directory.Exists(folderTemplete)) System.IO.Directory.CreateDirectory(folderTemplete);
    string fileContent = dpz3.File.UTF8File.ReadAllText(pathXorm);
    using (var fileDoc = dpz3.Xml.Parser.GetDocument(fileContent)) {
    var table = fileDoc["table"];
    var fields = table.GetNodesByTagName("field", false);
    sb.Append("<div class=\"list\">\n");
    sb.Append("<dl>\n");
    sb.Append("<dt>相关信息</dt>\n");
    sb.Append("<dd>名称：");
    sb.Append(table.Attr["name"]);
    sb.Append("</dd>\n");
    sb.Append("<dd>标题：");
    sb.Append(table.Attr["title"]);
    sb.Append("</dd>\n");
    sb.Append("<dd>版本：");
    sb.Append(table.Attr["version"]);
    sb.Append("</dd>\n");
    sb.Append("<dt>快捷操作</dt>\n");
    sb.Append("<dd>\n");
    sb.Append("<s><a href=\"/modular-project-manage/index?group=");
    sb.Append(groupName);
    sb.Append("&item=");
    sb.Append(itemName);
    sb.Append("\">返回主界面</a></s>&nbsp;\n");
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildController('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("','");
    sb.Append(name);
    sb.Append("');\">创建类构造代码</a></s>&nbsp;\n");
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildList('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("','");
    sb.Append(name);
    sb.Append("');\">创建列表代码</a></s>&nbsp;\n");
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildSelector('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("','");
    sb.Append(name);
    sb.Append("');\">创建选择器代码</a></s>&nbsp;\n");
    sb.Append("</dd>\n");
    sb.Append("<dd>\n");
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildAddForm('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("','");
    sb.Append(name);
    sb.Append("');\">创建添加对话框代码</a></s>&nbsp;\n");
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildEditForm('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("','");
    sb.Append(name);
    sb.Append("');\">创建编辑对话框代码</a></s>&nbsp;\n");
    sb.Append("<s><a href=\"javascript:;\" onclick=\"pg.buildViewForm('");
    sb.Append(groupName);
    sb.Append("','");
    sb.Append(itemName);
    sb.Append("','");
    sb.Append(name);
    sb.Append("');\">创建视图对话框代码</a></s>&nbsp;\n");
    sb.Append("</dd>\n");
    sb.Append("<dt>字段一览</dt>\n");
    foreach (var field in fields) { 
    var data = field["data"]; 
    sb.Append("<dd>");
    sb.Append(field.Attr["title"]);
    sb.Append("(");
    sb.Append(field.Attr["name"]);
    sb.Append(") - ");
    sb.Append(data.Attr["type"]);
    sb.Append("(");
    sb.Append(data.Attr["size"]);
    sb.Append(",");
    sb.Append(data.Attr["float"]);
    sb.Append(")</dd>\n");
    } 
    sb.Append("</dl>\n");
    sb.Append("</div>\n");
    }
    } else { 
    sb.Append("<div>未发现项目信息</div>\n");
    }
    sb.Append("</body>\n");
    sb.Append("</html>\n");
    return Html(sb.ToString());
}

    }
}
