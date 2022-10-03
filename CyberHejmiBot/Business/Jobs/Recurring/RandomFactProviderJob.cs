using CyberHejmiBot.Business.Common;
using CyberHejmiBot.Entities;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Jobs.Recurring
{
    public class RandomFactProviderJob : IReccurringJob
    {
        private readonly DiscordSocketClient Client;
        private readonly LocalDbContext DbContext;
        private readonly IRandomFactFetcher RandomFactFetcher;

        public RandomFactProviderJob(DiscordSocketClient client, LocalDbContext dbContext, IRandomFactFetcher randomFactFetcher)
        {
            Client = client;
            DbContext = dbContext;
            RandomFactFetcher = randomFactFetcher;
        }

        public void AddOrUpdate()
        {
            RecurringJob.AddOrUpdate<RandomFactProviderJob>(x => x.DoWork(), Cron.Daily(7));
        }

        public async Task DoWork()
        {
            var subscriptions = DbContext
                .FactsSubscriptions
                .ToList();

            foreach (var subscription in subscriptions)
            {
                var randomFactOfADay = await RandomFactFetcher
                    .GetRandomFactOfToday(FactType.Event);

                var restChannel = (await Client
                   .Rest
                   .GetChannelAsync(subscription.ChannelId)) as RestTextChannel;

                if (restChannel == null)
                    continue;

                var embedBuilder = new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .WithTitle($"Random fact for Today! On this day in {randomFactOfADay.Year}:")
                    .WithDescription(randomFactOfADay.Description);

                await restChannel.SendMessageAsync(embed: embedBuilder.Build());
            }
        }
    }
}
