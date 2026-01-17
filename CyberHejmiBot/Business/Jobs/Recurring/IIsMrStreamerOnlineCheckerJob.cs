using CyberHejmiBot.Business.Common;
using CyberHejmiBot.Data.Entities.JobRelated;
using CyberHejmiBot.Entities;
using Discord.Rest;
using Discord.WebSocket;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.Jobs.Recurring
{
    public class IIsMrStreamerOnlineCheckerJob : IReccurringJob
    {
        private readonly DiscordSocketClient Client;
        private readonly ITwitchChecker TwitchChecker;
        private readonly LocalDbContext DbContext;
        private const ulong CHANNEL_ID = 920773991394869248;
        private readonly ILogger<IIsMrStreamerOnlineCheckerJob> Logger;

        public IIsMrStreamerOnlineCheckerJob(
            DiscordSocketClient client,
            ITwitchChecker twitchChecker,
            LocalDbContext dbContext,
            ILogger<IIsMrStreamerOnlineCheckerJob> logger
        )
        {
            Client = client;
            TwitchChecker = twitchChecker;
            DbContext = dbContext;
            Logger = logger;
        }

        public void AddOrUpdate()
        {
            RecurringJob.AddOrUpdate<IIsMrStreamerOnlineCheckerJob>(
                x => x.DoWork(),
                "*/15 17-19 * * *"
            );
        }

        public async Task DoWork()
        {
            if (await DbContext.MrStreamerCheckerLogs.AnyAsync())
            {
                var latestSuccessfulCheck = await DbContext.MrStreamerCheckerLogs.MaxAsync(r =>
                    r.LastSuccessfullCheck
                );

                if (latestSuccessfulCheck.Date == DateTime.Today.Date)
                    return;
            }

            var isMrStreamerOnline = await TwitchChecker.IsMrStreamerOnline();

            if (!isMrStreamerOnline.IsSuccesfull)
            {
                Logger.LogError(
                    new Exception(isMrStreamerOnline.Error),
                    "Error while checking if MrStreamer is online"
                );
                return;
            }

            if (isMrStreamerOnline.IsSuccesfull && isMrStreamerOnline.Result)
            {
                Logger.LogInformation("MrStreamer is online");

                Logger.LogInformation("Clearing previous entries");

                await ClearPreviousEntries();

                Logger.LogInformation("Adding new entry");

                var log = new MrStreamerCheckerLogs
                {
                    JobName = nameof(IIsMrStreamerOnlineCheckerJob),
                    LastSuccessfullCheck = DateTime.UtcNow,
                };

                await DbContext.AddAsync(log);
                await DbContext.SaveChangesAsync();

                Logger.LogInformation("Sending message to channel");

                if (
                    await Client.Rest.GetChannelAsync(CHANNEL_ID) is not RestTextChannel restChannel
                )
                    return;

                var embedded = new Discord.EmbedBuilder()
                {
                    Url = "https://www.twitch.tv/StreamKoderka",
                    ImageUrl = "https://media.tenor.com/rLrYjKCRnUUAAAAM/pingu-wave.gif",
                }
                    .WithColor(Discord.Color.Gold)
                    .WithTitle("ej bo Szymek streamuje");

                Logger.LogInformation("Message sent");

                await restChannel.SendMessageAsync(embed: embedded.Build());
            }
        }

        public async Task ClearPreviousEntries()
        {
            var logs = await DbContext.MrStreamerCheckerLogs.ToListAsync();

            if (logs.Any())
            {
                DbContext.RemoveRange(logs);
                await DbContext.SaveChangesAsync();
            }
        }
    }
}
