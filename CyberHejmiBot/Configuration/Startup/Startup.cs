using CyberHejmiBot.Business.Events;
using CyberHejmiBot.Configuration.Loging;
using CyberHejmiBot.Configuration.Settings;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Startup
{
    interface IStartup
    {
        Task Init();
    }

    internal class Startup: IStartup
    {
        private readonly DiscordSocketClient Client;
        private readonly TextCommandHandler CommandHandler;
        private readonly ILogger Logger;
        private readonly BotSettings BotSettings;
        private readonly IEventListener EventListener;

        public Startup(DiscordSocketClient client, TextCommandHandler commandHandler, ILogger logger, BotSettings botSettings, IEventListener eventListener)
        {
            Client = client;
            CommandHandler = commandHandler;
            Logger = logger;
            BotSettings = botSettings;
            EventListener = eventListener;
        }

        public async Task Init()
        {
            await CommandHandler.InstallCommandsAsync();
            await EventListener.StartAsync();

            Client.Log += Logger.Log;

            await Client.LoginAsync(TokenType.Bot, BotSettings.BotToken);
            await Client.StartAsync();

        }
    }
}
