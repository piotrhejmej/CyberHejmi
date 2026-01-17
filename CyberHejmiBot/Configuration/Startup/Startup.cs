using CyberHejmiBot.Business.Events;
using CyberHejmiBot.Business.SlashCommands;
using CyberHejmiBot.Business.TextCommands;
using Microsoft.Extensions.Logging;
using Discord;
using Discord.WebSocket;

namespace CyberHejmiBot.Configuration.Startup
{
    interface IStartup
    {
        Task Init();
    }

    internal class Startup : IStartup
    {
        private readonly DiscordSocketClient Client;
        private readonly TextCommandHandler CommandHandler;
        private readonly ILogger<Startup> Logger;
        private readonly IEventListener EventListener;
        private readonly ISlashCommandsConfig SlashCommandsConfig;

        public Startup(
            DiscordSocketClient client,
            TextCommandHandler commandHandler,
            ILogger<Startup> logger,
            IEventListener eventListener,
            ISlashCommandsConfig slashCommandsConfig
        )
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
            Client.Log += LogAsync;
        }

        private Task LogAsync(LogMessage container)
        {
            switch (container.Severity)
            {
                case LogSeverity.Critical:
                    Logger.LogCritical(container.Exception, container.Message);
                    break;
                case LogSeverity.Error:
                    Logger.LogError(container.Exception, container.Message);
                    break;
                case LogSeverity.Warning:
                    Logger.LogWarning(container.Exception, container.Message);
                    break;
                case LogSeverity.Info:
                    Logger.LogInformation(container.Exception, container.Message);
                    break;
                case LogSeverity.Verbose:
                    Logger.LogDebug(container.Exception, container.Message);
                    break;
                case LogSeverity.Debug:
                    Logger.LogDebug(container.Exception, container.Message);
                    break;
            }

            return Task.CompletedTask;
        }

        public async Task Ready()
        {
            await CommandHandler.InstallCommandsAsync();
            await SlashCommandsConfig.RegisterSlashCommands();
            await EventListener.StartAsync();
        }
    }
}
