using CyberHejmiBot.Business.SuperSpecial;
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
        private readonly ISuperSpecialLover SuperSpecialLover;

        public ExclamationTextCommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider, ISuperSpecialLover superSpecialLover)
        {
            Client = client;
            Commands = commands;
            ServiceProvider = serviceProvider;
            SuperSpecialLover = superSpecialLover;
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

            int argPos = 0;

            if (message.Author.IsBot)
                return;

            var context = new SocketCommandContext(Client, message);

            await Commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: ServiceProvider);
        }
    }
}