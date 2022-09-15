using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Hangfire
{
    public class HangfireTest
    {
        private readonly DiscordSocketClient Client;

        public HangfireTest(DiscordSocketClient client)
        {
            Client = client;
        }

        public async Task DoSomething(ulong guildId, ulong channelId)
        {
            var b = Client.Guilds.Select(r => r.Name);

            foreach(var item in b)
            {
                Console.WriteLine(item);
            }

            await Client
                .Guilds
                .FirstOrDefault(r => r.Id == guildId)
                .TextChannels
                .FirstOrDefault(r => r.Id == channelId)
                .SendMessageAsync("Helloing");
        }
    }
}
