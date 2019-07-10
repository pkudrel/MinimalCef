using System.IO;

namespace MinimalCef.Downloader.Helpers
{
    public class CopyTools
    {
        public static void CopyAll(string sourcePath, string destinationPath, bool overrideFile)
        {
            //Now Create all of the directories
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*.*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            //Copy all the files
            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), overrideFile);
        }
    }
}