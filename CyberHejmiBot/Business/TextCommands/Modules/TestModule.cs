using CyberHejmiBot.Configuration.Hangfire;
using CyberHejmiBot.Configuration.Loging;
using CyberHejmiBot.Entities;
using Discord;
using Discord.Commands;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Modules
{
	[Group("test")]
	public class TestModule : ModuleBase<SocketCommandContext>
	{
		private readonly LocalDbContext Context;

		public TestModule(LocalDbContext context) : base()
        {
            Context = context;
        }

        [Command("say")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder][Summary("The text to echos")] string echo)
			=> ReplyAsync(echo);

		[Command("get")]
		[Summary("Gets from Db")]
		public async Task GetAsync()
        {
			var test = Context.TestEntities.FirstOrDefault();

			if (test is null)
				return;

			await ReplyAsync($"from db: {test.Id} {test.Name} {test.Description}");
		}
	}
}
