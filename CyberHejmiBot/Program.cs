using CyberHejmiBot.Configuration;
using CyberHejmiBot.Configuration.Startup;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

public class Program
{

    private readonly IServiceProvider _serviceProvider;

    public Program()
    {
        _serviceProvider = ServicesConfig.CreateProvider();
    }

    static void Main(string[] args)
        => new Program().RunAsync(args).GetAwaiter().GetResult();
        
    private DiscordSocketClient Client;

    public async Task RunAsync(string[] args)
    {
        var startup = _serviceProvider.GetRequiredService<IStartup>();
        await startup.Init();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private async Task EventCreated(SocketGuildEvent event1)
    {
        var evento = event1;
        await CreateTextChannel(evento);
    }

    private async Task EventEnded(SocketGuildEvent event1)
    {
        var evento = event1;

        var guild = event1.Guild;
        var textChannel = guild.Channels.FirstOrDefault(ch => ch.Name.Equals(evento.Name, StringComparison.OrdinalIgnoreCase));

        if (textChannel != null)
        {
            await textChannel.DeleteAsync();
        }
    }

    private async Task CreateTextChannel(SocketGuildEvent event1)
    {
        var guild = event1.Guild;
        RestCategoryChannel eventsChannel;
        if (!guild.CategoryChannels.Any(c => c.Name == "Events"))
        {
            eventsChannel = await guild.CreateCategoryChannelAsync("Events");
        }

        var eventsCat = guild.CategoryChannels.FirstOrDefault(r => r.Name == "Events");

        if (eventsCat == null)
            return;

        var textChannel = await guild.CreateTextChannelAsync(event1.Name, opt =>
        {
            opt.CategoryId = eventsCat.Id;
            opt.Topic = event1.Description;
        });

        await textChannel.SendMessageAsync($"Welcome to {event1.Name}, a channel created for event!");
    }
}