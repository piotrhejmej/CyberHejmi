namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoEntryList
{
    public class AlkoEntryListValidator
    {
        public bool ValidatePage(long? pageOption, out int page, out string error)
        {
            page = 0;
            error = string.Empty;

            if (pageOption == null)
            {
                return true;
            }

            if (pageOption < 0)
            {
                error = "❌ Page number cannot be negative.";
                return false;
            }

            try
            {
                page = checked((int)pageOption.Value);
            }
            catch (OverflowException)
            {
                error = "❌ Page number is too large.";
                return false;
            }
            return true;
        }
    }
}
