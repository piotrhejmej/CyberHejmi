using CyberHejmiBot.Configuration.Hangfire;
using CyberHejmiBot.Configuration.Logging.Hangfire;
using CyberHejmiBot.Configuration.Startup;
using CyberHejmiBot.Entities;
using Discord.WebSocket;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CyberHejmiBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Initialize Hangfire Config - this was previously in RunAsync
                    GlobalConfiguration
                        .Configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseColouredConsoleLogProvider()
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UsePostgreSqlStorage(
                            Environment.GetEnvironmentVariable("Db_ConnectionString")
                        )
                        .UseActivator(new HangfireJobActivator(services));

                    var discordClient = services.GetRequiredService<DiscordSocketClient>();

                    GlobalConfiguration.Configuration.UseLogProvider(
                        new DiscordLoggerProvider(discordClient)
                    );

                    // We need a background job server.
                    // In a console app with Host.Run, usually services are hosted services.
                    // But here we are manually managing it for now to match legacy behavior or use Using.
                    // However, 'using var server' inside Main means it disposes when Main exits.
                    // 'host.Run()' blocks until shutdown.

                    using var server = new BackgroundJobServer();

                    var dbContext = services.GetRequiredService<LocalDbContext>();
                    dbContext.Database.Migrate();
                    dbContext.Seed();

                    var recurringJobsConfig = services.GetRequiredService<IRecurringJobsConfig>();
                    recurringJobsConfig.RegisterJobs();

                    var startup = services.GetRequiredService<IStartup>();
                    await startup.Init();

                    // Run the host (blocks)
                    await host.RunAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during startup: {ex.Message}");
                    throw;
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        ServicesConfig.Register(services);
                    }
                );
    }
}
