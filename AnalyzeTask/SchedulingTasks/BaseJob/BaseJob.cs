using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyzeTask.Tool;
using Quartz.Impl;
using System.Configuration;
using AnalyzeTask.QuartzPack;
using Quartz.Impl.Matchers;
using AnalyzeTask.Tool.Dapper;
using System.Data;

namespace AnalyzeTask.SchedulingTasks
{
    /// <summary>
    /// 插件化处理
    /// </summary>
    public abstract class BaseJob : IJob
    {
        private static IScheduler CurrentSched { get; set; }
        protected static ILogger log = Serilogs.logger;
        private static string Cron = ConfigurationManager.AppSettings["Cron"];
        protected ITrigger cronTrigger = null;
        public static List<IJobDetail> jobDetails = new List<IJobDetail>();
        public static List<ITrigger> cronTriggers = new List<ITrigger>();
        protected DbContext dbContext = new DbContext();
        protected string TaskName { get; set; }

        public BaseJob()
        {
            int DeBug = 0;
            int.TryParse(ConfigurationManager.AppSettings["DeBug"], out DeBug);
            if (DeBug > 0)
            {
                cronTrigger = TriggerBuilder.Create().WithIdentity(Guid.NewGuid().ToString(), JobKey.DefaultGroup.ToLower()).StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(DeBug).RepeatForever()).Build();
            }
            else
            {
                if (cronTrigger == null)
                {
                    cronTrigger = TriggerBuilder.Create().WithIdentity(Guid.NewGuid().ToString(), JobKey.DefaultGroup.ToLower()).StartNow().WithCronSchedule(Cron).Build();
                }
            }
        }
        public static void Initialize(List<BaseJob> baseJob)
        {
            log.Information("日志控件加载");
            CurrentSched = CurrentSched = StdSchedulerFactory.GetDefaultScheduler();
            for (int i = 0; i < baseJob.Count; i++)
            {
                log.Debug(baseJob[i].TaskName + "正在初始化");
                jobDetails.Add(baseJob[i].CreateJobDetail(JobKey.DefaultGroup.ToLower()));
                cronTriggers.Add(baseJob[i].CreateITrigger(Guid.NewGuid().ToString(), JobKey.DefaultGroup.ToLower()));
            }
            log.Information("初始化完成");
        }
        public virtual void Execute(IJobExecutionContext context)
        {
            log.Information($"Hello");
        }

        protected void NextTime(IJobExecutionContext context)
        {
            log.Information($"{TaskName}=>下次执行时间为：{context.NextFireTimeUtc}");
        }

        public abstract IJobDetail CreateJobDetail(string TaskGroup);

        public abstract ITrigger CreateITrigger(string TriggerName, string TriggerGroup);

        public static void Run()
        {
            for (int i = 0; i < jobDetails.Count; i++)
            {
                CurrentSched.ScheduleJob(jobDetails[i], cronTriggers[i]);
            }
            GroupMatcher<JobKey> matcher = GroupMatcher<JobKey>.GroupEquals(JobKey.DefaultGroup.ToLower());
            JobListener jobListener = new MyJobListener();
            CurrentSched.ListenerManager.AddJobListener(jobListener, matcher);
            CurrentSched.Start();
        }
    }
}
