using System.IO;
using Serilog;

namespace ScatterBot_v2.core.ErrorHandling
{
    /// <summary>
    /// Initializing the logger
    /// </summary>
    public static class Logging
    {
        private static string AppPath => Directory.GetCurrentDirectory();
        private static string LogPath => AppPath + "/logs/";
        private static string LogFile => LogPath + "log.txt";

        public static void Create()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(LogFile, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}