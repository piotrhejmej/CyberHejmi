using CyberHejmiBot.Business.Events;
using CyberHejmiBot.Business.Events.GuildEvents;
using CyberHejmiBot.Configuration.Loging;
using CyberHejmiBot.Configuration.Settings;
using Discord.Commands;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Startup
{
    internal static class ServicesConfig
    {
        public static IServiceProvider CreateProvider()
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

            collection
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(commandServiceConfig)
                .AddSingleton<CommandService>()
                .AddSingleton(botSettings)
                .AddScoped<TextCommandHandler>()
                .AddScoped<ILogger, ConsoleLogger>()
                .AddScoped<IGuildEventsListener, GuildEventsListener>()
                .AddScoped<IEventListener, EventListener>()
                .AddScoped<IStartup, Startup>();

            return collection;
        }
    }
}