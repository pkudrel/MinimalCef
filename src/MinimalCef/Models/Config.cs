﻿using MinimalCef.Downloader.Models;

namespace MinimalCef.Models
{
    public class Config
    {
        public PackageConfig PackageConfig { get; set; }
        public bool UseGlobalBrowserSubprocess { get; set; }
    }
}