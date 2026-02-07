using CyberHejmiBot.Data.Entities.Karma;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.Events.Karma
{
    public class KarmaEventListener : IKarmaEventListener
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KarmaEventListener> _logger;

        public KarmaEventListener(DiscordSocketClient client, IServiceScopeFactory scopeFactory, ILogger<KarmaEventListener> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task StartAsync()
        {
            _client.ReactionAdded += OnReactionAdded;
            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
        {
            if (reaction.Emote.Name != "🍺" && reaction.Emote.Name != "🍻")
                return;

            var message = await messageCache.GetOrDownloadAsync();
            if (message == null)
                return;

            // Prevent self-karma
            if (message.Author.Id == reaction.UserId)
                return;

            // Prevent giving karma to bots
            if (message.Author.IsBot)
                return;

            // Ensure it's a guild channel
            if (channelCache.Value is not SocketGuildChannel guildChannel)
                return;

            var guildId = guildChannel.Guild.Id;

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();

                var userKarma = await dbContext.UserKarma
                    .FirstOrDefaultAsync(x => x.UserId == message.Author.Id && x.GuildId == guildId);

                if (userKarma == null)
                {
                    userKarma = new UserKarma
                    {
                        UserId = message.Author.Id,
                        GuildId = guildId,
                        Points = 0
                    };
                    await dbContext.UserKarma.AddAsync(userKarma);
                }

                userKarma.Points++;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
