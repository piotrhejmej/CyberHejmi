using CyberHejmiBot.Business.Events.GuildEvents;
using CyberHejmiBot.Business.Events.Karma;

namespace CyberHejmiBot.Business.Events
{
    internal class EventListener : IEventListener
    {
        private readonly IGuildEventsListener GuildEventsListener;
        private readonly IKarmaEventListener KarmaEventListener;

        public EventListener(IGuildEventsListener guildEventsListener, IKarmaEventListener karmaEventListener)
        {
            GuildEventsListener = guildEventsListener;
            KarmaEventListener = karmaEventListener;
        }

        public async Task StartAsync()
        {
            await GuildEventsListener.StartAsync();
            await KarmaEventListener.StartAsync();
        }
    }
}
