using CyberHejmiBot.Configuration.Loging;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.SlashCommands
{
    public abstract class BaseSlashCommandHandler<T> where T: ISlashCommand
    {
        public abstract string CommandName { get; }
        public abstract string Description { get; }
        public DiscordSocketClient Client;
        public ILogger Logger;

        protected BaseSlashCommandHandler(DiscordSocketClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;
        }

        public async Task Register()
        {
            var commandBuilder = new SlashCommandBuilder()
               .WithName(CommandName)
               .WithDescription(Description);

            try
            {
                await Client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
                Client.SlashCommandExecuted += DoWork;
            }
            catch (Exception ex)
            {
                Logger.Log(new LogMessage(LogSeverity.Error, ex.Source, ex.Message, ex));
            }
        }

        public virtual async Task DoWork(SocketSlashCommand command)
        {
            if (command?.Data.Name != CommandName)
                return;
        }
    }
}
