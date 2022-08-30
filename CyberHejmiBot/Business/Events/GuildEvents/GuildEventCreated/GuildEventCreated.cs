using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Events.GuildEvents.GuildEventCreatedScope
{
    internal record GuildEventCreated(SocketGuildEvent guildEvent) : IRequest<Unit>;

    internal class GuildEventCreatedHandler : IRequestHandler<GuildEventCreated, Unit>
    {
        public async Task<Unit> Handle(GuildEventCreated request, CancellationToken cancellationToken)
        {
            var guild = request.guildEvent.Guild;

            ICategoryChannel? eventsChannel;

            if (!guild.CategoryChannels.Any(c => c.Name == "Events"))
            {
                eventsChannel = await guild.CreateCategoryChannelAsync("Events");
            }
            else
            {
                eventsChannel = guild.CategoryChannels.FirstOrDefault(r => r.Name == "Events");
            }


            if (eventsChannel is null)
                return Unit.Value;

            var textChannel = await guild.CreateTextChannelAsync(request.guildEvent.Name, opt =>
            {
                opt.CategoryId = eventsChannel.Id;
                opt.Topic = request.guildEvent.Description ?? "";
            });

            await textChannel.SendMessageAsync($"Welcome to {request.guildEvent.Name}, a channel created for event!");
            return Unit.Value;
        }
    }
}
