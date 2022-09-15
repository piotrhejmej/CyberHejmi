using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Hangfire
{
    public class HangfireJobActivator: JobActivator
    {
        private IServiceProvider ServiceProvider;

        public HangfireJobActivator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            var service = ServiceProvider.GetService(type);
            return ServiceProvider.GetService(type);
        }
    }
}
