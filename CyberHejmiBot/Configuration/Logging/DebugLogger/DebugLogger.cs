using CyberHejmiBot.Business.Common; // Tam gdzie wrzuciłeś serwis
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Configuration.Logging.DebugLogger
{
    public interface IDebugLogger
    {
        bool Log(LogLevel logLevel, string message, Exception? exception = null);
        bool LogInfo(string message, Exception? exception = null);
        bool LogWarning(string message, Exception? exception = null);
        bool LogError(string message, Exception? exception = null);
    }

    public class DebugLogger : IDebugLogger
    {
        private readonly DiscordLogService _logService;

        public DebugLogger(DiscordLogService logService)
        {
            _logService = logService;
        }

        public bool Log(LogLevel logLevel, string message, Exception? exception = null)
        {
            _logService.Log(logLevel, message, exception, "BOT");
            return true;
        }

        public bool LogInfo(string message, Exception? exception = null)
            => Log(LogLevel.Information, message, exception);

        public bool LogWarning(string message, Exception? exception = null)
            => Log(LogLevel.Warning, message, exception);

        public bool LogError(string message, Exception? exception = null)
            => Log(LogLevel.Error, message, exception);
    }
}