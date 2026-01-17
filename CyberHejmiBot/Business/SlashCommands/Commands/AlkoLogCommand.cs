using System;
using System.Threading.Tasks;
using CyberHejmiBot.Business.SlashCommands;
using CyberHejmiBot.Data.Entities.Alcohol;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class AlkoLogCommand : BaseSlashCommandHandler<ISlashCommand>
    {
        private readonly LocalDbContext _dbContext;
        private readonly Microsoft.Extensions.Logging.ILogger<AlkoLogCommand> _logger;

        public override string CommandName => "alko-log";
        public override string Description =>
            "Log that you consumed alcohol today (or specified date)";

        public AlkoLogCommand(
            DiscordSocketClient client,
            LocalDbContext dbContext,
            ILogger<AlkoLogCommand> logger
        )
            : base(client, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task Register()
        {
            var options = new List<AdditionalOption>
            {
                new AdditionalOption(
                    "date",
                    "Date of consumption (DD-MM-YYYY) - Optional, defaults to today",
                    false,
                    ApplicationCommandOptionType.String
                ),
            };

            await base.Register(options);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if (await base.DoWork(command))
                return false;

            try
            {
                var dateOption =
                    command.Data.Options.FirstOrDefault(x => x.Name == "date")?.Value as string;
                var date = DateTime.UtcNow.Date;

                if (!string.IsNullOrEmpty(dateOption))
                {
                    if (
                        !DateTime.TryParseExact(
                            dateOption,
                            "dd-MM-yyyy",
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out date
                        )
                    )
                    {
                        await command.RespondAsync(
                            "Invalid date format. Please use DD-MM-YYYY.",
                            ephemeral: true
                        );
                        return true;
                    }
                }

                var entry = new AlkoStat
                {
                    UserId = command.User.Id,
                    Date = date,
                    CreatedAt = DateTime.UtcNow,
                };

                _dbContext.AlkoStats.Add(entry);
                await _dbContext.SaveChangesAsync();

                try
                {
                    await command.User.SendMessageAsync(
                        $"Logged alcohol consumption for {date:dd-MM-yyyy}."
                    );
                    await command.RespondAsync("Done! Check your DMs.", ephemeral: true);
                }
                catch (Discord.Net.HttpException ex)
                {
                    _logger.LogWarning(
                        ex,
                        $"Could not send DM to user {command.User.Username} ({command.User.Id}) in {CommandName}"
                    );
                    await command.RespondAsync(
                        "I couldn't send you a DM. Please check your privacy settings.",
                        ephemeral: true
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {CommandName}");
                await command.RespondAsync(
                    "An unexpected error occurred. Administrators have been notified.",
                    ephemeral: true
                );
            }

            return true;
        }
    }
}
