using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;

namespace CyberHejmiBot.Business.TextCommands
{
    public class TextCommandHandler
    {
        private readonly DiscordSocketClient Client;
        private readonly CommandService Commands;
        private readonly IServiceProvider ServiceProvider;

        public TextCommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider)
        {
            Client = client;
            Commands = commands;
            ServiceProvider = serviceProvider;
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
