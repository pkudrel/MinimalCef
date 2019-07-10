using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using NLog;

namespace MinimalCef.Common.Bootstrap
{
    /// <summary>
    /// Credit: https://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c
    /// </summary>
    public static class ExclusiveWorker
    {
        private const int _WAIT_TIME_IN_MS = 5000;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static readonly MutexAccessRule _allowEveryoneRule =
            new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid
                    , null)
                , MutexRights.FullControl
                , AccessControlType.Allow
            );

        private static readonly string _appGuid =
            ((GuidAttribute) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false)
                .GetValue(0)).Value ?? typeof(ExclusiveWorker).Name;

        public static void DoExclusiveWorker(Action exclusiveWork, string mutexId, int waitTimeInMs)
        {
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(_allowEveryoneRule);
            using (var mutex = new Mutex(false, mutexId, out _, securitySettings))
            {
                var hasHandle = false;
                try
                {
                    try
                    {
                        // note, you may want to time out here instead of waiting forever
                        // mutex.WaitOne(Timeout.Infinite, false);
                        hasHandle = mutex.WaitOne(waitTimeInMs, false);
                        if (hasHandle == false)
                            throw new TimeoutException(
                                $"Timeout waiting for global mutex exclusive access. Wait time: {waitTimeInMs}");
                    }
                    catch (AbandonedMutexException e)
                    {
                        // Log the fact that the mutex was abandoned in another process,
                        // it will still get acquired
                        _log.Debug(e, "Global mutex was abandoned in another process");
                        hasHandle = true;
                    }

                    // Perform your work here.
                    exclusiveWork();
                }
                finally
                {
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }

        public static void DoExclusiveWorker(Action exclusiveWork)
        {
            var mutexId = $"Global\\{{{_appGuid}}}";
            DoExclusiveWorker(exclusiveWork, mutexId, _WAIT_TIME_IN_MS);
        }

        public static void DoExclusiveWorker(Action exclusiveWork, string mutexId)
        {
            DoExclusiveWorker(exclusiveWork, mutexId, _WAIT_TIME_IN_MS);
        }
    }
}