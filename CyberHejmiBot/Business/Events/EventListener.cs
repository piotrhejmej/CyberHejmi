using CyberHejmiBot.Business.Events.GuildEvents;
using Discord.WebSocket;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Events
{
    internal class EventListener: IEventListener
    {
        private readonly IGuildEventsListener GuildEventsListener;

        public EventListener(IGuildEventsListener guildEventsListener)
        {
            GuildEventsListener = guildEventsListener;
        }

        public async Task StartAsync()
        {
            await GuildEventsListener.StartAsync();
        }
    }
}
