namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoStat
{
    public class AlkoStatValidator
    {
        public bool ValidateYear(long? yearOption, out int? year, out string error)
        {
            year = null;
            error = string.Empty;

            if (yearOption == null)
            {
                return true;
            }

            int yearVal;
            try
            {
                yearVal = checked((int)yearOption.Value);
            }
            catch (OverflowException)
            {
                error = "❌ Validation Error: Year is invalid (number too large/small).";
                return false;
            }

            // Reasonable bounds check (e.g. 1900 - 2100)
            if (yearVal < 1900 || yearVal > 2100)
            {
                error = "❌ Validation Error: Year must be between 1900 and 2100.";
                return false;
            }

            year = yearVal;
            return true;
        }
    }
}
