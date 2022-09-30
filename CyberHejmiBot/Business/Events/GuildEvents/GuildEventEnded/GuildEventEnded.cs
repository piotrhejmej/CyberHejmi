using CyberHejmiBot.Business.Jobs;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Events.GuildEvents.GuildEventEndedScope
{
    public record GuildEventEndedCommand(SocketGuildEvent guildEvent) : IRequest<Unit>;

    public class GuildEventEndedHandler : IRequestHandler<GuildEventEndedCommand, Unit>
    {
        private readonly IRemoveEventTextChannelJob RemoveEventTextChannelJob;
        private readonly DiscordSocketClient Client;

        public GuildEventEndedHandler(IRemoveEventTextChannelJob removeEventTextChannelJob, DiscordSocketClient client)
        {
            RemoveEventTextChannelJob = removeEventTextChannelJob;
            Client = client;
        }

        public async Task<Unit> Handle(GuildEventEndedCommand request, CancellationToken cancellationToken)
        {
            var guild = request.guildEvent.Guild;
            var textChannel = guild.Channels.FirstOrDefault(ch => ch.Name.Replace('-', ' ').Equals(request.guildEvent.Name, StringComparison.OrdinalIgnoreCase));

            if (textChannel == null)
                return Unit.Value;

            var restChannel = (await Client
                .Rest
                .GetChannelAsync(textChannel.Id)) as RestTextChannel;

            if (restChannel == null)
                return Unit.Value;

            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("All good thing once ends...")
                .WithDescription($"{request.guildEvent.Name} ended.\nChannel will be up and running for another Day\nGather memories, backup your photos and see you next time!");

            await restChannel.SendMessageAsync(embed: embedBuilder.Build());

            RemoveEventTextChannelJob.ScheduleRemoveTextChannel(request, textChannel.Id);

            return Unit.Value;
        }
    }
}
