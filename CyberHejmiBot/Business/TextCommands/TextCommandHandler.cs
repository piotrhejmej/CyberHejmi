using CyberHejmiBot.Business.Events.GuildEvents;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public class TextCommandHandler
{
    private readonly DiscordSocketClient Client;
    private readonly CommandService Commands;
    private readonly IServiceProvider ServiceProvider;

    // Retrieve client and CommandService instance via ctor
    public TextCommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider)
    {
        Commands = commands;
        Client = client;
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
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        int argPos = 0;
        var hehe = message.HasCharPrefix('!', ref argPos);
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