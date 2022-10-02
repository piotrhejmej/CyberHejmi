using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Common
{
    public record WikiEventEntry
    {
        public string Description { get; set; }
        public string Year { get; set; }
    }

    public record WikiFactsOfDayResponse
    {
        public ICollection<WikiEventEntry> Births { get; set; }
        public ICollection<WikiEventEntry> Deaths { get; set; }
        public ICollection<WikiEventEntry> Events { get; set; }
        public ICollection<WikiEventEntry> Holidays { get; set; }
        public ICollection<WikiEventEntry> Selected { get; set; }
    }
}
