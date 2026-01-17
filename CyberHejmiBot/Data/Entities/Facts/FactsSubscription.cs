namespace CyberHejmiBot.Data.Entities.Facts
{
    public record FactsSubscription
    {
        public int Id { get; set; }
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
    }
}
