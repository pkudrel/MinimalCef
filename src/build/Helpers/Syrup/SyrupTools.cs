using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AbcVersionTool;
using Helpers.Azure;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Nuke.Common;

namespace Helpers.Syrup
{
    [PublicAPI]
    public static class SyrupTools
    {
        public static void MakeSyrupFile(string path, AbcVersion abcVersion, ProjectDefinition productInfo)
        {
            var r = new SyrupInfo();
            r.App = productInfo.Name;
            r.Name = Path.GetFileNameWithoutExtension(path);
            r.File = Path.GetFileName(path);
            r.Sha = GetsShaHashForFile(path);
            r.SemVer = abcVersion.SemVersion;
            r.Channel = abcVersion.GitBranch;
            r.ReleaseDate = $"{abcVersion.DateTime:s}Z";
            var dst = Path.GetFullPath(path) + ".syrup";
            var json = JsonConvert.SerializeObject(r, Formatting.Indented);
            File.WriteAllText(dst, json, new UTF8Encoding(false));
            Logger.Normal($"Syrup; Make syrup for: {path}; Syrup file: {Path.GetFileName(dst)}");
        }

        static string GetsShaHashForFile(string path)
        {
            using (var stream = new BufferedStream(File.OpenRead(path)))
            {
                var sha = new SHA256Managed();
                var checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
            }
        }
    }
}