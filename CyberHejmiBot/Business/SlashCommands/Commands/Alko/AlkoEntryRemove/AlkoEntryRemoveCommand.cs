using CyberHejmiBot.Entities;
using AlkoStatEntity = CyberHejmiBot.Data.Entities.Alcohol.AlkoStat;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoEntryRemove
{
    public class AlkoEntryRemoveCommand : BaseSlashCommandHandler<ISlashCommand>
    {
        private readonly LocalDbContext _dbContext;
        private readonly ILogger<AlkoEntryRemoveCommand> _logger;
        private readonly AlkoEntryRemoveValidator _validator;

        public override string CommandName => "alko-entry-remove";
        public override string Description => "Removes an alcohol log entry by ID.";

        public AlkoEntryRemoveCommand(
            DiscordSocketClient client,
            LocalDbContext dbContext,
            ILogger<AlkoEntryRemoveCommand> logger,
            AlkoEntryRemoveValidator validator
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
                    "id",
                    "The ID of the entry to remove (from list)",
                    true,
                    ApplicationCommandOptionType.String
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

                if (!TryGetId(command, out var id))
                {
                    await command.FollowupAsync("❌ Invalid ID format. Please use the UUID from `/alko-entry-list`.", ephemeral: true);
                    return true;
                }

                var entry = await _dbContext.AlkoStats.FindAsync(id);

                if (!_validator.ValidateEntry(command, entry, out var errorResponse))
                {
                    await command.FollowupAsync(errorResponse, ephemeral: true);
                    return true;
                }

                await RemoveEntry(entry!);
                await command.FollowupAsync(BuildSuccessMessage(entry!), ephemeral: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {CommandName}");
                await command.FollowupAsync("An error occurred while removing the entry.", ephemeral: true);
            }

            return true;
        }

        private bool TryGetId(SocketSlashCommand command, out Guid id)
        {
            var idStr = command.Data.Options.FirstOrDefault(x => x.Name == "id")?.Value as string;
            return Guid.TryParse(idStr, out id);
        }

        private async Task RemoveEntry(AlkoStatEntity entry)
        {
            _dbContext.AlkoStats.Remove(entry);
            await _dbContext.SaveChangesAsync();
        }

        private string BuildSuccessMessage(AlkoStatEntity entry)
        {
            var deletedDetails = $"Date: {entry.Date:dd-MM-yyyy}";
            if (entry.AmountMl.HasValue) deletedDetails += $", {entry.AmountMl}ml";
            return $"✅ Deleted entry ({deletedDetails}).";
        }
    }
}
