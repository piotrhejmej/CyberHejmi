using CyberHejmiBot.Business.Common;
using Hangfire.Logging;

namespace CyberHejmiBot.Configuration.Logging.Hangfire
{
    public class DiscordHangfireLogger : ILog
    {
        private readonly DiscordLogService _service;
        private readonly string _name;

        public DiscordHangfireLogger(DiscordLogService service, string name)
        {
            _service = service;
            _name = name;
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            var msLevel = MapLevel(logLevel);

            _service.Log(msLevel, messageFunc?.Invoke() ?? "", exception, $"HF: {_name}");
            return true;
        }

        private Microsoft.Extensions.Logging.LogLevel MapLevel(LogLevel hfLevel)
        {
            return hfLevel switch
            {
                LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
                LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
                LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
                LogLevel.Warn => Microsoft.Extensions.Logging.LogLevel.Warning,
                LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                LogLevel.Fatal => Microsoft.Extensions.Logging.LogLevel.Critical,
                _ => Microsoft.Extensions.Logging.LogLevel.None
            };
        }
    }

    public class DiscordHangfireLogProvider : ILogProvider
    {
        private readonly DiscordLogService _service;

        public DiscordHangfireLogProvider(DiscordLogService service)
        {
            _service = service;
        }

        public ILog GetLogger(string name)
        {
            return new DiscordHangfireLogger(_service, name);
        }
    }
}