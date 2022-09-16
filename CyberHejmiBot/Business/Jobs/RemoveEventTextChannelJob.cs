using CyberHejmiBot.Business.Events.GuildEvents.GuildEventEndedScope;
using Discord.WebSocket;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Jobs
{
    public interface IRemoveEventTextChannelJob
    {
        void ScheduleRemoveTextChannel(GuildEventEndedCommand eventEnded, ulong channelId);
        Task RemoveTextChannel(ulong guildId, ulong textChannelId);
    }

    public class RemoveEventTextChannelJob: IRemoveEventTextChannelJob
    {
        private readonly DiscordSocketClient Client;

        public RemoveEventTextChannelJob(DiscordSocketClient client)
        {
            Client = client;
        }

        public void ScheduleRemoveTextChannel(GuildEventEndedCommand eventEnded, ulong channelId)
        {
            BackgroundJob.Schedule<IRemoveEventTextChannelJob>(
                x => x.RemoveTextChannel(eventEnded.guildEvent.Guild.Id, channelId),
                TimeSpan.FromSeconds(10));
        }

        public async Task RemoveTextChannel(ulong guildId, ulong textChannelId)
        {
            var guild = Client
                .Guilds
                .FirstOrDefault(r => r.Id == guildId);

            if (guild == null)
                return;

            var channel = guild
                .TextChannels
                .FirstOrDefault(r => r.Id == textChannelId);

            if (channel == null)
                return;

            await channel.DeleteAsync();
        }
    }
}
