using CyberHejmiBot.Business.Common;
using CyberHejmiBot.Configuration.Loging;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands.Commands.RandomFacts
{
    public class RandomEventFactHandler : BaseSlashCommandHandler<ISlashCommand>
    {
        override public string CommandName => "fact-of-a-day";
        override public string Description => "Returns a random history fact that occured on the same day as today (or the date you've chosen) :)";

        private readonly ICollection<AdditionalOption> AdditionalOptions = new List<AdditionalOption>()
        {
            new AdditionalOption("date", "date of the fact you're seeking (Format: MM-dd)", false, ApplicationCommandOptionType.String)
        };
        private readonly IRandomFactFetcher RandomFactFetcher;

        public RandomEventFactHandler(DiscordSocketClient client, ILogger logger, IRandomFactFetcher randomFactFetcher) : base(client, logger)
        {
            Client = client;
            Logger = logger;
            RandomFactFetcher = randomFactFetcher;
        }

        public override Task Register()
        {
            return base.Register(this.AdditionalOptions);
        }

        public override async Task<bool> DoWork(SocketSlashCommand command)
        {
            if ((await base.DoWork(command)))
                return false;

            RandomFactResult result;
            var isToday = true;
            var usedDate = DateTime.Now;
            try
            {
                var dateOption = command.Data.Options.FirstOrDefault(r => r.Name == "date");

                if (dateOption is not null)
                {
                    var dateSplit = dateOption.Value.ToString()?.Split('-').Select(r => int.Parse(r)).ToArray();

                    if (dateSplit is not null && dateSplit.Length > 0)
                    {
                        usedDate = new DateTime(DateTime.Now.Year, dateSplit[0], dateSplit[1]);
                        result = await RandomFactFetcher.GetRandomFactOfDate(usedDate, FactType.Event);
                        isToday = false;
                    }
                    else
                        result = await RandomFactFetcher.GetRandomFactOfToday(FactType.Event);
                }
                else
                {
                    result = await RandomFactFetcher.GetRandomFactOfToday(FactType.Event);
                }
            }
            catch
            {
                result = await RandomFactFetcher.GetRandomFactOfToday(FactType.Event);
            }

            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle(isToday ? $"On this day in {result.Year}:" : $"On {usedDate.ToString("MMMM dd")} {result.Year}:")
                .WithDescription(result.Description);

            var restChannel = (await Client
                .Rest
                .GetChannelAsync(command.ChannelId ?? 0)) as RestTextChannel;

            if (restChannel == null)
                return false;

            await command.RespondAsync(embed: embedBuilder.Build());

            return true;
        }
    }
}
