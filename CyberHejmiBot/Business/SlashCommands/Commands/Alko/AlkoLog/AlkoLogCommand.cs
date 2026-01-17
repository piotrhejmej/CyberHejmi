using CyberHejmiBot.Business.Common.Parsers;
using CyberHejmiBot.Data.Entities.Alcohol;
using AlkoStatEntity = CyberHejmiBot.Data.Entities.Alcohol.AlkoStat;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoLog
{
    public class AlkoLogCommand : BaseSlashCommandHandler<ISlashCommand>, IAlkoCommand
    {
        private readonly LocalDbContext _dbContext;
        private readonly ILogger<AlkoLogCommand> _logger;
        private readonly AlkoLogValidator _validator;

        public override string CommandName => "alko-log";
        public override string Description =>
            "Log alcohol. Provide amount & percentage for details, or leave empty for generic.";

        private static readonly AdditionalOption[] _options = new[]
        {
            new AdditionalOption(
                "amount",
                "Amount in ml (Required if percentage is set)",
                false,
                ApplicationCommandOptionType.Integer
            ),
            new AdditionalOption(
                "percentage",
                "Alcohol percentage (e.g. 5, 40) (Required if amount is set)",
                false,
                ApplicationCommandOptionType.Number
            ),
            new AdditionalOption(
                "date",
                "Date of consumption (DD-MM-YYYY) - Optional, defaults to today",
                false,
                ApplicationCommandOptionType.String
            ),
        };

        public IReadOnlyList<AdditionalOption> Options => _options;

        public AlkoLogCommand(
            DiscordSocketClient client,
            LocalDbContext dbContext,
            ILogger<AlkoLogCommand> logger,
            AlkoLogValidator validator
        )
            : base(client, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _validator = validator;
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
                var (amount, percentage, dateOption) = GetOptions(command);

                var validationError = _validator.ValidateInterdependencies(amount, percentage);
                if (!string.IsNullOrEmpty(validationError))
                {
                    await command.FollowupAsync(validationError, ephemeral: true);
                    return true;
                }

                var (date, dateError) = _validator.ValidateAndParseDate(dateOption);
                if (dateError != null)
                {
                    await command.FollowupAsync(dateError, ephemeral: true);
                    return true;
                }

                await SaveStats(command, date!.Value, amount, percentage);

                await SendResponse(command, amount, percentage, date.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {CommandName}");
                await command.FollowupAsync(
                    "An unexpected error occurred.",
                    ephemeral: true
                );
            }

            return true;
        }

        private (int? amount, float? percentage, string? dateOption) GetOptions(
            SocketSlashCommand command
        )
        {
            var amountOption = command.Data.Options.FirstOrDefault(x => x.Name == "amount")?.Value;
            var percentageOption = command
                .Data.Options.FirstOrDefault(x => x.Name == "percentage")
                ?.Value;
            var dateOption =
                command.Data.Options.FirstOrDefault(x => x.Name == "date")?.Value as string;

            int? amount = amountOption != null ? Convert.ToInt32(amountOption) : null;
            float? percentage =
                percentageOption != null ? Convert.ToSingle(percentageOption) : null;

            return (amount, percentage, dateOption);
        }

        private async Task SaveStats(
            SocketSlashCommand command,
            DateTime date,
            int? amount,
            float? percentage
        )
        {
            var entry = new AlkoStatEntity
            {
                UserId = command.User.Id,
                Date = date,
                AmountMl = amount,
                Percentage = percentage,
                CreatedAt = DateTime.UtcNow,
            };

            _dbContext.AlkoStats.Add(entry);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SendResponse(
            SocketSlashCommand command,
            int? amount,
            float? percentage,
            DateTime date
        )
        {
            try
            {
                var msg = amount.HasValue
                    ? $"Logged {amount}ml of {percentage}% alcohol for {date:dd-MM-yyyy}."
                    : $"Logged alcohol consumption for {date:dd-MM-yyyy}.";

                if (percentage.HasValue && percentage == 100)
                {
                    msg += " serio, 100% alko? :O";
                }

                await command.User.SendMessageAsync(msg);
                await command.FollowupAsync("Done! Check your DMs.", ephemeral: true);
            }
            catch (Discord.Net.HttpException)
            {
                await command.FollowupAsync(
                    "I couldn't send you a DM. Please check your privacy settings.",
                    ephemeral: true
                );

                throw;
            }
        }
    }
}
