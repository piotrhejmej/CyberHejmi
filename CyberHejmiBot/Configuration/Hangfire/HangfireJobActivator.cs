using Hangfire;

namespace CyberHejmiBot.Configuration.Hangfire
{
    public class HangfireJobActivator : JobActivator
    {
        private readonly IServiceProvider ServiceProvider;

        public HangfireJobActivator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public override object? ActivateJob(Type type) => ServiceProvider.GetService(type);
    }
}
