using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.QuartzPack
{
    class TriggerListener : ITriggerListener
    {
        public string Name
        {
            get { return "TriggerListener"; }
        }
        public void TriggerFired(ITrigger trigger, IJobExecutionContext context)
        {

        }

        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context)
        {
            return true;
        }

        public void TriggerMisfired(ITrigger trigger)
        {

        }

        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode)
        {

        }
    }
}
