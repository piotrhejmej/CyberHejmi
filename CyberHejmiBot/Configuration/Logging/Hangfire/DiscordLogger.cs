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
            //if (logLevel == LogLevel.Error)
            //{
            if (Client.LoginState != Discord.LoginState.LoggedIn)
                return false;

                if (Client.Rest.GetChannelAsync(CHANNEL_ID).Result is not RestTextChannel restChannel)
                {
                    Console.WriteLine("restchannel is null");
                    return false;
                }

                if (messageFunc != null)
                    restChannel.SendMessageAsync($"Error: {messageFunc()}");

                if (exception != null)
                {
                    restChannel.SendMessageAsync($"Exception: {exception.Message}");

                    if (exception.InnerException != null)
                        restChannel.SendMessageAsync($"Inner Exception: {exception.InnerException.Message}");
                }
            //}

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
