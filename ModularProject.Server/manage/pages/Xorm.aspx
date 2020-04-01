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
    string name = $"{request.Query["name"]}";
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
    <script src="/modular-project-manage/xorm/page.js"></script>
    <link rel="stylesheet" href="/modular-project-manage/xorm/page.css" />
</head>
<body>
    <div class="info" style="display: none;">正在处理,请稍等...</div>
    <%if (item != null) { %>
    <%
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
    %>
    <div class="list">
        <dl>
            <dt>相关信息</dt>
            <dd>名称：<%=table.Attr["name"]%></dd>
            <dd>标题：<%=table.Attr["title"]%></dd>
            <dd>版本：<%=table.Attr["version"]%></dd>
            <dt>快捷操作</dt>
            <dd>
                <s><a href="/modular-project-manage/index?group=<%=groupName%>&item=<%=itemName%>">返回主界面</a></s>&nbsp;
                <s><a href="javascript:;" onclick="pg.buildController('<%=groupName%>','<%=itemName%>','<%=name%>');">创建类构造代码</a></s>&nbsp;
                <s><a href="javascript:;" onclick="pg.buildList('<%=groupName%>','<%=itemName%>','<%=name%>');">创建列表代码</a></s>&nbsp;
                <s><a href="javascript:;" onclick="pg.buildSelector('<%=groupName%>','<%=itemName%>','<%=name%>');">创建选择器代码</a></s>&nbsp;
            </dd>
            <dd>
                <s><a href="javascript:;" onclick="pg.buildAddForm('<%=groupName%>','<%=itemName%>','<%=name%>');">创建添加对话框代码</a></s>&nbsp;
                <s><a href="javascript:;" onclick="pg.buildEditForm('<%=groupName%>','<%=itemName%>','<%=name%>');">创建编辑对话框代码</a></s>&nbsp;
                <s><a href="javascript:;" onclick="pg.buildViewForm('<%=groupName%>','<%=itemName%>','<%=name%>');">创建视图对话框代码</a></s>&nbsp;
            </dd>
            <dt>字段一览</dt>
            <%foreach (var field in fields) { %>
            <%var data = field["data"]; %>
            <dd><%=field.Attr["title"]%>(<%=field.Attr["name"]%>) - <%=data.Attr["type"]%>(<%=data.Attr["size"]%>,<%=data.Attr["float"]%>)</dd>
            <%} %>
        </dl>
    </div>
    <%
        }
    %>
    <%} else { %>
    <div>未发现项目信息</div>
    <%}%>
</body>
</html>
