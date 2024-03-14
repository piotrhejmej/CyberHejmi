using CyberHejmiBot.Configuration.Hangfire;
using CyberHejmiBot.Configuration.Startup;
using CyberHejmiBot.Entities;
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