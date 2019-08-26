using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.QuartzPack
{
    public class JobListener : IJobListener
    {
        public virtual string Name
        {
            get { return "JobListener"; }
        }

        public virtual void JobToBeExecuted(IJobExecutionContext context)
        {

        }

        public virtual void JobExecutionVetoed(IJobExecutionContext context)
        {

        }

        public virtual void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {

        }
    }
}
