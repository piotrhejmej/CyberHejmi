namespace CyberHejmiBot.Data.Entities.Wisdom
{
    public record WisdomEntry
    {
        public int Id { get; set; }
        public string? AuthorName { get; set; }
        public string? Text { get; set; }
    }
}
