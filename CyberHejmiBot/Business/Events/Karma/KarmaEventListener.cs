using CyberHejmiBot.Data.Entities.Karma;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Events.Karma
{
    public class KarmaEventListener : IKarmaEventListener
    {
        private readonly DiscordSocketClient _client;
        private readonly LocalDbContext _dbContext;

        public KarmaEventListener(DiscordSocketClient client, LocalDbContext dbContext)
        {
            _client = client;
            _dbContext = dbContext;
        }

        public Task StartAsync()
        {
            _client.ReactionAdded += OnReactionAdded;
            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
        {
            if (reaction.Emote.Name != "🍺")
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

            var userKarma = await _dbContext.UserKarma.FirstOrDefaultAsync(x => x.UserId == message.Author.Id);

            if (userKarma == null)
            {
                userKarma = new UserKarma
                {
                    UserId = message.Author.Id,
                    Points = 0
                };
                await _dbContext.UserKarma.AddAsync(userKarma);
            }

            userKarma.Points++;
            await _dbContext.SaveChangesAsync();
        }
    }
}
