using Discord.Rest;
using Discord.WebSocket;
using Hangfire.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Logging.Hangfire
{
    public class DiscordLogger : ILog
    {
        private readonly DiscordSocketClient Client;
        private static ulong CHANNEL_ID = 1012391792014008353;

        public DiscordLogger(DiscordSocketClient client, string name)
        {
            Client = client;
            Name = name;
        }

        public string Name { get; set; }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception? exception = null)
        {
            if (logLevel == LogLevel.Error)
            {
                if (Client.LoginState != Discord.LoginState.LoggedIn)
                    return false;

                if (Client.Rest.GetChannelAsync(CHANNEL_ID).Result is not RestTextChannel restChannel)
                    return false;

                var embedded = new Discord.EmbedBuilder()
                    .WithColor(Discord.Color.Red)
                    .WithTimestamp(DateTimeOffset.UtcNow);

                embedded.WithTitle(messageFunc is not null ? messageFunc() : "Error");

                if (exception != null)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"Exception: {exception.Message}");
                    stringBuilder.AppendLine("---------");

                    if (exception.InnerException != null) 
                        stringBuilder.AppendLine($"Inner Exception: {exception.InnerException.Message}");

                    embedded.WithDescription(stringBuilder.ToString());
                }

                restChannel.SendMessageAsync(embed: embedded.Build());
            }

            return true;
        }
    }

    public class DiscordLoggerProvider : ILogProvider
    {
        private readonly DiscordSocketClient Client;

        public DiscordLoggerProvider(DiscordSocketClient client)
        {
            Client = client;
        }

        public ILog GetLogger(string name)
        {
            return new DiscordLogger(Client, name);
        }
    }
}
