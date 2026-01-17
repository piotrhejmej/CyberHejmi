using System.Text;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoHelp
{
    public class AlkoHelpCommand : BaseSlashCommandHandler<ISlashCommand>, IAlkoCommand
    {
        private readonly IServiceProvider _serviceProvider;

        public override string CommandName => "alko-help";
        public override string Description => "Lists available Alko commands and their parameters.";
        public IReadOnlyList<AdditionalOption> Options => Array.Empty<AdditionalOption>();

        public AlkoHelpCommand(
            DiscordSocketClient client,
            ILogger<AlkoHelpCommand> logger,
            IServiceProvider serviceProvider
        )
            : base(client, logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task<SlashCommandProperties> Register()
        {
            return await base.Register(Options);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if (await base.DoWork(command))
                return false;

            await command.DeferAsync(ephemeral: true);

            var sb = new StringBuilder();
            sb.AppendLine("## 🍺 Alko-Tracker Helpers 🍺");
            sb.AppendLine("Here are the commands you can use:");
            sb.AppendLine();

            using var scope = _serviceProvider.CreateScope();
            var commands = scope.ServiceProvider.GetRequiredService<
                IEnumerable<BaseSlashCommandHandler<ISlashCommand>>
            >();
            var alkoCommands = commands.OfType<IAlkoCommand>().ToList();

            foreach (var cmd in alkoCommands)
            {
                sb.AppendLine($"### `/{cmd.CommandName}`");
                sb.AppendLine($"> {cmd.Description}");

                if (cmd.Options != null && cmd.Options.Any())
                {
                    sb.AppendLine("**Parameters:**");
                    foreach (var opt in cmd.Options)
                    {
                        var req = opt.IsRequired ? "(Required)" : "(Optional)";
                        sb.AppendLine($"- `{opt.OptionName}`: {opt.Description} *{req}*");
                    }
                }
                else
                {
                    sb.AppendLine("*No parameters.*");
                }
                sb.AppendLine();
            }

            await command.FollowupAsync(sb.ToString(), ephemeral: true);
            return true;
        }
    }
}
