using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands
{
    internal interface ISlashCommandHandler<T> where T: ISlashCommand
    {
        Task Register();
        Task DoWork();
    }
}
