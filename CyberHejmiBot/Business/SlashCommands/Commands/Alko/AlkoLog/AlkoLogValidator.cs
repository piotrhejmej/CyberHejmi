using CyberHejmiBot.Business.Common.Parsers;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoLog
{
    public class AlkoLogValidator
    {
        private readonly IDateParser _dateParser;

        public AlkoLogValidator(IDateParser dateParser)
        {
            _dateParser = dateParser;
        }

        public string? ValidateInterdependencies(
            long? amountOption,
            float? percentage
        )
        {
            if (
                (amountOption.HasValue && !percentage.HasValue)
                || (!amountOption.HasValue && percentage.HasValue)
            )
            {
                return "❌ Validation Error: You must provide **both** 'amount' and 'percentage' if you specify one of them.";
            }

            if (amountOption.HasValue)
            {
                int amount;
                try
                {
                    amount = checked((int)amountOption.Value);
                }
                catch (OverflowException)
                {
                    return "❌ Validation Error: Amount is too large (exceeds integer limit).";
                }

                if (amount <= 0)
                {
                    return "❌ Validation Error: Amount must be greater than 0!";
                }

                if (amount >= 10000)
                {
                    return "Co ty typie, wypiłeś ponad 10litrów alko na raz? weź to podziel na kilka wpisów alkusie Ty xD";
                }
            }

            if (percentage.HasValue)
            {
                if (percentage < 0 || percentage > 100)
                {
                    return "❌ Validation Error: Percentage must be between 0 and 100!";
                }
            }

            return null;
        }

        public (DateTime? Date, string? Error) ValidateAndParseDate(
            string? dateOption
        )
        {
            var date = DateTime.UtcNow.Date;

            if (!string.IsNullOrEmpty(dateOption))
            {
                if (!_dateParser.TryParse(dateOption, out date))
                {
                    return (null, $"❌ Validation Error: Invalid date format '{dateOption}'. Please use DD-MM-YYYY.");
                }
            }

            if (date.Date > DateTime.UtcNow.Date)
            {
                return (null, "Data nie moze być późniejsza od dzisiejszej. Co ty mordo, planujesz spożywanie trucizny?");
            }

            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return (date, null);
        }
    }
}
