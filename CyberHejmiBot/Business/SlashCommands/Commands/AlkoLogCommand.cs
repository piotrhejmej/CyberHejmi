using CyberHejmiBot.Data.Entities.Alcohol;
using CyberHejmiBot.Entities;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands.Commands
{
    public class AlkoLogCommand : BaseSlashCommandHandler<ISlashCommand>
    {
        private readonly LocalDbContext _dbContext;
        private readonly ILogger<AlkoLogCommand> _logger;

        public override string CommandName => "alko-log";
        public override string Description =>
            "Log alcohol. Provide amount & percentage for details, or leave empty for generic.";

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

            await base.Register(options);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if (await base.DoWork(command))
                return false;

            try
            {
                var (amount, percentage, dateOption) = GetOptions(command);

                var validationRes = await ValidateInterdependencies(command, amount, percentage);

                if (!validationRes)
                    return true;

                var dateResult = await ValidateAndParseDate(command, dateOption);

                if (!dateResult.IsValid)
                    return true;

                var date = dateResult.Date;

                await SaveStats(command, date, amount, percentage);

                await SendResponse(command, amount, percentage, date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {CommandName}");
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

        private async Task<bool> ValidateInterdependencies(
            SocketSlashCommand command,
            int? amount,
            float? percentage
        )
        {
            if (
                (amount.HasValue && !percentage.HasValue)
                || (!amount.HasValue && percentage.HasValue)
            )
            {
                await command.RespondAsync(
                    "❌ Validation Error: You must provide **both** 'amount' and 'percentage' if you specify one of them.",
                    ephemeral: true
                );
                return false;
            }
            return true;
        }

        private async Task<(bool IsValid, DateTime Date)> ValidateAndParseDate(
            SocketSlashCommand command,
            string? dateOption
        )
        {
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
                        $"❌ Validation Error: Invalid date format '{dateOption}'. Please use DD-MM-YYYY.",
                        ephemeral: true
                    );
                    return (false, DateTime.MinValue);
                }
            }

            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return (true, date);
        }

        private async Task SaveStats(
            SocketSlashCommand command,
            DateTime date,
            int? amount,
            float? percentage
        )
        {
            var entry = new AlkoStat
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

                await command.User.SendMessageAsync(msg);
                await command.RespondAsync("Done! Check your DMs.", ephemeral: true);
            }
            catch (Discord.Net.HttpException)
            {
                await command.RespondAsync(
                    "I couldn't send you a DM. Please check your privacy settings.",
                    ephemeral: true
                );

                throw;
            }
        }
    }
}
