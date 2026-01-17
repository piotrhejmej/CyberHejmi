using CyberHejmiBot.Business.SlashCommands;
using CyberHejmiBot.Entities;
using CyberHejmiBot.Data.Entities.Alcohol;
using CyberHejmiBot.Configuration.Logging.DebugLogger;
using CyberHejmiBot.Configuration.Loging;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class AlkoLogCommand : BaseSlashCommandHandler<ISlashCommand>
    {
        private readonly LocalDbContext _dbContext;
        private readonly IDebugLogger _debugLogger;

        public override string CommandName => "alko-log";
        public override string Description => "Log that you consumed alcohol today (or specified date)";

        public AlkoLogCommand(DiscordSocketClient client, ILogger logger, LocalDbContext dbContext, IDebugLogger debugLogger) 
            : base(client, logger)
        {
            _dbContext = dbContext;
            _debugLogger = debugLogger;
        }

        public override async Task Register()
        {
            var options = new List<AdditionalOption>
            {
                new AdditionalOption("date", "Date of consumption (DD-MM-YYYY) - Optional, defaults to today", false, ApplicationCommandOptionType.String)
            };
            
            await base.Register(options);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if (await base.DoWork(command))
                return false;

            try
            {
                var dateOption = command.Data.Options.FirstOrDefault(x => x.Name == "date")?.Value as string;
                var date = DateTime.UtcNow.Date;

                if (!string.IsNullOrEmpty(dateOption))
                {
                    if (!DateTime.TryParseExact(dateOption, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out date))
                    {
                        await command.RespondAsync("Invalid date format. Please use DD-MM-YYYY.", ephemeral: true);
                        return true;
                    }
                }

                var entry = new AlkoStat
                {
                    UserId = command.User.Id,
                    Date = date,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.AlkoStats.Add(entry);
                await _dbContext.SaveChangesAsync();

                try
                {
                    await command.User.SendMessageAsync($"Logged alcohol consumption for {date:dd-MM-yyyy}.");
                    await command.RespondAsync("Done! Check your DMs.", ephemeral: true);
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
