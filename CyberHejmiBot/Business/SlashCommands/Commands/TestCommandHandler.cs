using CyberHejmiBot.Business.Jobs.Recurring;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class TestCommandHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        public override string CommandName => "test";

        public override string Description => "For test purposes";

        private readonly RandomFactProviderJob RandomFactProviderJob;

        public TestCommandHandler(RandomFactProviderJob randomFactProviderJob, DiscordSocketClient client, ILogger<TestCommandHandler> logger) : base(client, logger)
        {
            RandomFactProviderJob = randomFactProviderJob;
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if ((await base.DoWork(command)))
                return false;

            if (command.GuildId is null || command.ChannelId is null)
                return false;

            await RandomFactProviderJob.BirthdayOverride(new Data.Entities.Facts.FactsSubscription()
            {
                GuildId = command.GuildId.Value,
                ChannelId = command.ChannelId.Value
            });

            return true;
        }
    }
}
