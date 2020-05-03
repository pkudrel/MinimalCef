using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MinimalCef.Downloader.Helpers;
using MinimalCef.Downloader.View;
using MinimalCef.Models;
using NLog;

namespace MinimalCef.Downloader
{
    public class ProgramDownloader
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void DownloadCefSharpEnvIfNeeded(Registry reg)
        {
            var sw = new Stopwatch();

            if (File.Exists(reg.GlobalBrowserSubprocessPath) == false)
            {
                sw.Start();
                _log.Debug($"Create global CefSharpEnv in: {reg.GlobalCefSharpEnvPath}");
                BeginDownloadProcess(reg);
                sw.Stop();
                _log.Debug($"Create global CefSharpEnv took: {sw.ElapsedMilliseconds}ms");
            }

            sw.Reset();

            if (reg.UseGlobalBrowserSubprocess) return;
            if (File.Exists(reg.LocalBrowserSubprocessPath)) return;
            _log.Debug($"Create local CefSharpEnv in: {reg.LocalCefSharpEnvPath}");
            sw.Start();
            CopyTools.CopyAll(reg.GlobalCefSharpEnvPath,
                reg.LocalCefSharpEnvPath, false);
            sw.Stop();
            _log.Debug($"Create local CefSharpEnv took: {sw.ElapsedMilliseconds}ms");
        }


        private static void BeginDownloadProcess(Registry reg)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DownloadForm(reg));
        }
    }
}