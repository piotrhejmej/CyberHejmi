using CyberHejmiBot.Business.Common.Calculators;
using CyberHejmiBot.Data.Entities.Alcohol;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class AlkoStatCommand : BaseSlashCommandHandler<ISlashCommand>
    {
        private readonly LocalDbContext _dbContext;
        private readonly IAlkoStatsCalculator _calculator;
        private readonly Microsoft.Extensions.Logging.ILogger<AlkoStatCommand> _logger;

        public override string CommandName => "alko-stat";
        public override string Description => "Get your alcohol consumption statistics";

        public AlkoStatCommand(
            DiscordSocketClient client,
            LocalDbContext dbContext,
            IAlkoStatsCalculator calculator,
            ILogger<AlkoStatCommand> logger
        )
            : base(client, logger)
        {
            _dbContext = dbContext;
            _calculator = calculator;
            _logger = logger;
        }

        public override async Task Register()
        {
            await base.Register();
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if (await base.DoWork(command))
                return false;

            try
            {
                var year = DateTime.UtcNow.Year;
                var logs = await GetLogsForYear(command.User.Id, year);

                if (!logs.Any())
                {
                    await command.RespondAsync(
                        "No alcohol consumption logged for this year.",
                        ephemeral: true
                    );
                    return true;
                }

                await SendStats(command, logs, year);
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


        private async Task<List<AlkoStat>> GetLogsForYear(ulong userId, int year)
        {
            return await _dbContext
                .AlkoStats.Where(x => x.UserId == userId && x.Date.Year == year)
                .ToListAsync();
        }

        private async Task SendStats(SocketSlashCommand command, List<AlkoStat> logs, int year)
        {
            var stats = _calculator.Calculate(logs, year);
            var embed = _calculator.BuildEmbed(
                stats,
                year,
                $"Alcohol Stats for {year}",
                $"Here are your stats for {year}:"
            );

            try
            {
                await command.User.SendMessageAsync(embed: embed);
                await command.RespondAsync("Sent your stats via DM!", ephemeral: true);
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
    }
}
