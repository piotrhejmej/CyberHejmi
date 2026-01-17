using CyberHejmiBot.Business.SlashCommands;

namespace CyberHejmiBot.Business.SlashCommands.Commands.Alko
{
    public interface IAlkoCommand
    {
        string CommandName { get; }
        string Description { get; }
        IReadOnlyList<AdditionalOption> Options { get; }
    }
}
