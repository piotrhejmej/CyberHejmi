using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CyberHejmiBot.Configuration.Logging.DebugLogger
{
    public interface IDebugLogger
    {
        bool Log(LogLevel logLevel, string message, Exception? exception = null);
        bool LogInfo(string message, Exception? exception = null);
        bool LogWarning(string message, Exception? exception = null);
        bool LogError(string message, Exception? exception = null);
    }

    public class DebugLogger: IDebugLogger
    {
        private readonly DiscordSocketClient Client;
        private static ulong CHANNEL_ID = 1012391792014008353;

        public DebugLogger(DiscordSocketClient client)
        {
            Client = client;
        }

        public bool Log(LogLevel logLevel, string message, Exception? exception = null)
        {
            if (Client.LoginState != Discord.LoginState.LoggedIn)
                return false;

            if (Client.Rest.GetChannelAsync(CHANNEL_ID).Result is not RestTextChannel restChannel)
                return false;

            var embedded = new Discord.EmbedBuilder()
                   .WithColor(LogColors[logLevel])
                   .WithTimestamp(DateTimeOffset.UtcNow);

            embedded.WithTitle(message ?? "Error");

            if (exception != null)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"{Enum.GetName(typeof(LogLevel), logLevel)}: {exception.Message}");
                stringBuilder.AppendLine("---------");

                if (exception.InnerException != null)
                    stringBuilder.AppendLine($"Inner Exception: {exception.InnerException.Message}");

                embedded.WithDescription(stringBuilder.ToString());
            }

            restChannel.SendMessageAsync(embed: embedded.Build());

            return true;
        }

        public bool LogInfo(string message, Exception? exception = null)
        {
            return Log(LogLevel.Information, message, exception);
        }

        public bool LogWarning(string message, Exception? exception = null)
        {
            return Log(LogLevel.Warning, message, exception);
        }

        public bool LogError(string message, Exception? exception = null)
        {
            return Log(LogLevel.Error, message, exception);
        }

        private static readonly Dictionary<LogLevel, Color> LogColors = new Dictionary<LogLevel, Color>
        {
            { LogLevel.Trace, Color.Blue },
            { LogLevel.Debug, Color.DarkPurple },
            { LogLevel.Information, Color.Blue },
            { LogLevel.Warning, Color.Orange },
            { LogLevel.Error, Color.Red },
            { LogLevel.Critical, Color.DarkRed }
        };
    }
}
