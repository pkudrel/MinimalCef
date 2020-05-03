using System.IO;
using MinimalCef.Common.Bootstrap;
using MinimalCef.Common.Misc;
using MinimalCef.Downloader.Models;

namespace MinimalCef.Models
{
    public class Registry
    {
        public Registry(AppEnvironment env, Config config)
        {
            PackageConfig = config.PackageConfig;
            CefSharpPackageName = config.PackageConfig.Name;
            CefSharpEnvStorePath = Path.Combine(env.CefBinariesDir, "CefSharp", "packages");
            GlobalCefSharpEnvPath = Path.Combine(CefSharpEnvStorePath, config.PackageConfig.Name);
            CefSharpLocalePath = Path.Combine(GlobalCefSharpEnvPath, "locales");
            GlobalBrowserSubprocessPath = Path.Combine(GlobalCefSharpEnvPath, "CefSharp.BrowserSubprocess.exe");
            LocalCefSharpEnvPath = env.ExeFileDir;
            LocalBrowserSubprocessPath = Path.Combine(LocalCefSharpEnvPath, "CefSharp.BrowserSubprocess.exe");
            UseGlobalBrowserSubprocess = config.UseGlobalBrowserSubprocess;
            Io.CreateDirIfNotExist(CefSharpEnvStorePath);
        }
        public bool UseGlobalBrowserSubprocess { get; set; }
        public string CefSharpLocalePath { get; }
        public PackageConfig PackageConfig { get; }
        public string GlobalBrowserSubprocessPath { get; }
        public string LocalBrowserSubprocessPath { get; }
        public string GlobalCefSharpEnvPath { get; }
        public string LocalCefSharpEnvPath { get; }
        public string CefSharpPackageName { get; }
        public string CefSharpEnvStorePath { get; }
    }
}