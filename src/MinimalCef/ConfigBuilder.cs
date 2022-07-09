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

        public static PackageConfig GetPackageConfig()
        {
            return new PackageConfig
            {
                Name = "cefsharp_103.0.9_x64",
                Nugets = new List<NugetInfo>
                {
                    new NugetInfo("CefSharp.Common", "103.0.90",
                        new List<CopyInfo>
                        {
                            new CopyInfo("/CefSharp/x64", "/"),
                            new CopyInfo("/lib/net452", "/")
                        }),

                    new NugetInfo("CefSharp.WinForms", "103.0.90",
                        new List<CopyInfo> {new CopyInfo("/lib/net452", "/")}),


                    new NugetInfo("cef.redist.x64", "103.0.9",
                        new List<CopyInfo>
                        {
                            new CopyInfo("/CEF", "/"),
                            new CopyInfo("/CEF/locales", "/locales"),
                            //new CopyInfo("/CEF/swiftshader", "/swiftshader")
                        })
                }
            };
        }
    }
}