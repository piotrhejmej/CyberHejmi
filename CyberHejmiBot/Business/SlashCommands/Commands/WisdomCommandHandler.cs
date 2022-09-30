using CyberHejmiBot.Configuration.Loging;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class WisdomCommandHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        override public string CommandName => "wisdom";
        override public string Description => "Returns highly motivational and extremely wise quote from one of the greatest minds of human history";
        private readonly LocalDbContext DbContext;

        public WisdomCommandHandler(DiscordSocketClient client, ILogger logger, LocalDbContext dbContext): base (client, logger)
        {
            Client = client;
            Logger = logger;
            DbContext = dbContext;
        }

        public override async Task DoWork(SocketSlashCommand command)
        {
            await base.DoWork(command);

            var rnd = new Random();
            var widomIds = await DbContext.WisdomEntries.Select(r => r.Id).ToArrayAsync();
            var wisdomIndex = widomIds[rnd.Next(widomIds.Length)];

            var wisdom = DbContext.WisdomEntries.FirstOrDefault(r => r.Id == wisdomIndex);

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Like wise {wisdom.AuthorName} once said:")
                .WithColor(Color.Green)
                .WithDescription(wisdom.Text);

            await command.RespondAsync(embed: embedBuilder.Build());
        }
    }
}
