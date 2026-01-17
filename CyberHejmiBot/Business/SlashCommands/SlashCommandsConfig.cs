namespace CyberHejmiBot.Business.SlashCommands
{
    internal interface ISlashCommandsConfig
    {
        Task RegisterSlashCommands();
    }

    internal class SlashCommandsConfig : ISlashCommandsConfig
    {
        private readonly IEnumerable<BaseSlashCommandHandler<ISlashCommand>> Things;

        public SlashCommandsConfig(IEnumerable<BaseSlashCommandHandler<ISlashCommand>> things)
        {
            Things = things;
        }

        public async Task RegisterSlashCommands()
        {
            foreach (var thing in Things)
                await thing.Register();
        }
    }
}
