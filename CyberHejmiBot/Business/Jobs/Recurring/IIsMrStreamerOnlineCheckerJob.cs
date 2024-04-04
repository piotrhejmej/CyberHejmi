using CyberHejmiBot.Business.Common;
using CyberHejmiBot.Data.Entities.JobRelated;
using CyberHejmiBot.Entities;
using Discord.Rest;
using Discord.WebSocket;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace CyberHejmiBot.Business.Jobs.Recurring
{
    public class IIsMrStreamerOnlineCheckerJob : IReccurringJob
    {
        private readonly DiscordSocketClient Client;
        private readonly ITwitchChecker TwitchChecker;
        private readonly LocalDbContext DbContext;
        private const ulong CHANNEL_ID = 920773991394869248;

        public IIsMrStreamerOnlineCheckerJob(DiscordSocketClient client, ITwitchChecker twitchChecker, LocalDbContext dbContext)
        {
            Client = client;
            TwitchChecker = twitchChecker;
            DbContext = dbContext;
        }

        public void AddOrUpdate()
        {
            RecurringJob.AddOrUpdate<IIsMrStreamerOnlineCheckerJob>(x => x.DoWork(), "*/15 17-20 * * *");
        }

        public async Task DoWork()
        {
            if (await DbContext.MrStreamerCheckerLogs.AnyAsync())
            {
                var latestSuccessfulCheck = await DbContext
                    .MrStreamerCheckerLogs
                    .MaxAsync(r => r.LastSuccessfullCheck);

                if (latestSuccessfulCheck.Date == DateTime.Today.Date)
                    return;
            }

            var isMrStreamerOnline = await TwitchChecker.IsMrStreamerOnline();

            if (isMrStreamerOnline.IsSuccesfull && isMrStreamerOnline.Result)
            {
                await ClearPreviousEntries();
                var log = new MrStreamerCheckerLogs
                {
                    JobName = nameof(IIsMrStreamerOnlineCheckerJob),
                    LastSuccessfullCheck = DateTime.UtcNow
                };

                await DbContext.AddAsync(log);
                await DbContext.SaveChangesAsync();

                if (await Client.Rest.GetChannelAsync(CHANNEL_ID) is not RestTextChannel restChannel)
                    return;

                var embedded = new Discord.EmbedBuilder()
                {
                    Url = "https://www.twitch.tv/StreamKoderka",
                    ImageUrl = "https://media.tenor.com/rLrYjKCRnUUAAAAM/pingu-wave.gif"
                }
                .WithColor(Discord.Color.Gold)
                .WithTitle("ej bo Szymek streamuje");

                await restChannel.SendMessageAsync(embed: embedded.Build());
            }
        }

        public async Task ClearPreviousEntries()
        {
            var logs = await DbContext
                .MrStreamerCheckerLogs
                .ToListAsync();

            if (logs.Any())
            {
                DbContext.RemoveRange(logs);
                await DbContext.SaveChangesAsync();
            }
        }
    }
}
