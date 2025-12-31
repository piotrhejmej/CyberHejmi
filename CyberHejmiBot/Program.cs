using CyberHejmiBot.Business.Common;
using CyberHejmiBot.Configuration.Hangfire;
using CyberHejmiBot.Configuration.Logging.Hangfire;
using CyberHejmiBot.Configuration.Settings;
using CyberHejmiBot.Configuration.Startup;
using CyberHejmiBot.Entities;
using Discord.WebSocket;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CyberHejmiBot
{
    public class Program
    {
        static void Main()
            => Program.RunAsync().GetAwaiter().GetResult();

        public static async Task RunAsync()
        {
            using var services = ServicesConfig.CreateProvider();
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Environment.GetEnvironmentVariable("Db_ConnectionString"))
                .UseActivator(new HangfireJobActivator(services));

            var discordClient = services.GetService<DiscordSocketClient>();

            if (discordClient is null)
                throw new InvalidOperationException("DiscordSocketClient is not registered in services.");

            var logService = services.GetRequiredService<DiscordLogService>();

            GlobalConfiguration.Configuration
                .UseLogProvider(new DiscordHangfireLogProvider(logService));

            using var server = new BackgroundJobServer();

            var dbContext = services.GetRequiredService<LocalDbContext>();

            dbContext.Database.Migrate();
            dbContext.Seed();

            var recurringJobsConfig = services.GetRequiredService<IRecurringJobsConfig>();
            recurringJobsConfig.RegisterJobs();

            var startup = services.GetRequiredService<IStartup>();
            await startup.Init();

            await Task.Delay(-1);
        }
    }
}