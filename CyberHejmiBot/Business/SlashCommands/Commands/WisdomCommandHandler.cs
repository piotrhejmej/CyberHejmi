using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class WisdomCommandHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        override public string CommandName => "wisdom";
        override public string Description => "Returns highly motivational and extremely wise quote from one of the greatest minds of human history";
        private readonly LocalDbContext DbContext;

        public WisdomCommandHandler(DiscordSocketClient client, ILogger<WisdomCommandHandler> logger, LocalDbContext dbContext) : base(client, logger)
        {
            Client = client;
            Logger = logger;
            DbContext = dbContext;
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if ((await base.DoWork(command)))
                return false;

            var count = await DbContext.WisdomEntries.CountAsync();
            if (count == 0)
                return false;

            var index = Random.Shared.Next(count);
            var wisdom = await DbContext.WisdomEntries.Skip(index).Take(1).FirstOrDefaultAsync();

            if (wisdom is null)
                return false;

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Like wise {wisdom.AuthorName} once said:")
                .WithColor(Color.Green)
                .WithDescription(wisdom.Text);

            await command.RespondAsync(embed: embedBuilder.Build());
            return true;
        }
    }
}
