using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MinimalCef.Common.Bootstrap
{
    public class AppVersionBuilder
    {
        /// <summary>
        /// Example:
        /// 0.1.0+BuildCounter.18.Branch.dev.DateTime.2016-07-15T08:37:10Z.Env.local.Sha.a11caeb0db526c1707e8aa40aaec20a315edb119.CommitsCounter.36
        /// </summary>
        /// <returns></returns>
        public static AppVersion GetVersion()
        {
            var w = new Worker();
            return w.Make();
        }

        public static AppVersion GetVersion(Assembly assembly)
        {
            var w = new Worker(assembly);
            return w.Make();
        }


        internal class Worker
        {
            private readonly Assembly _assembly;
            private readonly List<CustomAttributeData> _attributeDatas;

            public Worker(Assembly assembly)
            {
                _assembly = assembly;
                _attributeDatas = _assembly?.GetCustomAttributesData().ToList();
            }

            public Worker()
            {
                _assembly = Assembly.GetEntryAssembly();
                _attributeDatas = _assembly?.GetCustomAttributesData().ToList();
            }

            public string GetAssemblyVersion => _assembly.GetName().Version.ToString();

            public AppVersion Make()
            {
                var name = _assembly.GetName();
                var assemblyVersion = name.Version.ToString();
                var appName = GetAttributeSafe<AssemblyProductAttribute>() ?? name.Name;

                var assemblyFileVersion = GetAttributeSafe<AssemblyFileVersionAttribute>();
                var information = GetAttributeSafe<AssemblyInformationalVersionAttribute>();
                var res = new AppVersion(assemblyVersion, assemblyFileVersion, appName);
                if (information == null) return res;
                var parts = information.Split('+');
                if (parts.Length != 2) return res;
                var sem = parts[0];
                var info = new Info(parts[1]);

                return new AppVersion(assemblyVersion, assemblyFileVersion, appName,
                    parts[1], sem,
                    info.Get("BuildCounter"), info.Get("Branch"), info.Get("DateTime"),
                    info.Get("Env"), info.Get("Sha"), info.Get("CommitsCounter")
                );
            }


            private T GetAttribute<T>() where T : class
            {
                var t = Attribute.GetCustomAttribute(_assembly, typeof(T), false);
                return t as T;
            }


            private string GetAttributeSafe<T>() where T : class
            {
                var name = typeof(T).Name;
                var val = _attributeDatas.FirstOrDefault(x => x.AttributeType.Name.StartsWith(name))
                    ?.ConstructorArguments.FirstOrDefault().Value.ToString();

                return val ?? string.Empty;
            }

            internal class Info
            {
                private readonly string[] _arr;
                private readonly int _max;

                public Info(string s)
                {
                    _arr = s.Split('.');
                    _max = _arr.GetUpperBound(0) - 1;
                }

                public string Get(string name)
                {
                    var i = Array.FindIndex(_arr, t => t.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (i < 0) return string.Empty;
                    if (i > _max) return string.Empty;
                    return _arr[i + 1];
                }
            }
        }
    }

    //public class Version
    //{
    //    public Version(string assemblyVersion, string assemblyFileVersion, string assemblyName)
    //    {
    //        AssemblyVersion = assemblyVersion;
    //        AssemblyFileVersion = assemblyFileVersion;
    //        AssemblyName = assemblyName;
    //    }

    //    public Version(string assemblyVersion, string assemblyFileVersion, string assemblyName,
    //        string assemblyProductVersion, string semVer, string buildCounter,
    //        string branch, string dateTime, string env, string sha, string commitsCounter)
    //    {
    //        AssemblyVersion = assemblyVersion;
    //        AssemblyFileVersion = assemblyFileVersion;
    //        AssemblyName = assemblyName;
    //        AssemblyProductVersion = assemblyProductVersion;
    //        SemVer = semVer;
    //        BuildCounter = buildCounter;
    //        Branch = branch;
    //        DateTime = dateTime;
    //        Env = env;
    //        Sha = sha;
    //        CommitsCounter = commitsCounter;
    //    }

    //    public string AssemblyVersion { get; }
    //    public string AssemblyFileVersion { get; }
    //    public string AssemblyName { get; set; }
    //    public string AssemblyProductVersion { get; }
    //    public string SemVer { get; }
    //    public string BuildCounter { get; }
    //    public string Branch { get; }
    //    public string DateTime { get; }
    //    public string Env { get; }
    //    public string Sha { get; }
    //    public string CommitsCounter { get; }

    //    public string MainVersion => string.IsNullOrEmpty(SemVer) ? AssemblyFileVersion : SemVer;
    //    public string FullName => $"{AssemblyName} {MainVersion}";

    //    public string FullInfo => string.IsNullOrEmpty(AssemblyProductVersion)
    //        ? FullName
    //        : $"{AssemblyName} {AssemblyProductVersion}";

    //    public override string ToString()
    //    {
    //        return MainVersion;
    //    }
    //}
}