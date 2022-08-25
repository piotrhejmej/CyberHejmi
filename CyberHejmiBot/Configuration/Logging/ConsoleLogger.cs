using Discord;

namespace CyberHejmiBot.Configuration.Loging
{
    internal class ConsoleLogger: ILogger
    {
        public Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
