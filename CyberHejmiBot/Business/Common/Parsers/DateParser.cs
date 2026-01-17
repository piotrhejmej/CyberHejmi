using System.Globalization;

namespace CyberHejmiBot.Business.Common.Parsers
{
    public interface IDateParser
    {
        bool TryParse(string input, out DateTime date);
    }

    public class DateParser : IDateParser
    {
        public bool TryParse(string input, out DateTime date)
        {
            return DateTime.TryParseExact(
                input,
                "dd-MM-yyyy",
                null,
                DateTimeStyles.None,
                out date
            );
        }
    }
}
