<%@ Page Language="C#" %>

<%@ Import Namespace="dpz3" %>

<%
    dpz3.db.Connection dbc = Host.Connection;
    var session = Host.Session;
    var response = Host.Context.Response;
    string path = Host.WorkFolder.Replace("\\", "/");
    if (!path.EndsWith("/")) path += "/";
    string pathXml = $"{path}conf/projects.xml";
    string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
    dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
    var xml = doc["xml"];
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
    <script src="/modular-project-main/index/page.js"></script>
    <link rel="stylesheet" href="/modular-project-main/index/page.css" />
</head>
<body>
    <div class="left">
        <div class="menu">
            <%var groups = xml.GetNodesByTagName("group", false);%>
            <%foreach (var group in groups) {%>
            <dl id="dl_<%=group.Attr["name"]%>_close" style="display: none;">
                <dt onclick="pg.open('<%=group.Attr["name"]%>');">
                    <s>
                        <img src="/modular-project-main/index/image/config.png" /></s>
                    <s><%=group.Attr["title"]%></s>
                    <s>
                        <img src="/modular-project-main/index/image/down2.png" /></s>
                </dt>
            </dl>
            <dl id="dl_<%=group.Attr["name"]%>_open">
                <dt onclick="pg.close('<%=group.Attr["name"]%>');">
                    <s>
                        <img src="/modular-project-main/index/image/config.png" /></s>
                    <s><%=group.Attr["title"]%></s>
                    <s>
                        <img src="/modular-project-main/index/image/down.png" /></s>
                </dt>
                <%var items = group.GetNodesByTagName("item", false);%>
                <%foreach (var item in items) {%>
                <dd id="dd_<%=group.Attr["name"]%>_<%=item.Attr["name"]%>" onclick="pg.navTo('<%=group.Attr["name"]%>_<%=item.Attr["name"]%>','/modular-project-manage/index?group=<%=group.Attr["name"]%>&item=<%=item.Attr["name"]%>');"><%=item.Attr["title"]%>(<%=item.Attr["name"]%>)</dd>
                <%} %>
            </dl>
            <%} %>
        </div>
    </div>
    <div class="right">
        <div class="header">
            <div id="switch_hide" class="switch">
                <a id="lnk_menu_hide" href="javascript:;">
                    <div class="switch_icon">
                        <img id="img_menu_hide" src="/modular-project-main/index/image/left.png" style="display: none;" />
                        <img id="img_menu_hide2" src="/modular-project-main/index/image/left_b.png" />
                    </div>
                    <div class="switch_title">收起菜单栏</div>
                    <div class="clear"></div>
                </a>
            </div>
            <div id="switch_show" class="switch" style="display: none;">
                <a id="lnk_menu_show" href="javascript:;">
                    <div class="switch_icon">
                        <img id="img_menu_show" src="/modular-project-main/index/image/right.png" style="display: none;" />
                        <img id="img_menu_show2" src="/modular-project-main/index/image/right_b.png" />
                    </div>
                    <div class="switch_title">显示菜单栏</div>
                    <div class="clear"></div>
                </a>
            </div>
            <div class="exit"></div>
            <div class="user"></div>
        </div>
        <div class="frame">
            <iframe id="frm_main" src="about:blank"></iframe>
        </div>
    </div>
    <script type="text/javascript">
        $(function () {

        });
    </script>
</body>
</html>
