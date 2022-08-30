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
    internal record GuildEventEnded(SocketGuildEvent guildEvent) : IRequest<Unit>;

    internal class GuildEventEndedHandler : IRequestHandler<GuildEventEnded, Unit>
    {
        public async Task<Unit> Handle(GuildEventEnded request, CancellationToken cancellationToken)
        {
            var guild = request.guildEvent.Guild;
            var textChannel = guild.Channels.FirstOrDefault(ch => ch.Name.Equals(request.guildEvent.Name, StringComparison.OrdinalIgnoreCase));

            if (textChannel != null)
            {
                await textChannel.DeleteAsync();
            }

            return Unit.Value;
        }
    }
}
