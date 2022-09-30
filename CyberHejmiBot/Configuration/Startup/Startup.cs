using CyberHejmiBot.Business.Events;
using CyberHejmiBot.Business.SlashCommands;
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
        private readonly ISlashCommandsConfig SlashCommandsConfig;

        public Startup(DiscordSocketClient client,
                       TextCommandHandler commandHandler,
                       ILogger logger,
                       BotSettings botSettings,
                       IEventListener eventListener,
                       ISlashCommandsConfig slashCommandsConfig)
        {
            Client = client;
            CommandHandler = commandHandler;
            Logger = logger;
            BotSettings = botSettings;
            EventListener = eventListener;
            SlashCommandsConfig = slashCommandsConfig;
        }

        public async Task Init()
        {
            await CommandHandler.InstallCommandsAsync();
            await SlashCommandsConfig.RegisterSlashCommands();
            await EventListener.StartAsync();

            Client.Log += Logger.Log;
            
            await Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT_TOKEN"));
            await Client.StartAsync();

        }
    }
}
