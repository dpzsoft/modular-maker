using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Helpers;
using Chromely.Core.Infrastructure;
using Chromely.Core.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ModularProject.Chromely {
    internal class MyChromelyConfiguration : IChromelyConfiguration {
        public string AppName { get; set; }
        public string StartUrl { get; set; }
        public string AppExeLocation { get; set; }
        public string ChromelyVersion { get; set; }
        public ChromelyPlatform Platform { get; set; }
        public bool DebuggingMode { get; set; }
        public string DevToolsUrl { get; set; }
        public IDictionary<string, string> CommandLineArgs { get; set; }
        public List<string> CommandLineOptions { get; set; }
        public List<ControllerAssemblyInfo> ControllerAssemblies { get; set; }
        public IDictionary<string, string> CustomSettings { get; set; }
        public List<ChromelyEventHandler<object>> EventHandlers { get; set; }
        public IDictionary<string, object> ExtensionData { get; set; }
        public IChromelyJavaScriptExecutor JavaScriptExecutor { get; set; }
        public List<UrlScheme> UrlSchemes { get; set; }
        public CefDownloadOptions CefDownloadOptions { get; set; }
        public IWindowOptions WindowOptions { get; set; }

        internal MyChromelyConfiguration() {
            AppName = "模块化项目管理器 Ver:" + Assembly.GetEntryAssembly()?.GetName().Version.ToString();
            Platform = ChromelyRuntime.Platform;
            AppExeLocation = AppDomain.CurrentDomain.BaseDirectory;
            //StartUrl = "local://app/chromely.html";
            StartUrl = "http://127.0.0.1:8080/modular-project-main/";
            DebuggingMode = false;
            UrlSchemes = new List<UrlScheme>();
            CefDownloadOptions = new CefDownloadOptions();
            WindowOptions = new WindowOptions {
                RelativePathToIconFile = @"X:\Projects\modular\maker\ModularProject.Chromely\client.ico",
                Title = AppName,
                Size = new WindowSize(1200, 700)
            };

            UrlSchemes.AddRange(new List<UrlScheme>()
            {
                new UrlScheme(DefaultSchemeName.RESOURCE, "local", string.Empty, string.Empty, UrlSchemeType.Resource, false),
                new UrlScheme(DefaultSchemeName.CUSTOM, "http", "chromely.com", string.Empty, UrlSchemeType.Custom, false),
                new UrlScheme(DefaultSchemeName.COMMAND, "http", "command.com", string.Empty, UrlSchemeType.Command, false),
                new UrlScheme(DefaultSchemeName.GITHUBSITE, string.Empty, string.Empty, "https://github.com/chromelyapps/Chromely", UrlSchemeType.External, true)
            });

            ControllerAssemblies = new List<ControllerAssemblyInfo>();

            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var externalControllerFile = Path.Combine(appDirectory, "Chromely.External.Controllers.dll");
            if (File.Exists(externalControllerFile)) {
                ControllerAssemblies.RegisterServiceAssembly("Chromely.External.Controllers.dll");
                var assemblyOptions = new AssemblyOptions(externalControllerFile, null, "app");
                UrlSchemes.Add(new UrlScheme(DefaultSchemeName.ASSEMBLYRESOURCE, "assembly", "app", string.Empty, UrlSchemeType.AssemblyResource, false, assemblyOptions));

                var mixAssemblyOptions = new AssemblyOptions(externalControllerFile, null, "appresources");
                UrlSchemes.Add(new UrlScheme(DefaultSchemeName.MIXASSEMBLYRESOURCE, "mixassembly", "app", string.Empty, UrlSchemeType.AssemblyResource, false, mixAssemblyOptions));
            }

            CustomSettings = new Dictionary<string, string>() {
                ["cefLogFile"] = "logs\\chromely.cef.log",
                ["logSeverity"] = "info",
                ["locale"] = "zh-CN"
            };
        }
    }
}