using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 宿主程序
/// </summary>
public static class Host {

    /// <summary>
    /// 获取超文本上下文
    /// </summary>
    public static HttpContext Context { get; }

    /// <summary>
    /// 获取交互管理器
    /// </summary>
    public static ISessionManager Session { get; }

    /// <summary>
    /// 获取数据库连接管理器
    /// </summary>
    public static dpz3.db.Connection Connection { get; }

}