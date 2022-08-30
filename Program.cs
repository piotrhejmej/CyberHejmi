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

    public async Task RunAsync(string[] args)
    {
        var startup = _serviceProvider.GetRequiredService<IStartup>();
        await startup.Init();

        await Task.Delay(-1);
    }
}