using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MinimalCef.Common.Bootstrap
{
    public class DeveloperMode
    {
        private const string _NLOG_FILE = "NLog.config";
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void NLog(AppEnvironment res, List<string> log)
        {
            var nlogDevFilePath = Path.Combine(res.RootDir, _NLOG_FILE);


          
            const string targetName = "form-winform";
            LogManager.Configuration = LogManager.Configuration ?? new LoggingConfiguration();
            if (LogManager.Configuration.AllTargets.Any(x => x.Name == targetName)) return;


           


            if (File.Exists(nlogDevFilePath))
            {
                log.Add($"Dev NLog config exists, use it: {nlogDevFilePath} ");
                LogManager.Configuration = new XmlLoggingConfiguration(nlogDevFilePath, true);
                LogManager.Configuration.Variables["logDirectory"] = res.LogDir;
                LogManager.Configuration.Variables["appName"] = res.AssemblyName.Replace('.', '-').GenerateSlug();
            }
            else
            {
                log.Add($"Use default embed NLog config");
                var config = LogManager.Configuration ?? new LoggingConfiguration();

                // text 
                config.Variables["logDirectory"] = res.LogDir;
                config.Variables["appName"] = res.AssemblyName.Replace('.', '-').SplitOnSpaces().GenerateSlug();
                var fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);
                fileTarget.FileName = "${var:logDirectory}/debug.${var:appName}.txt";
                fileTarget.ArchiveFileName = "${var:logDirectory}/archives/debug.${var:appName}.{#}.txt";
                fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";
                fileTarget.ArchiveEvery = FileArchivePeriod.Day;
                fileTarget.ArchiveNumbering = ArchiveNumberingMode.Rolling;
                fileTarget.MaxArchiveFiles = 12;
                var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
                config.LoggingRules.Add(rule);

                // form
               

                LogManager.Configuration = config;
            }
        }
    }
}