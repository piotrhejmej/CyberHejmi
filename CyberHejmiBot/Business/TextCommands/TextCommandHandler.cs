using CyberHejmiBot.Business.Events.GuildEvents;
using CyberHejmiBot.Business.SuperSpecial;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CyberHejmiBot.Business.TextCommands
{
    public class TextCommandHandler
    {
        private readonly DiscordSocketClient Client;
        private readonly CommandService Commands;
        private readonly IServiceProvider ServiceProvider;
        private readonly ISuperSpecialLover SuperSpecialLover;

        public TextCommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider, ISuperSpecialLover superSpecialLover)
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

            Console.WriteLine($"{messageParam.Channel.Name}:{messageParam.Channel.Id}");

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