using CyberHejmiBot.Business.Common;
using CyberHejmiBot.Configuration.Logging.DebugLogger;
using CyberHejmiBot.Entities;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Jobs.Recurring
{
    public class KingOfBeersJob : IReccurringJob
    {
        private readonly DiscordSocketClient _client;
        private readonly LocalDbContext _dbContext;
        private readonly IDebugLogger _logger;

        public KingOfBeersJob(DiscordSocketClient client, LocalDbContext dbContext, IDebugLogger logger)
        {
            _client = client;
            _dbContext = dbContext;
            _logger = logger;
        }

        public void AddOrUpdate()
        {
            RecurringJob.AddOrUpdate<KingOfBeersJob>(
                x => x.DoWork(),
                "0 19 1 * *", // 19:00 on the 1st of every month
                TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw")
            );
        }

        public async Task DoWork()
        {
            var subscriptions = await _dbContext.FactsSubscriptions.ToListAsync();

            foreach (var subscription in subscriptions)
            {
                if (await _client.Rest.GetChannelAsync(subscription.ChannelId) is not RestTextChannel channel)
                    continue;

                var topKarmaUser = await _dbContext.UserKarma
                    .Where(x => x.GuildId == channel.GuildId)
                    .OrderByDescending(k => k.Points)
                    .FirstOrDefaultAsync();

                if (topKarmaUser == null || topKarmaUser.Points == 0)
                    continue;

                var user = await _client.Rest.GetUserAsync(topKarmaUser.UserId);
                var userName = user?.Username ?? $"Użytkownik o ID {topKarmaUser.UserId}";

                var imageUrl = await GetRandomGifUrl();
                
                var embedBuilder = new EmbedBuilder()
                    .WithTitle("👑 Król Piwek Miesiąca! 👑")
                    .WithDescription($"W tym miesiącu tytuł Króla Piwek 🍺 zdobywa **{userName}** z wynikiem **{topKarmaUser.Points}** piwek! 🍺\n\nGratulujemy i życzymy smacznego! 🍻")
                    .WithColor(Color.Gold);

                if (imageUrl.StartsWith("http"))
                {
                    embedBuilder.WithImageUrl(imageUrl);
                    await channel.SendMessageAsync(embed: embedBuilder.Build());
                }
                else
                {
                    embedBuilder.WithImageUrl($"attachment://{Path.GetFileName(imageUrl)}");
                    await channel.SendFileAsync(imageUrl, text: null, embed: embedBuilder.Build());
                }

                // Reset stats for this guild
                await _dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{nameof(_dbContext.UserKarma)}\" WHERE \"GuildId\" = {{0}}", channel.GuildId);
            }
            
            _logger.LogInfo("KingOfBeersJob: Completed.");
        }

        private async Task<string> GetRandomGifUrl()
        {
            // 1% chance for Ficus
            if (Random.Shared.Next(100) == 0)
            {
                var ficusPath = Path.Combine(AppContext.BaseDirectory, ImageConstants.ResourcesFolder, ImageConstants.SpecialImagesFolder, ImageConstants.FicusFileName);
                if (File.Exists(ficusPath))
                {
                    return ficusPath;
                }
            }

            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync("https://g.tenor.com/v1/random?q=beer%20cheers&key=LIVDSRZULELA&limit=1");
                var result = JsonConvert.DeserializeObject<TenorResponse>(response);

                var gifUrl = result?.Results?.FirstOrDefault()?.Media?.FirstOrDefault()?.Gif?.Url;
                if (!string.IsNullOrEmpty(gifUrl))
                {
                    return gifUrl;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching GIF from Tenor", ex);
            }

            // Fallback to local files
            var resourcesPath = Path.Combine(AppContext.BaseDirectory, ImageConstants.ResourcesFolder, ImageConstants.ImagesFolder);
            var files = Directory.GetFiles(resourcesPath);
            
            if (files.Any())
            {
                return files[Random.Shared.Next(files.Length)];
            }

            return "https://media.giphy.com/media/BPJmthQ3YRwD6QqcVD/giphy.gif"; // Ultimate fallback
        }

        private class TenorResponse
        {
            public TenorResult[]? Results { get; set; }
        }

        private class TenorResult
        {
            public TenorMedia[]? Media { get; set; }
        }

        private class TenorMedia
        {
            public TenorGif? Gif { get; set; }
        }

        private class TenorGif
        {
            public string? Url { get; set; }
        }
    }
}
