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

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            if (messageFunc == null)
                return logLevel > LogLevel.Info;

            if (logLevel == LogLevel.Error)
            {
                var mesage = messageFunc();

                if (Client.Rest.GetChannelAsync(CHANNEL_ID).Result is not RestTextChannel restChannel)
                    return false;

                restChannel.SendMessageAsync($"Error: {message}");

                if (exception != null)
                {
                    restChannel.SendMessageAsync($"Exception: {exception.Message}");

                    if (exception.InnerException != null)
                        restChannel.SendMessageAsync($"Inner Exception: {exception.InnerException.Message}");
                }
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
