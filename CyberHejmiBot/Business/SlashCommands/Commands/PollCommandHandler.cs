using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{

    internal class PollCommandHandler : ISlashCommandHandler<ISlashCommand>
    {
        public Task Register()
        {
            return Task.CompletedTask;
        }

        public Task DoWork()
        {
            return Task.CompletedTask;
        }
    }
}
