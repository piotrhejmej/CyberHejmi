using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CyberHejmiBot.Business.SlashCommands
{
    internal interface ISlashCommandsConfig
    {
        Task RegisterSlashCommands();
    }

    internal class SlashCommandsConfig : ISlashCommandsConfig
    {
        private readonly IEnumerable<BaseSlashCommandHandler<ISlashCommand>> Things;
        private readonly DiscordSocketClient _client;

        public SlashCommandsConfig(IEnumerable<BaseSlashCommandHandler<ISlashCommand>> things, DiscordSocketClient client)
        {
            Things = things;
            _client = client;
        }

        public async Task RegisterSlashCommands()
        {
            var commandProperties = new List<ApplicationCommandProperties>();

            foreach (var thing in Things)
            {
                commandProperties.Add(await thing.Register());
            }

            await _client.BulkOverwriteGlobalApplicationCommandsAsync(commandProperties.ToArray());
        }
    }
}
