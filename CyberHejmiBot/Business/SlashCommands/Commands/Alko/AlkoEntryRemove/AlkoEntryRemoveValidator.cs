using Discord.WebSocket;
using AlkoStatEntity = CyberHejmiBot.Data.Entities.Alcohol.AlkoStat;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko.AlkoEntryRemove
{
    public class AlkoEntryRemoveValidator
    {
        public bool ValidateEntry(SocketSlashCommand command, AlkoStatEntity? entry, out string errorResponse)
        {
            if (entry == null)
            {
                errorResponse = "❌ Entry not found.";
                return false;
            }

            if (entry.UserId != command.User.Id)
            {
                errorResponse = "❌ You can only delete your own entries!";
                return false;
            }

            errorResponse = string.Empty;
            return true;
        }
    }
}
