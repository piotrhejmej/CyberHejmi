using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberHejmiBot.Data.Entities.Alcohol
{
    public class AlkoStat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public ulong UserId { get; set; }
        public DateTime Date { get; set; }
        public int? AmountMl { get; set; }
        public float? Percentage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
