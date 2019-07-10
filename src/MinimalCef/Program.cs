using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using MinimalCef.Common.Bootstrap;
using MinimalCef.Downloader;
using MinimalCef.Models;
using NLog;

namespace MinimalCef
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static readonly AppEnvironment _env = AppEnvironmentBuilder.Instance.GetAppEnvironment();
        private static readonly Config _cnf = ConfigBuilder.Create();
        private static readonly Registry _reg = new Registry(_env, _cnf);

        [STAThread]
        private static void Main()
        {
            InitApp();

            try
            {
                StartApp();
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
        }


        private static void StartApp()
        {
            RunBrowser();
        }

        private static void InitApp()
        {
            Boot.Instance.Start(typeof(Program).Assembly);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ProgramDownloader.DownloadCefSharpEnvIfNeeded(_reg);
        }

        private static void RunBrowser()
        {
            //Monitor parent process exit and close subprocesses if parent process exits first
            //This will at some point in the future becomes the default
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();

            var settings = new CefSettings
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "CefSharp\\Cache")
            };

            //Example of setting a command line argument
            //Enables WebRTC
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, false, browserProcessHandler: null);

            var browser = new BrowserForm();
            Application.Run(browser);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var location = typeof(Program).Assembly.Location;
            _log.Debug($"CurrentDomain_AssemblyResolve Location: '{location}'");
            var dll = args.Name.Split(new[] {','}, 2)[0] + ".dll";
            _log.Debug($"Find: '{dll}' in '{_reg.GlobalCefSharpEnvPath}'");
            switch (dll)
            {
                case "CefSharp.Core.dll":
                case "CefSharp.dll":
                    var path = Path.Combine(_reg.GlobalCefSharpEnvPath, dll);
                    var asm = Assembly.LoadFrom(path);
                    return asm;
                default:
                    return null;
            }
        }
    }
}