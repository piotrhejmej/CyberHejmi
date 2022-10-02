using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Jobs.Recurring
{
    public interface IReccurringJob
    {
        void AddOrUpdate();
    }
}
