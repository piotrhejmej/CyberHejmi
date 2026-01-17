using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{

    internal class PollCommandHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        public PollCommandHandler(DiscordSocketClient client, ILogger<PollCommandHandler> logger) : base(client, logger)
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
