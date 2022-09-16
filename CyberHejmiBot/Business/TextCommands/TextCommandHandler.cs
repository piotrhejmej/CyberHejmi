using CyberHejmiBot.Business.Events.GuildEvents;
using CyberHejmiBot.Business.SuperSpecial;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        if (messageParam.Author.Username == Environment.GetEnvironmentVariable("SUPER_SECRET"))
            await SuperSpecialLover.SendLove(messageParam);

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