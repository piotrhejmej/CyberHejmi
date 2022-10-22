using CyberHejmiBot.Configuration.Loging;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{

    internal class PollCommandHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        public PollCommandHandler(DiscordSocketClient client, ILogger logger) : base(client, logger)
        {
        }

        public override string CommandName => "poll";

        public override string Description => "not working yet";

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if ((await base.DoWork(command)))
                return false;

            return false;
        }
    }
}
