using System;
using System.Collections.Generic;
using System.Reflection;
using NLog;

namespace MinimalCef.Common.Bootstrap
{
    public class Boot
    {
        private static readonly List<string> _buffer = new List<string>();
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly Boot _instance = new Boot();
        private AppEnvironment _appEnvironment;
	   

		static Boot()
        {
        }

        private Boot()
        {
        }

        // ReSharper disable once ConvertToAutoProperty
        public static Boot Instance => _instance;


	    public void Start(Assembly mainAssembly)
        {
            _buffer.Add($"Begin boot; Main assembly: {mainAssembly.GetName().Name}");
            AssemblyCollector.Instance.AddAssembly(mainAssembly, AssemblyInProject.Main);
            _appEnvironment = AppEnvironmentBuilder.Instance.GetAppEnvironment(mainAssembly);
            // Configure Nlog as soon as possible
            DeveloperMode.NLog(_appEnvironment, _buffer);
            FlushBuffer(_buffer);
            _log.Debug("After NLog config");
        }

        public AppEnvironment GetAppEnvironment()
        {
            if (_appEnvironment == null)
                throw new NullReferenceException("Application has not been started correctly. Use Start method");
            return _appEnvironment;
        }

        private static void FlushBuffer(ICollection<string> list)
        {
            foreach (var msg in list) _log.Debug(msg);
            list.Clear();
        }

        public void AddAssembly(Assembly assembly, AssemblyInProject assemblyInProject)
        {
            AssemblyCollector.Instance.AddAssembly(assembly, assemblyInProject);
        }

        public Assembly[] GetAssemblies()
        {
            return AssemblyCollector.Instance.GetAssemblies();
        }

	   
    }
}