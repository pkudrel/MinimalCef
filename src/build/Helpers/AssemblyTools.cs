using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AbcVersionTool;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.FileSystemTasks;

namespace Helpers
{
    public static class AssemblyTools
    {
        const string _TEMPORARY_EXTENSION = ".temporary-backup-file";
        const string _ASSEMBLY_INFO_FILE = "AssemblyInfo.cs";

        static void MakeBackup(string file)
        {
            var dir = Path.GetDirectoryName(file);
            var newName = $"{Path.GetFileName(file)}{_TEMPORARY_EXTENSION}";
            var fullPath = Path.Combine(dir, newName);
            CopyFile(file, fullPath);
        }

        public static void Patch(string dirPath, AbcVersion abcVersion, ProjectDefinition projectDefinition,
            ProductInfo productInfo)
        {
            var files = GetAssemblyFiles(dirPath);
            foreach (var file in files)
            {
                MakeBackup(file);

                var s = File.ReadAllText(file);
                s = OperateOn(s, "AssemblyVersion", abcVersion.AssemblyVersion);
                s = OperateOn(s, "AssemblyFileVersion", abcVersion.FileVersion);
                s = OperateOn(s, "AssemblyInformationalVersion", abcVersion.InformationalVersion);
                s = OperateOn(s, "AssemblyProduct", projectDefinition.Name);
                s = OperateOn(s, "AssemblyCopyright", productInfo.Copyright);
                s = OperateOn(s, "AssemblyCompany", productInfo.Company);
                s = Regex.Replace(s, @"//.*", "");
                s = Regex.Replace(s, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
                File.WriteAllText(file, s, new UTF8Encoding(false));
            }
        }


        public static void RollbackOriginalFiles(string dirPath)
        {
            var files = GlobFiles(dirPath, $"*/**/*{_TEMPORARY_EXTENSION}");
            foreach (var file in files)
            {
                var dir = Path.GetDirectoryName(file);
                var currentFile = Path.Combine(dir, _ASSEMBLY_INFO_FILE);
                DeleteFile(currentFile);
                CopyFile(file, currentFile);
                DeleteFile(file);
            }
        }

        static string OperateOn(string source, string tag, string value)
        {
            var newValue = $"{tag}(\"{value}\")";
            var pattern = tag + @"\((.*)\)";

            if (Regex.Match(source, pattern).Success == false)
                source += $"[assembly: {newValue}]{Environment.NewLine}";
            else
                source = Regex.Replace(source, pattern, newValue);

            return source;
        }

        static IEnumerable<string> GetAssemblyFiles(string dirPath) =>
            GlobFiles(dirPath, $"*/**/{_ASSEMBLY_INFO_FILE}");
    }
}