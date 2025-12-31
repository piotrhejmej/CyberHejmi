using System.ComponentModel.DataAnnotations;

namespace CyberHejmiBot.Data.Entities.Karma
{
    public class UserKarma
    {
        [Key]
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public int Points { get; set; }
    }
}
