using CyberHejmiBot.Configuration;
using CyberHejmiBot.Configuration.Hangfire;
using CyberHejmiBot.Configuration.Startup;
using CyberHejmiBot.Entities;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Hangfire;
using Hangfire.PostgreSql;
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
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Environment.GetEnvironmentVariable("Db_ConnectionString"))
                .UseActivator(new HangfireJobActivator(services));

            using (var server = new BackgroundJobServer())
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
}