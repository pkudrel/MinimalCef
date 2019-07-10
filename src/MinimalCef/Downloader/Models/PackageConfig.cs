using System.Collections.Generic;

namespace MinimalCef.Downloader.Models
{
    public class PackageConfig
    {
        public List<NugetInfo> Nugets { get; set; } = new List<NugetInfo>();
        public string Name { get; set; }
    }
}