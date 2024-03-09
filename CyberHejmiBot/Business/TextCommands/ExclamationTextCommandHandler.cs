using CyberHejmiBot.Business.SuperSpecial;
using CyberHejmiBot.Configuration.Loging;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace CyberHejmiBot.Business.TextCommands
{
    public class ExclamationTextCommandHandler
    {
        private readonly DiscordSocketClient Client;
        private readonly CommandService Commands;
        private readonly IServiceProvider ServiceProvider;
        private readonly ILogger Logger;

        public ExclamationTextCommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider, ILogger logger)
        {
            Client = client;
            Commands = commands;
            ServiceProvider = serviceProvider;
            Logger = logger;
        }

        public async Task InstallCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;

            await Commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: ServiceProvider);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message) return;

            Console.WriteLine($"{messageParam.Channel.Name}:{messageParam.Channel.Id}");

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(Client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(Client, message);

            await Commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: ServiceProvider);
        }
    }
}