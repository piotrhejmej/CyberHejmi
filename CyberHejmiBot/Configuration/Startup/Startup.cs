﻿using CyberHejmiBot.Business.Events;
using CyberHejmiBot.Business.SlashCommands;
using CyberHejmiBot.Business.TextCommands;
using CyberHejmiBot.Configuration.Loging;
using Discord;
using Discord.WebSocket;

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
        private readonly IEventListener EventListener;
        private readonly ISlashCommandsConfig SlashCommandsConfig;

        public Startup(DiscordSocketClient client,
                       TextCommandHandler commandHandler,
                       ILogger logger,
                       IEventListener eventListener,
                       ISlashCommandsConfig slashCommandsConfig)
        {
            Client = client;
            CommandHandler = commandHandler;
            Logger = logger;
            EventListener = eventListener;
            SlashCommandsConfig = slashCommandsConfig;
        }

        public async Task Init()
        {
            Console.WriteLine(Environment.GetEnvironmentVariable("BOT_TOKEN"));
            await Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT_TOKEN"));
            await Client.StartAsync();
            Client.Ready += Ready;
            Client.Log += Logger.Log;
        }

        public async Task Ready()
        {
            await CommandHandler.InstallCommandsAsync();
            await SlashCommandsConfig.RegisterSlashCommands();
            await EventListener.StartAsync();
        }
    }
}
