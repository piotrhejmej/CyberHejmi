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
		private readonly IBackgroundJobClient BackgroundJobClient;
		private readonly ILogger Logger;

		public TestModule(LocalDbContext context, ILogger logger) : base()
        {
            Context = context;
			Logger = logger;
			//BackgroundJobClient = backgroundJobClient;
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

		public async Task testc()
        {
			await ReplyAsync("after a while testc");
		}
		public void testd()
		{
			ReplyAsync("after a while testd").Wait();
		}
	}
}
