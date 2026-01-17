using System;
using System.Linq;
using System.Threading.Tasks;
using CyberHejmiBot.Business.Common.Calculators;
using CyberHejmiBot.Business.Jobs.Recurring;
using CyberHejmiBot.Data.Entities.Alcohol;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.Jobs.Recurring
{
    public class YearlyAlcoStatsJob : IReccurringJob
    {
        private readonly LocalDbContext _dbContext;
        private readonly DiscordSocketClient _client;
        private readonly IAlkoStatsCalculator _calculator;
        private readonly ILogger<YearlyAlcoStatsJob> _logger;

        public YearlyAlcoStatsJob(
            LocalDbContext dbContext,
            DiscordSocketClient client,
            IAlkoStatsCalculator calculator,
            ILogger<YearlyAlcoStatsJob> logger
        )
        {
            _dbContext = dbContext;
            _client = client;
            _calculator = calculator;
            _logger = logger;
        }

        public void AddOrUpdate()
        {
            Hangfire.RecurringJob.AddOrUpdate<YearlyAlcoStatsJob>(
                x => x.DoWork(),
                Cron.Yearly(1, 1, 12, 0)
            );
        }

        public async Task DoWork()
        {
            try
            {
                var year = DateTime.UtcNow.Year - 1; // Stats for the previous year
                var logs = await _dbContext.AlkoStats.Where(x => x.Date.Year == year).ToListAsync();

                if (!logs.Any())
                    return;

                var userGroups = logs.GroupBy(x => x.UserId);

                foreach (var group in userGroups)
                {
                    var userId = group.Key;
                    var user = await _client.GetUserAsync(userId);

                    if (user == null)
                        continue;

                    try
                    {
                        var stats = _calculator.Calculate(group, year);
                        var embed = _calculator.BuildEmbed(
                            stats,
                            year,
                            $"Alcohol Stats for {year}",
                            $"Here is your summary for the year of {year}:"
                        );

                        await user.SendMessageAsync(embed: embed);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            $"Error sending yearly stats to user {userId} (likely DMs blocked)"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in YearlyAlcoStatsJob");
            }
        }
    }
}
