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

        public Startup(DiscordSocketClient client, TextCommandHandler commandHandler, ILogger logger, BotSettings botSettings)
        {
            Client = client;
            CommandHandler = commandHandler;
            Logger = logger;
            BotSettings = botSettings;
        }

        public async Task Init()
        {
            await CommandHandler.InstallCommandsAsync();

            Client.Log += Logger.Log;

            await Client.LoginAsync(TokenType.Bot, BotSettings.BotToken);
            await Client.StartAsync();
        }
    }
}
