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
using CyberHejmiBot.Business.Jobs;
using CyberHejmiBot.Business.SuperSpecial;
using System.Reflection;
using CyberHejmiBot.Business.SlashCommands;

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
                .AddScoped<IRemoveEventTextChannelJob, RemoveEventTextChannelJob>()
                .AddScoped<ISuperSpecialLover, SuperSpecialLover>()
                .AddScoped<ISlashCommandsConfig, SlashCommandsConfig>();

            collection.AddClassesAsImplementedInterface(Assembly.GetExecutingAssembly(), typeof(ISlashCommandHandler<>));
            
            return collection;
        }

        private static List<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
        {
            var typeInfoList = assembly.DefinedTypes.Where(x => x.IsClass
                                && !x.IsAbstract
                                && x != compareType
                                && x.GetInterfaces()
                                        .Any(i => i.IsGenericType
                                                && i.GetGenericTypeDefinition() == compareType))?.ToList();

            return typeInfoList;
        }

        private static void AddClassesAsImplementedInterface(
                this IServiceCollection services,
                Assembly assembly,
                Type compareType,
                ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly.GetTypesAssignableTo(compareType).ForEach((type) =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(implementedInterface, type);
                            break;
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(implementedInterface, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(implementedInterface, type);
                            break;
                    }
                }
            });
        }
    }
}