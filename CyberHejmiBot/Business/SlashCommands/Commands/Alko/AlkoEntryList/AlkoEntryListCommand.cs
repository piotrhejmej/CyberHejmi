using System.Text;
using AlkoStatEntity = CyberHejmiBot.Data.Entities.Alcohol.AlkoStat;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoEntryList
{
    public class AlkoEntryListCommand : BaseSlashCommandHandler<ISlashCommand>
    {
        private readonly LocalDbContext _dbContext;
        private readonly ILogger<AlkoEntryListCommand> _logger;
        private readonly AlkoEntryListValidator _validator;
        private const int PageSize = 10;

        public override string CommandName => "alko-entry-list";
        public override string Description => "Lists your alcohol log entries.";

        public AlkoEntryListCommand(
            DiscordSocketClient client,
            LocalDbContext dbContext,
            ILogger<AlkoEntryListCommand> logger,
            AlkoEntryListValidator validator
        ) : base(client, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _validator = validator;
        }

        public override async Task<SlashCommandProperties> Register()
        {
            var options = new List<AdditionalOption>
            {
                new AdditionalOption(
                    "page",
                    "Page number (default 0)",
                    false,
                    ApplicationCommandOptionType.Integer
                )
            };

            return await base.Register(options);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if (await base.DoWork(command))
                return false;

            try
            {
                await command.DeferAsync(ephemeral: true);
                
                var pageOption = command.Data.Options.FirstOrDefault(x => x.Name == "page")?.Value as long?;
                
                if (!_validator.ValidatePage(pageOption, out var page, out var error))
                {
                    await command.FollowupAsync(error, ephemeral: true);
                    return true;
                }

                var (entries, totalCount) = await FetchEntries(command.User.Id, page);

                var response = BuildResponse(entries, totalCount, page);
                await command.FollowupAsync(response, ephemeral: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {CommandName}");
                await command.FollowupAsync("An error occurred while fetching the list.", ephemeral: true);
            }

            return true;
        }

        private int GetPage(SocketSlashCommand command)
        {
            var pageOption = command.Data.Options.FirstOrDefault(x => x.Name == "page")?.Value;
            var page = pageOption != null ? Convert.ToInt32(pageOption) : 0;
            return Math.Max(0, page);
        }

        private async Task<(List<AlkoStatEntity> entries, int totalCount)> FetchEntries(ulong userId, int page)
        {
            var baseQuery = _dbContext.AlkoStats.Where(x => x.UserId == userId);
            
            var totalCount = await baseQuery.CountAsync();
            
            var entries = await baseQuery
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.CreatedAt)
                .Skip(page * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return (entries, totalCount);
        }

        private string BuildResponse(List<AlkoStatEntity> entries, int totalCount, int page)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"**Alcohol Logs (Page {page}) - Total: {totalCount}**");
            sb.AppendLine("```");
            sb.AppendLine($"{"Date",-12} | {"Amount",-8} | {"%",-4} | {"ID"}");
            sb.AppendLine(new string('-', 60));

            foreach (var entry in entries)
            {
                var dateStr = entry.Date.ToString("dd-MM-yyyy");
                var amountStr = entry.AmountMl.HasValue ? $"{entry.AmountMl}ml" : "-";
                var percStr = entry.Percentage.HasValue ? $"{entry.Percentage}%" : "-";

                sb.AppendLine($"{dateStr,-12} | {amountStr,-8} | {percStr,-4} | {entry.Id}");
            }
            sb.AppendLine("```");
            sb.AppendLine("Use `/alko-entry-remove [id]` to delete an entry.");

            return sb.ToString();
        }
    }
}
