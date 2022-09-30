using CyberHejmiBot.Business.SlashCommands.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CyberHejmiBot.Business.SlashCommands
{
    internal interface ISlashCommandsConfig
    {
        Task RegisterSlashCommands();
    }

    internal class SlashCommandsConfig: ISlashCommandsConfig
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IEnumerable<ISlashCommandHandler<ISlashCommand>> Things;

        public SlashCommandsConfig(IServiceProvider serviceProvider, IEnumerable<ISlashCommandHandler<ISlashCommand>> things)
        {
            ServiceProvider = serviceProvider;
            Things = things;
        }

        public async Task RegisterSlashCommands()
        {
            foreach (var thing in Things)
                await thing.Register();
        }
    }
}
