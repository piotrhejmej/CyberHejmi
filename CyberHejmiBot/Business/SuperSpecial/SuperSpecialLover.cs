using CyberHejmiBot.Static;
using Discord;
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

        public SuperSpecialLover(DiscordSocketClient client)
        {
            Client = client;
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
        }
    }
}
