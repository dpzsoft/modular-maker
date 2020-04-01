<%@ Page Language="C#" %>

<%@ Import Namespace="dpz3" %>

<%
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
%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>模块化网站项目管理器</title>
    <link rel="icon" href="/modular-project-common/manager.ico" type="image/x-icon" />
    <script src="/modular-project-common/js/jquery-3.4.1.min.js"></script>
    <script src="/modular-project-common/js/vue.js"></script>
    <script src="/modular-project-common/js/dpz2.js"></script>
    <script src="/modular-project-common/js/host.js"></script>
    <script src="/modular-project-common/js/jttp.js"></script>
    <script src="/modular-project-manage/index/page.js"></script>
    <link rel="stylesheet" href="/modular-project-manage/index/page.css" />
</head>
<body>
    <div class="info" style="display: none;">正在处理,请稍等...</div>
    <%if (item != null) { %>
    <%
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
    %>
    <div class="list">
        <dl>
            <dt>包信息</dt>
            <dd>名称：<%=package.Str["name"]%></dd>
            <dd>版本：<%=package.Str["version"]%></dd>
            <dd>描述：<%=package.Str["description"]%></dd>
            <dt>快捷操作</dt>
            <dd>
                <%if (System.IO.File.Exists($"{workFolder}/pages/pages.sln")) {%>
                <s><a href="javascript:;" onclick="pg.buildPages('<%=groupName%>','<%=itemName%>');">生成页面控制器</a></s>&nbsp;
                <%} %>
                <%if (System.IO.File.Exists($"{workFolder}/controller/controller.sln")) {%>
                <s><a href="javascript:;" onclick="pg.buildController('<%=groupName%>','<%=itemName%>');">编译控制器</a></s>&nbsp;
                <%} %>
                <s><a href="javascript:;" onclick="pg.buildPackage('<%=groupName%>','<%=itemName%>');">生成MP包</a></s>&nbsp;
            </dd>
            <dd>
                <%if (System.IO.File.Exists($"{workFolder}/pages/pages.sln")) {%>
                <s><a href="javascript:;" onclick="pg.openPages('<%=groupName%>','<%=itemName%>');">使用VS编辑Pages</a></s>&nbsp;
                <%} %>
                <%if (System.IO.File.Exists($"{workFolder}/controller/controller.sln")) {%>
                <s><a href="javascript:;" onclick="pg.openController('<%=groupName%>','<%=itemName%>');">使用VS编辑Controller</a></s>&nbsp;
                <%} %>
                <s><a href="/modular-project-manage/dir?group=<%=groupName%>&item=<%=itemName%>&path=/">浏览文件</a></s>&nbsp;
            </dd>
            <dt>对象映射信息</dt>
            <%if (files.Length > 0) { %>
            <%foreach (var file in files) { %>
            <%string fileContent = dpz3.File.UTF8File.ReadAllText(file);%>
            <%using (var fileDoc = dpz3.Xml.Parser.GetDocument(fileContent)) {%>
            <%var table = fileDoc["table"];%>
            <%if (table != null) { %>
            <dd><i><a href="/modular-project-manage/Xorm?group=<%=groupName%>&item=<%=itemName%>&name=<%=table.Attr["name"]%>"><%=table.Attr["title"]%>&nbsp;[<%=table.Attr["name"]%>:<%=table.Attr["version"]%>]</a></i></dd>
            <%} else { %>
            <dd>文件<%=file%>读取错误</dd>
            <%} %>
            <%} %>
            <%} %>
            <%} else { %>
            <dd>未发现对象映射信息</dd>
            <%}%>
        </dl>
    </div>
    <%} else { %>
    <div>未发现项目信息</div>
    <%}%>
</body>
</html>
