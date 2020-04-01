using System;
using System.Text;
using dpz3;
using dpz3.Modular;

namespace pages.manager {

    [Modular(ModularTypes.Session, "/manager")]
    public class _Page : SessionControllerBase {

        [Modular(ModularTypes.Get, "nologin")]
public IResult nologin() {
    StringBuilder sb = new StringBuilder();
    var session = Host.Session;
    sb.Append("<html>\n");
    sb.Append("<head>\n");
    sb.Append("<meta charset=\"utf-8\" />\n");
    sb.Append("<title>店小妹超市收银(后台管理端)</title>\n");
    sb.Append("<script src=\"/dxm-cashier-common/js/jquery-3.4.1.min.js\"></script>\n");
    sb.Append("<script src=\"/dxm-cashier-common/js/vue.js\"></script>\n");
    sb.Append("<script src=\"/dxm-cashier-common/js/dpz2.js\"></script>\n");
    sb.Append("<script src=\"/dxm-cashier-common/js/jttp.js\"></script>\n");
    sb.Append("<script src=\"/dxm-cashier-common/js/host.js\"></script>\n");
    sb.Append("<script src=\"/dxm-cashier-common/manager/nologin/page.js\"></script>\n");
    sb.Append("<link href=\"/dxm-cashier-common/manager/nologin/page.css\" type=\"text/css\" rel=\"stylesheet\" />\n");
    sb.Append("</head>\n");
    sb.Append("<body>\n");
    sb.Append("<div class=\"notice\">用户尚未登录或登录超时</div>\n");
    sb.Append("<div class=\"link\"><a href=\"/dxm-cashier-manager-login/index\" target=\"_parent\">返回用户登录页面</a></div>\n");
    sb.Append("</body>\n");
    sb.Append("</html>\n");
    return Html(sb.ToString());
}

    }
}
