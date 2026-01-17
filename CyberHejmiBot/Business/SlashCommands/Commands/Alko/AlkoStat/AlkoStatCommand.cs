using CyberHejmiBot.Business.Common.Calculators;
using AlkoStatEntity = CyberHejmiBot.Data.Entities.Alcohol.AlkoStat;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoStat
{
    public class AlkoStatCommand : BaseSlashCommandHandler<ISlashCommand>, IAlkoCommand
    {
        private readonly LocalDbContext _dbContext;
        private readonly ILogger<AlkoStatCommand> _logger;
        private readonly IAlkoStatsCalculator _calculator;

        public override string CommandName => "alko-stat";
        public override string Description => "Shows alcohol statistics for the user.";

        private static readonly AdditionalOption[] _options = new[]
        {
            new AdditionalOption(
                "year",
                "Year to show stats for (defaults to current year)",
                false,
                ApplicationCommandOptionType.Integer
            )
        };

        public IReadOnlyList<AdditionalOption> Options => _options;

        public AlkoStatCommand(
            DiscordSocketClient client,
            LocalDbContext dbContext,
            ILogger<AlkoStatCommand> logger,
            IAlkoStatsCalculator calculator
        )
            : base(client, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _calculator = calculator;
        }

        public override async Task<SlashCommandProperties> Register()
        {
            return await base.Register(Options);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if (await base.DoWork(command))
                return false;

            try
            {
                await command.DeferAsync(ephemeral: true);
                var yearOption = command.Data.Options.FirstOrDefault(x => x.Name == "year")?.Value;
                var year = yearOption != null ? Convert.ToInt32(yearOption) : DateTime.Now.Year;

                var logs = await GetLogsForYear(command.User.Id, year);
                if (!logs.Any())
                {
                    await command.FollowupAsync($"No alcohol logs found for year {year}.", ephemeral: true);
                    return true;
                }

                await SendStats(command, logs, year);
                await command.FollowupAsync("Stats sent to your DM!", ephemeral: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {CommandName}");
                await command.FollowupAsync(
                    "An unexpected error occurred while generating stats.",
                    ephemeral: true
                );
            }

            return true;
        }


        private async Task<List<AlkoStatEntity>> GetLogsForYear(ulong userId, int year)
        {
            return await _dbContext
                .AlkoStats.Where(x => x.UserId == userId && x.Date.Year == year)
                .ToListAsync();
        }

        private async Task SendStats(SocketSlashCommand command, List<AlkoStatEntity> logs, int year)
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
                await command.FollowupAsync("Sent your stats via DM!", ephemeral: true);
            }
            catch (Discord.Net.HttpException ex)
            {
                _logger.LogWarning(
                    ex,
                    $"Could not send DM to user {command.User.Username} ({command.User.Id}) in {CommandName}"
                );
                await command.FollowupAsync(
                    "I couldn't send you a DM. Please check your privacy settings.",
                    ephemeral: true
                );
            }
        }
    }
}
