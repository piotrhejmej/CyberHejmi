using CyberHejmiBot.Business.Common;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (isMrStreamerOnline)
                await ReplyAsync("MrStreamer jest online!");
            else
                await ReplyAsync("MrStreamer nie jest online :(");
        }
    }
}
