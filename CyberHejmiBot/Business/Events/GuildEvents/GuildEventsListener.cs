using CyberHejmiBot.Business.Events.GuildEvents.GuildEventCreatedScope;
using CyberHejmiBot.Business.Events.GuildEvents.GuildEventEndedScope;
using Discord.WebSocket;
using MediatR;

namespace CyberHejmiBot.Business.Events.GuildEvents
{
    internal class GuildEventsListener : IGuildEventsListener
    {
        private readonly DiscordSocketClient Client;
        private readonly IMediator Mediator;

        public GuildEventsListener(DiscordSocketClient client, IMediator mediator)
        {
            Client = client;
            Mediator = mediator;
        }

        public Task StartAsync()
        {
            Client.GuildScheduledEventCreated += (SocketGuildEvent guildEvent) => Mediator.Send(new GuildEventCreated(guildEvent));
            Client.GuildScheduledEventCompleted += (SocketGuildEvent guildEvent) => Mediator.Send(new GuildEventEndedCommand(guildEvent));
            Client.GuildScheduledEventCancelled += (SocketGuildEvent guildEvent) => Mediator.Send(new GuildEventEndedCommand(guildEvent));

            return Task.FromResult(Unit.Value);
        }
    }
}
