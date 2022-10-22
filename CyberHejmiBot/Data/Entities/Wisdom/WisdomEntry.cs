﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Data.Entities.Wisdom
{
    public record WisdomEntry
    {
        public int Id { get; set; }
        public string? AuthorName { get; set; }
        public string? Text { get; set; }
    }
}
