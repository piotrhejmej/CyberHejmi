using CyberHejmiBot.Configuration.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CyberHejmiBot.Business.Common
{
    public class DiscordLogService
    {
        private readonly DiscordSocketClient _client;
        private readonly ulong _logChannelId;

        public DiscordLogService(DiscordSocketClient client, BotSettings settings)
        {
            _client = client;
            _logChannelId = settings.LogChannelId;
        }

        public void Log(LogLevel logLevel, string message, Exception? exception = null, string source = "App")
        {
            PrintToConsole(logLevel, message, exception, source);

            if (logLevel < LogLevel.Warning) 
                return;

            if (_client.LoginState != LoginState.LoggedIn) 
                return;

            if (_client.GetChannel(_logChannelId) is not IMessageChannel channel) 
                return;

            var embed = BuildEmbed(logLevel, message, exception, source);

            _ = Task.Run(async () =>
            {
                try { await channel.SendMessageAsync(embed: embed); }
                catch {}
            });
        }

        private Embed BuildEmbed(LogLevel logLevel, string message, Exception? exception, string source)
        {
            var color = LogColors.TryGetValue(logLevel, out var c) ? c : Color.Default;

            var embedBuilder = new EmbedBuilder()
                .WithColor(color)
                .WithTitle($"[{source}] {logLevel}: {message}")
                .WithTimestamp(DateTimeOffset.UtcNow);

            if (exception != null)
            {
                var sb = new StringBuilder();
                var exMsg = exception.ToString();
                if (exMsg.Length > 2000) exMsg = exMsg.Substring(0, 2000) + "...";

                sb.AppendLine($"```{exMsg}```");
                embedBuilder.WithDescription(sb.ToString());
            }

            return embedBuilder.Build();
        }

        private void PrintToConsole(LogLevel level, string msg, Exception? ex, string source)
        {
            var color = level >= LogLevel.Error ? ConsoleColor.Red : ConsoleColor.Gray;
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{source}] {level}: {msg}");
            if (ex != null) Console.WriteLine(ex.Message);
            Console.ResetColor();
        }

        private static readonly Dictionary<LogLevel, Color> LogColors = new()
        {
            { LogLevel.Trace, Color.LightGrey },
            { LogLevel.Debug, Color.DarkGrey },
            { LogLevel.Information, Color.Blue },
            { LogLevel.Warning, Color.Orange },
            { LogLevel.Error, Color.Red },
            { LogLevel.Critical, Color.DarkRed }
        };
    }
}