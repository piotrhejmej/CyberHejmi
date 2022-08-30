using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Modules
{
	[Group("test")]
	public class TestModule : ModuleBase<SocketCommandContext>
	{
		[Command("say")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder][Summary("The text to echos")] string echo)
			=> ReplyAsync(echo);

	}
}
