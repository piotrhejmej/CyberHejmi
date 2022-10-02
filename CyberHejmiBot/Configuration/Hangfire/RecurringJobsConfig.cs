using CyberHejmiBot.Business.Jobs.Recurring;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Hangfire
{
    public interface IRecurringJobsConfig
    {
        void RegisterJobs();
    }

    public class RecurringJobsConfig: IRecurringJobsConfig
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
