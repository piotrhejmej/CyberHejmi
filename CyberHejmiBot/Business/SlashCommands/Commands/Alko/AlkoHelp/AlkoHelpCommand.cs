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
        private static List<AlkoCommandInfo>? _cachedCommands;
        private static readonly object _lock = new();

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

            var commands = GetOrLoadCommands();
            
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🍺 Alko-Tracker Helpers 🍺")
                .WithDescription("Here are the commands you can use:")
                .WithColor(Color.Gold);

            foreach (var cmd in commands)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"> {cmd.Description}");
                
                if (cmd.Options.Any())
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

                embedBuilder.AddField($"/{cmd.Name}", sb.ToString());
            }

            try 
            {
                await command.User.SendMessageAsync(embed: embedBuilder.Build());
                await command.FollowupAsync("📬 I've sent the help list to your DMs!", ephemeral: true);
            }
            catch (Discord.Net.HttpException)
            {
                await command.FollowupAsync(
                    "❌ I couldn't send you a DM. Please check your privacy settings.",
                    ephemeral: true
                );
            }

            return true;
        }

        private List<AlkoCommandInfo> GetOrLoadCommands()
        {
            if (_cachedCommands != null)
                return _cachedCommands;

            // Build the command list outside the lock to avoid creating a service scope within the critical section.
            List<AlkoCommandInfo> commands;
            using (var scope = _serviceProvider.CreateScope())
            {
                var registeredCommands = scope.ServiceProvider.GetRequiredService<
                    IEnumerable<BaseSlashCommandHandler<ISlashCommand>>
                >();
                commands = registeredCommands
                    .OfType<IAlkoCommand>()
                    .Select(x => new AlkoCommandInfo(x.CommandName, x.Description, x.Options))
                    .ToList();
            }

            lock (_lock)
            {
                if (_cachedCommands == null)
                {
                    _cachedCommands = commands;
                }
            }

            return _cachedCommands;
        }

        private record AlkoCommandInfo(string Name, string Description, IReadOnlyList<AdditionalOption> Options);
    }
}
