using System.Collections.Generic;
using MinimalCef.Downloader.Models;
using MinimalCef.Models;

namespace MinimalCef
{
    public static class ConfigBuilder
    {
        public static Config Create()
        {
            var ret = new Config {PackageConfig = GetPackageConfig(), UseGlobalBrowserSubprocess = true};
            return ret;
        }

        private static PackageConfig GetPackageConfig()
        {
            return new PackageConfig
            {
                Name = "cefsharp_97.1.6_x64",
                Nugets = new List<NugetInfo>
                {
                    new NugetInfo("CefSharp.Common", "97.1.60",
                        new List<CopyInfo>
                        {
                            new CopyInfo("/CefSharp/x64", "/"),
                            new CopyInfo("/lib/net452", "/")
                        }),

                    new NugetInfo("CefSharp.WinForms", "97.1.60",
                        new List<CopyInfo> { new CopyInfo("/lib/net452", "/") }),


                    new NugetInfo("cef.redist.x64", "97.1.6",
                        new List<CopyInfo>
                        {
                            new CopyInfo("/CEF", "/"),
                            new CopyInfo("/CEF/locales", "/locales"),
                            new CopyInfo("/CEF/swiftshader", "/swiftshader")
                        })
                }
            };
        }
    }
}