using CyberHejmiBot.Business.Jobs.Recurring;
using CyberHejmiBot.Configuration.Loging;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class TestCommandHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        public override string CommandName => "test";

        public override string Description => "For test purposes";

        private readonly RandomFactProviderJob RandomFactProviderJob;

        public TestCommandHandler(RandomFactProviderJob randomFactProviderJob, DiscordSocketClient client, ILogger logger) : base(client, logger)
        {
            RandomFactProviderJob = randomFactProviderJob;
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            await RandomFactProviderJob.BirthdayOverride(new Data.Entities.Facts.FactsSubscription()
            {
                GuildId = command.GuildId.Value,
                ChannelId = command.ChannelId.Value
            });

            return true;
        }
    }
}
