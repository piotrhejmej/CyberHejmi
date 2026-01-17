using CyberHejmiBot.Data.Entities.Facts;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands.RandomFacts
{
    public class RandomDailyFactSubscribtionHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        public override string CommandName => "subscribe-random-fact";

        public override string Description => "Subscribes to daily random fact on selected channel";

        private readonly ICollection<AdditionalOption> AdditionalOptions = new List<AdditionalOption>()
        {
            new AdditionalOption("channel", "Channel where random facts will be sent daily", true, ApplicationCommandOptionType.Channel)
        };
        private readonly LocalDbContext DbContext;

        public RandomDailyFactSubscribtionHandler(DiscordSocketClient client, ILogger<RandomDailyFactSubscribtionHandler> logger, LocalDbContext dbContext) : base(client, logger)
        {
            Client = client;
            Logger = logger;
            DbContext = dbContext;
        }
        public override Task Register()
        {
            return base.Register(this.AdditionalOptions);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if ((await base.DoWork(command)))
                return false;

            var channel = (SocketGuildChannel?)command.Data.Options.FirstOrDefault(r => r.Name == "channel")?.Value;

            if (channel is null)
            {
                await command.RespondAsync("Wrong channel");
                return false;
            }

            var existingEntry = DbContext
                .FactsSubscriptions
                .FirstOrDefault(r => r.GuildId == channel.Guild.Id);

            if (existingEntry is not null)
                DbContext.Remove(existingEntry);

            await DbContext.AddAsync(new FactsSubscription()
            {
                ChannelId = channel.Id,
                GuildId = channel.Guild.Id
            });
            await DbContext.SaveChangesAsync();

            var embedBuilder = new EmbedBuilder()
               .WithColor(Color.Teal)
               .WithTitle("You have subscribed to daily facts!")
               .WithDescription($"Every day on channel {channel.Name} at 9 am I will post a little fact on event that happened on the same day in the past :)");

            await command.RespondAsync(embed: embedBuilder.Build());

            return true;
        }

    }
}
