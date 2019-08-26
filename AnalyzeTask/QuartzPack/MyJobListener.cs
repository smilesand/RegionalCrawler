using AnalyzeTask.SchedulingTasks;
using AnalyzeTask.Tool;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyzeTask.QuartzPack
{
    public class MyJobListener : JobListener
    {
        private ILogger log = Serilogs.logger;
        public override string Name => base.Name;
        /// <summary>
        /// 当任务执行前加载数据源
        /// </summary>
        /// <param name="context"></param>
        private static int JobIndex = 0;
        private static int WasIndex = 0;
        public int JobCount = BaseJob.jobDetails.Count;
        public override void JobToBeExecuted(IJobExecutionContext context)
        {
            lock (this)
            {
                try
                {
                    
                }
                catch (Exception ex)
                {
                    log.Error(ex, ex.Message);
                }

            }
        }

        public override void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            WasIndex++;
            if (WasIndex == JobCount)
            {
                WasIndex = 0;
                log.Information("--------所有任务执行完成--------");
            }
        }
    }
}
