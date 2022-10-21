using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Data.Entities.Birthdays
{
    public record Birthday
    {
        public Birthday(ulong guildId, string name, DateTime date, string? customDescription = null)
        {
            GuildId = guildId;
            Name = name;
            Date = date;
            CustomDescription = customDescription;
        }

        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string? CustomDescription { get; set; }
        public bool HasCusomDescription => !String.IsNullOrEmpty(CustomDescription);
    }
}
