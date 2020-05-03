using System.Collections.Generic;
using MinimalCef.Downloader.Models;
using MinimalCef.Models;

namespace MinimalCef
{
    public static class ConfigBuilder
    {
        public static Config Create()
        {
            var ret = new Config { PackageConfig = GetPackageConfig(), UseGlobalBrowserSubprocess = true };
            return ret;
        }

        private static PackageConfig GetPackageConfig()
        {
            return new PackageConfig
            {
                Name = "cefsharp_79.1.36_x64",
                Nugets = new List<NugetInfo>
                {
                    new NugetInfo("CefSharp.Common", "79.1.360",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("CefSharp.WinForms", "79.1.360",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("cef.redist.x64", "79.1.36",
                        new List<CopyInfo>
                        {
                            new CopyInfo("/CEF", "/"),
                            new CopyInfo("/CEF/locales", "/locales"),
                            new CopyInfo("/CEF/swiftshader", "/swiftshader"),
                        }),

                }
            };
        }
    }
}