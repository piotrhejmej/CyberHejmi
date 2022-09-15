using CyberHejmiBot.Configuration;
using CyberHejmiBot.Configuration.Startup;
using CyberHejmiBot.Entities;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    static void Main(string[] args)
        => new Program().RunAsync(args).GetAwaiter().GetResult();

    public async Task RunAsync(string[] args)
    {
        using (var services = ServicesConfig.CreateProvider())
        {
            var dbContext = services.GetRequiredService<LocalDbContext>();
            
            dbContext.Database.Migrate();
            dbContext.Seed();

            var startup = services.GetRequiredService<IStartup>();
            await startup.Init();

            await Task.Delay(-1);
        }
    }
}