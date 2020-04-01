<%@ Page Language="C#" %>

<%
    var session = Host.Session;
%>

<html>
<head>
    <meta charset="utf-8" />
    <title>店小妹超市收银(后台管理端)</title>
    <script src="/dxm-cashier-common/js/jquery-3.4.1.min.js"></script>
    <script src="/dxm-cashier-common/js/vue.js"></script>
    <script src="/dxm-cashier-common/js/dpz2.js"></script>
    <script src="/dxm-cashier-common/js/jttp.js"></script>
    <script src="/dxm-cashier-common/js/host.js"></script>
    <script src="/dxm-cashier-common/manager/nologin/page.js"></script>
    <link href="/dxm-cashier-common/manager/nologin/page.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <div class="notice">用户尚未登录或登录超时</div>
    <div class="link"><a href="/dxm-cashier-manager-login/index" target="_parent">返回用户登录页面</a></div>
</body>
</html>
