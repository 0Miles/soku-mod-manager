using Serilog;

namespace SokuModManager
{
    public class Logger
    {
        private static readonly ILogger _logger;

        static Logger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public static void LogError(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }
    }
}
