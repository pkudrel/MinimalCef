using NLog;

namespace MinimalCef
{
    public class SomeMain
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public void Write()
        {
            var path = GetType().Assembly.Location;
            _log.Debug($"SomeMain location: {path}");
        }
    }
}