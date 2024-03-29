﻿using CyberHejmiBot.Configuration.Hangfire;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Hangfire;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Events.GuildEvents.GuildEventCreatedScope
{
    internal record GuildEventCreated(SocketGuildEvent GuildEvent) : IRequest<Unit>;

    internal class GuildEventCreatedHandler : IRequestHandler<GuildEventCreated, Unit>
    {
        public async Task<Unit> Handle(GuildEventCreated request, CancellationToken cancellationToken)
        {
            var guild = request.GuildEvent.Guild;

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

            var textChannel = await guild.CreateTextChannelAsync(request.GuildEvent.Name, opt =>
            {
                opt.CategoryId = eventsChannel.Id;
                opt.Topic = request.GuildEvent.Description ?? "";
            });

            await textChannel.SendMessageAsync($"Welcome to {request.GuildEvent.Name}, a channel created for event!");

            return Unit.Value;
        }
    }
}
