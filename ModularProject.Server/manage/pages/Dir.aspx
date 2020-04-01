<%@ Page Language="C#" %>

<%@ Import Namespace="dpz3" %>

<%
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
    <script src="/modular-project-manage/dir/page.js"></script>
    <link rel="stylesheet" href="/modular-project-manage/dir/page.css" />
</head>
<body>
    <div class="info" style="display: none;">正在处理,请稍等...</div>
    <%if (item != null) { %>
    <%
        // 获取路径段
        string[] paths = path.Split('/');
        // 获取工作目录
        string workFolder = item.Attr["path"];
        string workFolderXorm = $"{workFolder}/xorm";
        string workFolderModular = $"{workFolder}/modular.json";
    %>
    <div class="list">
        <dl>
            <dd>
                <s><a href="/modular-project-manage/index?group=<%=groupName%>&item=<%=itemName%>">返回</a></s>&nbsp;
                <i><a href="/modular-project-manage/dir?group=<%=groupName%>&item=<%=itemName%>&path=/">根目录</a></i>
                <%string pathPart = ""; %>
                <%for (int i = 1; i < paths.Length; i++) {%>
                <%pathPart += $"/{paths[i]}"; %>
                <i>/</i>
                <i><a href="/modular-project-manage/dir?group=<%=groupName%>&item=<%=itemName%>&path=<%=pathPart%>"><%=paths[i]%></a></i>
                <%} %>
            </dd>
            <%
                // 获取所有文件夹
                string[] dirs = System.IO.Directory.GetDirectories($"{workFolder}{path}");
            %>
            <%for (int i = 0; i < dirs.Length; i++) {%>
            <%string fileName = System.IO.Path.GetFileName(dirs[i]); %>
            <dd>
                <img src="/modular-project-manage/dir/image/folder.png" /><i><a href="/modular-project-manage/dir?group=<%=groupName%>&item=<%=itemName%>&path=<%=path%>/<%=fileName%>"><%=fileName%></a></i></dd>
            <%} %>
            <%
                // 获取所有文件
                string[] files = System.IO.Directory.GetFiles($"{workFolder}{path}");
            %>
            <%for (int i = 0; i < files.Length; i++) {%>
            <%string fileName = System.IO.Path.GetFileName(files[i]); %>
            <%if (fileName == "config.json") { %>
            <dd>
                <img src="/modular-project-manage/dir/image/file.png" /><i><%=fileName%></i>&nbsp;|&nbsp;<i><a href="javascript:;" onclick="pg.buildMainPage('<%=groupName%>','<%=itemName%>','<%=path%>');">构建页面</a></i></dd>
            <%} else { %>
            <dd>
                <img src="/modular-project-manage/dir/image/file.png" /><i><%=fileName%></i></dd>
            <%} %>
            <%} %>
        </dl>
    </div>
    <%} else { %>
    <div>未发现项目信息</div>
    <%}%>
</body>
</html>
