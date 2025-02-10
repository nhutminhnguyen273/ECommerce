using Serilog;

namespace ECommerce.SharedLibrary.Logs
{
    public static class LogException
    {
        public static void LogExceptions(Exception ex)
        {
            LogToFile(ex);
            LogToDebug(ex);
            LogToConsole(ex);
        }

        private static void LogToFile(Exception ex)
            => Log.Information(ex.Message);
        private static void LogToDebug(Exception ex)
            => Log.Debug(ex.Message);
        private static void LogToConsole(Exception ex)
            => Log.Warning(ex.Message);
    }
}