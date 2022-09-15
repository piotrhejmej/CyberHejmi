using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Loging
{
    public interface ILogger
    {
        Task Log(LogMessage msg);
    }
}
