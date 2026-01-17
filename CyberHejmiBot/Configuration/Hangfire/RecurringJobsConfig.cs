using CyberHejmiBot.Business.Jobs.Recurring;

namespace CyberHejmiBot.Configuration.Hangfire
{
    public interface IRecurringJobsConfig
    {
        void RegisterJobs();
    }

    public class RecurringJobsConfig : IRecurringJobsConfig
    {
        private readonly IEnumerable<IReccurringJob> RecurringJobs;

        public RecurringJobsConfig(IEnumerable<IReccurringJob> recurringJobs)
        {
            RecurringJobs = recurringJobs;
        }

        public void RegisterJobs()
        {
            foreach (var job in RecurringJobs)
                job.AddOrUpdate();
        }
    }
}
