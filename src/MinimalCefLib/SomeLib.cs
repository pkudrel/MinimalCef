using NLog;

namespace MinimalCefLib
{
    public class SomeLib
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public void Write()
        {
            var path = GetType().Assembly.Location;
            _log.Debug($"SomeMain location: {path}");
        }
    }
}