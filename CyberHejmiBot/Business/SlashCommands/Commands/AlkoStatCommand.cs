using CyberHejmiBot.Business.SlashCommands;
using CyberHejmiBot.Entities;
using CyberHejmiBot.Data.Entities.Alcohol;
using CyberHejmiBot.Business.Common.Calculators;
using CyberHejmiBot.Configuration.Logging.DebugLogger;
using CyberHejmiBot.Configuration.Loging;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class AlkoStatCommand : BaseSlashCommandHandler<ISlashCommand>
    {
        private readonly LocalDbContext _dbContext;
        private readonly IAlkoStatsCalculator _calculator;
        private readonly IDebugLogger _debugLogger;

        public override string CommandName => "alko-stat";
        public override string Description => "Get your alcohol consumption statistics";

        public AlkoStatCommand(DiscordSocketClient client, ILogger logger, LocalDbContext dbContext, IAlkoStatsCalculator calculator, IDebugLogger debugLogger) 
            : base(client, logger)
        {
            _dbContext = dbContext;
            _calculator = calculator;
            _debugLogger = debugLogger;
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
                var userId = command.User.Id;

                var logs = await _dbContext.AlkoStats
                    .Where(x => x.UserId == userId && x.Date.Year == year)
                    .ToListAsync();

                if (!logs.Any())
                {
                    await command.RespondAsync("No alcohol consumption logged for this year.", ephemeral: true);
                    return true;
                }

                var stats = _calculator.Calculate(logs, year);
                var embed = _calculator.BuildEmbed(stats, year, $"Alcohol Stats for {year}", $"Here are your stats for {year}:");

                try
                {
                    await command.User.SendMessageAsync(embed: embed);
                    await command.RespondAsync("Sent your stats via DM!", ephemeral: true);
                }
                catch (Discord.Net.HttpException ex)
                {
                     _debugLogger.LogWarning($"Could not send DM to user {command.User.Username} ({command.User.Id}) in {CommandName}", ex);
                    await command.RespondAsync("I couldn't send you a DM. Please check your privacy settings.", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                 _debugLogger.LogError($"Error in {CommandName}", ex);
                 await command.RespondAsync("An unexpected error occurred. Administrators have been notified.", ephemeral: true);
            }

            return true;
        }
    }
}
