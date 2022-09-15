using CyberHejmiBot.Business.Events;
using CyberHejmiBot.Business.Events.GuildEvents;
using CyberHejmiBot.Configuration.Loging;
using CyberHejmiBot.Configuration.Settings;
using CyberHejmiBot.Entities;
using Discord.Commands;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Hangfire;
using Hangfire.PostgreSql;
using CyberHejmiBot.Configuration.Hangfire;

namespace CyberHejmiBot.Configuration.Startup
{
    internal static class ServicesConfig
    {
        public static ServiceProvider CreateProvider()
        {
            var serviceCollection = RegisterServices();

            serviceCollection
                .AddMediatR(typeof(Program));

            return serviceCollection.BuildServiceProvider();
        }

        private static IServiceCollection RegisterServices()
        {
            var collection = new ServiceCollection();
           
            var config = new DiscordSocketConfig();
            var commandServiceConfig = new CommandServiceConfig();
            var botSettings = JsonConvert.DeserializeObject<BotSettings>(File.ReadAllText("BotSettings.json"))
                ?? new BotSettings();

            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "BotSettings.json");

            var configuration = _builder.Build();

            collection
                .AddSingleton(configuration)
                .AddDbContext<LocalDbContext>()
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(commandServiceConfig)
                .AddSingleton<CommandService>()
                .AddSingleton(botSettings)
                .AddScoped<TextCommandHandler>()
                .AddScoped<ILogger, ConsoleLogger>()
                .AddScoped<IGuildEventsListener, GuildEventsListener>()
                .AddScoped<IEventListener, EventListener>()
                .AddScoped<IStartup, Startup>()
                .AddScoped<HangfireTest>();

            return collection;
        }
    }
}