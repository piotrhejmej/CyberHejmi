﻿using CyberHejmiBot.Business.Common;
using Discord.Commands;

namespace CyberHejmiBot.Business.TextCommands.Modules
{
    [Group("czy")]
    public class MrStreamerStreamingModule: ModuleBase<SocketCommandContext>
    {
        private readonly ITwitchChecker _twitchChecker;

        public MrStreamerStreamingModule(ITwitchChecker twitchChecker) : base()
        {
            _twitchChecker = twitchChecker;
        }


        [Command("szymek streamuje?")]
        [Summary("Sprawdza czy Szymek streamuje")]
        public async Task IsStreaming() 
        { 
            var isMrStreamerOnline = await _twitchChecker.IsMrStreamerOnline();

            if (!isMrStreamerOnline.IsSuccesfull) {
                await ReplyAsync("błund");
                await ReplyAsync(isMrStreamerOnline.Error);
                return;
            }

            if (isMrStreamerOnline.Result == true)
            {

                var embedded = new Discord.EmbedBuilder()
                {
                    Url = "https://www.twitch.tv/StreamKoderka",
                }
                .WithColor(Discord.Color.Gold)
                .WithTimestamp(DateTimeOffset.Now)
                .WithTitle("ej bo Szymek streamuje")
                .WithDescription("wbijajcie");

                await ReplyAsync(embed: embedded.Build());  
            }
            else
            {

                var embedded = new Discord.EmbedBuilder()
                {
                    Url = "https://www.twitch.tv/StreamKoderka",
                }
                .WithColor(Discord.Color.Gold)
                .WithTimestamp(DateTimeOffset.Now)
                .WithTitle("ej bo Szymek streamuje")
                .WithDescription("wbijajcie");

                await ReplyAsync(embed: embedded.Build());
            }
        }
    }
}
