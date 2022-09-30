using CyberHejmiBot.Data.Entities.Wisdom;
using CyberHejmiBot.Entities;
using CyberHejmiBot.Static;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SuperSpecial
{
    public interface ISuperSpecialLover
    {
        Task SendLove(SocketMessage messageParam);
    }

    public class SuperSpecialLover : ISuperSpecialLover
    {
        private readonly DiscordSocketClient Client;
        private readonly LocalDbContext DbContext;

        public SuperSpecialLover(DiscordSocketClient client, LocalDbContext dbContext)
        {
            Client = client;
            DbContext = dbContext;
        }

        public async Task SendLove(SocketMessage messageParam)
        {
            var rnd = new Random();

            if (!(rnd.Next(100) % 10 == 0))
                return;

            var guild = Client
                .Guilds
                .FirstOrDefault(r => r.Channels.Any(r => r.Id == messageParam.Channel.Id));

            if (guild is null)
                return;

            var customEmotes = guild.Emotes.ToArray();
            var emojis = Emojis.UnicodeArray.Select(r => new Emoji(r)).ToArray();


            if (customEmotes.Length == 0 || rnd.Next(10) % 2 == 0)
            {
                var randomEmoji = emojis[rnd.Next(emojis.Length - 1)];
                await messageParam.AddReactionAsync(randomEmoji);
            }
            else
            {
                var randomEmote = customEmotes[rnd.Next(customEmotes.Length - 1)];
                await messageParam.AddReactionAsync(randomEmote);
            }

            if ((rnd.Next(100) % 3 == 0))
                await LogMesage(messageParam);
        }

        private async Task LogMesage(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;
            if ((message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(Client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                    return;

            await DbContext.AddAsync(new WisdomEntry()
            {
                AuthorName = message.Author.Username,
                Text = messageParam.Content
            });
            await DbContext.SaveChangesAsync();
        }
    }
}
