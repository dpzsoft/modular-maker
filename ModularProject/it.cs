using System;
using System.Collections.Generic;
using System.Text;

namespace ModularProject {

    /// <summary>
    /// 主体替代器
    /// </summary>
    internal static class it {

        // 配置文件
        internal const string Path_WorkFolder = "\\.XormManager";
        internal const string Path_WorkConfig = "\\XormManager.conf";
        internal const string Path_Config = "manager.conf";
        internal const string Path_TemplateFolder = "template";

        // 工作配置
        internal static dpz3.File.ConfFile WorkCfg;

        // 工作目录
        internal static string WorkPath = null;
        internal static string WorkPathInit = null;

        // 当前目录
        internal static string LocalPath = null;
        internal static string LocalTemplateFolder = null;

    }
}
