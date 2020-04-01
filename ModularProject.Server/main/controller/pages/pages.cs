using System;
using System.Text;
using dpz3;
using dpz3.Modular;

namespace pages {

    [Modular(ModularTypes.Session, "")]
    public class _Page : SessionControllerBase {

        [Modular(ModularTypes.Get, "index")]
public IResult index() {
    StringBuilder sb = new StringBuilder();
    dpz3.db.Connection dbc = Host.Connection;
    var session = Host.Session;
    var response = Host.Context.Response;
    string path = Host.WorkFolder.Replace("\\", "/");
    if (!path.EndsWith("/")) path += "/";
    string pathXml = $"{path}conf/projects.xml";
    string szXml = dpz3.File.UTF8File.ReadAllText(pathXml);
    dpz3.Xml.XmlDocument doc = dpz3.Xml.Parser.GetDocument(szXml);
    var xml = doc["xml"];
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
    sb.Append("<script src=\"/modular-project-main/index/page.js\"></script>\n");
    sb.Append("<link rel=\"stylesheet\" href=\"/modular-project-main/index/page.css\" />\n");
    sb.Append("</head>\n");
    sb.Append("<body>\n");
    sb.Append("<div class=\"left\">\n");
    sb.Append("<div class=\"menu\">\n");
    var groups = xml.GetNodesByTagName("group", false);
    foreach (var group in groups) {
    sb.Append("<dl id=\"dl_");
    sb.Append(group.Attr["name"]);
    sb.Append("_close\" style=\"display: none;\">\n");
    sb.Append("<dt onclick=\"pg.open('");
    sb.Append(group.Attr["name"]);
    sb.Append("');\">\n");
    sb.Append("<s>\n");
    sb.Append("<img src=\"/modular-project-main/index/image/config.png\" /></s>\n");
    sb.Append("<s>");
    sb.Append(group.Attr["title"]);
    sb.Append("</s>\n");
    sb.Append("<s>\n");
    sb.Append("<img src=\"/modular-project-main/index/image/down2.png\" /></s>\n");
    sb.Append("</dt>\n");
    sb.Append("</dl>\n");
    sb.Append("<dl id=\"dl_");
    sb.Append(group.Attr["name"]);
    sb.Append("_open\">\n");
    sb.Append("<dt onclick=\"pg.close('");
    sb.Append(group.Attr["name"]);
    sb.Append("');\">\n");
    sb.Append("<s>\n");
    sb.Append("<img src=\"/modular-project-main/index/image/config.png\" /></s>\n");
    sb.Append("<s>");
    sb.Append(group.Attr["title"]);
    sb.Append("</s>\n");
    sb.Append("<s>\n");
    sb.Append("<img src=\"/modular-project-main/index/image/down.png\" /></s>\n");
    sb.Append("</dt>\n");
    var items = group.GetNodesByTagName("item", false);
    foreach (var item in items) {
    sb.Append("<dd id=\"dd_");
    sb.Append(group.Attr["name"]);
    sb.Append("_");
    sb.Append(item.Attr["name"]);
    sb.Append("\" onclick=\"pg.navTo('");
    sb.Append(group.Attr["name"]);
    sb.Append("_");
    sb.Append(item.Attr["name"]);
    sb.Append("','/modular-project-manage/index?group=");
    sb.Append(group.Attr["name"]);
    sb.Append("&item=");
    sb.Append(item.Attr["name"]);
    sb.Append("');\">");
    sb.Append(item.Attr["title"]);
    sb.Append("(");
    sb.Append(item.Attr["name"]);
    sb.Append(")</dd>\n");
    } 
    sb.Append("</dl>\n");
    } 
    sb.Append("</div>\n");
    sb.Append("</div>\n");
    sb.Append("<div class=\"right\">\n");
    sb.Append("<div class=\"header\">\n");
    sb.Append("<div id=\"switch_hide\" class=\"switch\">\n");
    sb.Append("<a id=\"lnk_menu_hide\" href=\"javascript:;\">\n");
    sb.Append("<div class=\"switch_icon\">\n");
    sb.Append("<img id=\"img_menu_hide\" src=\"/modular-project-main/index/image/left.png\" style=\"display: none;\" />\n");
    sb.Append("<img id=\"img_menu_hide2\" src=\"/modular-project-main/index/image/left_b.png\" />\n");
    sb.Append("</div>\n");
    sb.Append("<div class=\"switch_title\">收起菜单栏</div>\n");
    sb.Append("<div class=\"clear\"></div>\n");
    sb.Append("</a>\n");
    sb.Append("</div>\n");
    sb.Append("<div id=\"switch_show\" class=\"switch\" style=\"display: none;\">\n");
    sb.Append("<a id=\"lnk_menu_show\" href=\"javascript:;\">\n");
    sb.Append("<div class=\"switch_icon\">\n");
    sb.Append("<img id=\"img_menu_show\" src=\"/modular-project-main/index/image/right.png\" style=\"display: none;\" />\n");
    sb.Append("<img id=\"img_menu_show2\" src=\"/modular-project-main/index/image/right_b.png\" />\n");
    sb.Append("</div>\n");
    sb.Append("<div class=\"switch_title\">显示菜单栏</div>\n");
    sb.Append("<div class=\"clear\"></div>\n");
    sb.Append("</a>\n");
    sb.Append("</div>\n");
    sb.Append("<div class=\"exit\"></div>\n");
    sb.Append("<div class=\"user\"></div>\n");
    sb.Append("</div>\n");
    sb.Append("<div class=\"frame\">\n");
    sb.Append("<iframe id=\"frm_main\" src=\"about:blank\"></iframe>\n");
    sb.Append("</div>\n");
    sb.Append("</div>\n");
    sb.Append("<script type=\"text/javascript\">\n");
    sb.Append("$(function () {\n");
    sb.Append("});\n");
    sb.Append("</script>\n");
    sb.Append("</body>\n");
    sb.Append("</html>\n");
    return Html(sb.ToString());
}

    }
}
