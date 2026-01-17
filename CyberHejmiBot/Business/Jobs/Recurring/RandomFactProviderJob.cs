using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberHejmiBot.Business.Common;
using CyberHejmiBot.Data.Entities.Birthdays;
using CyberHejmiBot.Data.Entities.Facts;
using CyberHejmiBot.Entities;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.Jobs.Recurring
{
    public class RandomFactProviderJob : IReccurringJob
    {
        private readonly DiscordSocketClient Client;
        private readonly LocalDbContext DbContext;
        private readonly ILogger<RandomFactProviderJob> Logger;

        public RandomFactProviderJob(
            DiscordSocketClient client,
            LocalDbContext dbContext,
            ILogger<RandomFactProviderJob> logger
        )
        {
            Client = client;
            DbContext = dbContext;
            Logger = logger;
        }

        public void AddOrUpdate()
        {
            RecurringJob.AddOrUpdate<RandomFactProviderJob>(x => x.DoWork(), Cron.Daily(7));
        }

        public async Task DoWork()
        {
            var subscriptions = DbContext.FactsSubscriptions.ToList();

            foreach (var subscription in subscriptions)
            {
                if (await BirthdayOverride(subscription))
                    continue;
            }
        }

        public async Task<bool> BirthdayOverride(FactsSubscription subscription)
        {
            var today = DateTime.UtcNow;

            var jubilees = DbContext
                .Birthdays.Where(b => b.GuildId == subscription.GuildId)
                .Where(r => r.Date.Month == today.Month && r.Date.Day == today.Day)
                .ToList();

            Logger.LogInformation(
                $"BirthdayOverride: Found {jubilees.Count} jubilees for guild {subscription.GuildId}."
            );

            if (jubilees.Any())
            {
                if (
                    await Client.Rest.GetChannelAsync(subscription.ChannelId)
                    is not RestTextChannel restChannel
                )
                    return false;

                var description =
                    $"**{String.Join(" i ", jubilees.Select(r => $"{r.Name} obchodzi {today.Year - r.Date.Year}"))} urodziny!** z tej okazji życzymy:\nStooo lat, stooo lat, niech żyje cumpel nam! \nI jeszcze jeden i jeszcze raz!\nPrzez ręce Maaaaaryiiiiii\nSto lat, sto lat, sto lat, sto lat niech żyje nam\n A KTO??";

                description = jubilees.Any(r => r.HasCusomDescription)
                    ? jubilees.First(r => r.HasCusomDescription).CustomDescription
                    : description;

                var embedBuilder = new EmbedBuilder()
                {
                    ImageUrl = jubilees.Any(r => r.HasCusomDescription)
                        ? null
                        : "https://media.tenor.com/dM2Tdvd4gsEAAAAC/wszystkiego-najlepszego.gif",
                }
                    .WithColor(Color.Gold)
                    .WithTitle(
                        $"On this day in {String.Join(" and ", jubilees.Select(r => r.Date.Year))}:"
                    )
                    .WithDescription(description);

                await restChannel.SendMessageAsync(embed: embedBuilder.Build());

                return true;
            }

            return false;
        }
    }
}
