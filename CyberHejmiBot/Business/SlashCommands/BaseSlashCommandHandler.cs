using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace CyberHejmiBot.Business.SlashCommands
{
    public record AdditionalOption(string OptionName, string Description, bool IsRequired, ApplicationCommandOptionType Type);

    public abstract class BaseSlashCommandHandler<T> where T : ISlashCommand
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

        public virtual async Task Register()
        {
            await Register(null);
        }

        public virtual async Task Register(ICollection<AdditionalOption>? AdditionalOptions)
        {
            var commandBuilder = new SlashCommandBuilder()
               .WithName(CommandName)
               .WithDescription(Description);

            if (AdditionalOptions?.Any() == true)
            {
                AdditionalOptions
                    .ToList()
                    .ForEach(option =>
                    {
                        commandBuilder.AddOption(option.OptionName,
                            option.Type,
                            option.Description,
                            option.IsRequired);
                    });
            }

            try
            {
                await Client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
                Client.SlashCommandExecuted += DoWork;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error registering command");
            }
        }

        public virtual Task<bool> DoWork(SocketSlashCommand command) => Task.FromResult(command?.Data.Name != CommandName);
    }
}
