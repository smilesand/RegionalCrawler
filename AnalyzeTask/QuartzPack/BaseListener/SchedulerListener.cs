using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.QuartzPack
{
    public class SchedulerListener : ISchedulerListener
    {
        public virtual void JobScheduled(ITrigger trigger) {

        }
        public virtual void JobUnscheduled(TriggerKey triggerKey) {

        }

        public virtual void TriggerFinalized(ITrigger trigger) {

        }
        public virtual void TriggerPaused(TriggerKey triggerKey) {

        }

        public virtual void TriggersPaused(string triggerGroup) {

        }

        public virtual void TriggersResumed(string triggerGroup) {

        }

        public virtual void TriggerResumed(TriggerKey triggerKey)
        {

        }
        public virtual void JobAdded(IJobDetail jobDetail) {

        }

        public virtual void JobDeleted(JobKey jobKey) {

        }

        public virtual void JobPaused(JobKey jobKey) {

        }

        public virtual void JobsPaused(string jobGroup) {

        }

        public virtual void JobResumed(JobKey jobKey) {

        }

        public virtual void JobsResumed(string jobGroup) {

        }


        public virtual void SchedulerError(string msg, SchedulerException cause) {

        }


        public virtual void SchedulerInStandbyMode() {

        }


        public virtual void SchedulerStarted() {

        }

        public virtual void SchedulerStarting() {

        }

        public virtual void SchedulerShutdown() {

        }

        public virtual void SchedulerShuttingdown() {

        }

        public virtual void SchedulingDataCleared() {

        }
    }
}
