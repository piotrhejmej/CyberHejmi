using CyberHejmiBot.Entities;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.TextCommands.Modules
{
    public class DebugBeerStatsModule : ModuleBase<SocketCommandContext>
    {
        private readonly LocalDbContext _dbContext;

        public DebugBeerStatsModule(LocalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Command("debug_beer_stats")]
        public async Task DebugBeerStats()
        {
            if (Context.Guild == null)
            {
                await ReplyAsync("This command can only be used in a server.");
                return;
            }

            var guildId = Context.Guild.Id;
            var karmaStats = await _dbContext.UserKarma
                .Where(k => k.GuildId == guildId)
                .OrderByDescending(k => k.Points)
                .ToListAsync();

            if (!karmaStats.Any())
            {
                await ReplyAsync($"No karma stats found for this server {Context.Guild.Id}.");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine("| User ID            | Points |");
            sb.AppendLine("|--------------------|--------|");

            foreach (var stat in karmaStats)
            {
                sb.AppendLine($"| {stat.UserId,-18} | {stat.Points,-6} |");
            }

            sb.AppendLine("```");

            await ReplyAsync(sb.ToString());
        }
    }
}
