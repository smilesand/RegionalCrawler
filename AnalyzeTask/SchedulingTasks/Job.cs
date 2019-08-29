using AnalyzeTask.Tool;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Less.Html;
using Less.Windows;
using AnalyzeTask.Model;
using AnalyzeTask.Tool.Dapper;

namespace AnalyzeTask.SchedulingTasks
{
    public class Job : BaseJob, IJob
    {
        public Job()
        {
            TaskName = "省市区爬虫";
        }
        public override void Execute(IJobExecutionContext context)
        {
            try
            {
                //需要寻找的class
                string[] NeedClass = new string[] { ".provincetr", ".citytr", ".countytr", ".towntr", ".villagetr" };
                Crawler crawler = new Crawler();
                crawler.wait = 2000;
                crawler.Timeout = 10000;
                //crawler.Cookie = "__utmz=207252561.1566065018.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); AD_RS_COOKIE=20082856; __utma=207252561.1090452968.1566065018.1566892516.1566911665.4; __utmc=207252561; __utmt=1; wzws_reurl=L3Rqc2ovdGpiei90anlxaGRtaGN4aGZkbS8yMDE4LzEzLmh0bWw=; __utmb=207252561.2.10.1566911665; wzws_cid=71a25ce145fef3ef24abcd820a9cb0a7362be1e734c83a66d1489e6dd05749fe7d46940c76eda51782a874be4358594674e0ffc98ea6cd85dbd9af045cc53d1d2f71135cff0df7fdf0e26a219f19ffc6";
                crawler.ThreadNum = 5;
                crawler.DoSomeThing((n, que) =>
                {
                    Qfun q = HtmlParser.Query(n);
                    if (q != null)
                    {
                        for (int i = 0; i < NeedClass.Length; i++)
                        {
                            var Class = q(NeedClass[i]);
                            foreach (var items in Class)
                            {
                                var td = q(items).find("td");
                                if (td.length == 2 || td.length == 1)
                                {
                                    var a = q(td).find("a");
                                    if (a.length > 0)
                                    {
                                        RegionalModel regional = new RegionalModel();
                                        regional.RegionalDataOID = Guid.NewGuid();
                                        regional.ID = a[0].textContent;
                                        regional.Name = a[1].textContent;
                                        regional.ParentOID = Guid.Parse(que.pairs.FirstOrDefault(f => f.Key == "ParentOID").Value);
                                        string Url = a[0].getAttribute("href");
                                        if (Url.IndexOf('/') > -1)
                                        {
                                            Url = Url.Substring(3, Url.Length - 5);
                                        }
                                        else
                                        {
                                            Url = Url.Substring(3, Url.Length - 8);
                                        }
                                        Url = UrlDecoding.Decoding(Url);
                                        crawler.EnQueue(CreateQueue(Url, regional.RegionalDataOID.ToString()));
                                        CreateSQLCommand(regional);
                                    }
                                    else
                                    {
                                        RegionalModel regional = new RegionalModel();
                                        regional.RegionalDataOID = Guid.NewGuid();
                                        regional.ID = td[0].textContent;
                                        regional.Name = td[1].textContent;
                                        regional.ParentOID = Guid.Parse(que.pairs.FirstOrDefault(f => f.Key == "ParentOID").Value);
                                        CreateSQLCommand(regional);
                                    }
                                }
                                else if (td.length == 3)
                                {
                                    RegionalModel regional = new RegionalModel();
                                    regional.RegionalDataOID = Guid.NewGuid();
                                    regional.ID = td[0].textContent;
                                    regional.Name = td[2].textContent;
                                    regional.ParentOID = Guid.Parse(que.pairs.FirstOrDefault(f => f.Key == "ParentOID").Value);
                                    CreateSQLCommand(regional);
                                }
                                else
                                {
                                    foreach (var item in td)
                                    {
                                        var a = q(item).find("a");
                                        foreach (var href in a)
                                        {
                                            RegionalModel regional = new RegionalModel();
                                            regional.RegionalDataOID = Guid.NewGuid();
                                            regional.Name = href.textContent;
                                            regional.ParentOID = Guid.Parse(que.pairs.FirstOrDefault(f => f.Key == "ParentOID").Value);
                                            string Url = href.getAttribute("href");
                                            regional.ID = Url.Substring(0, Url.Length - 5);
                                            crawler.EnQueue(CreateQueue(Url, regional.RegionalDataOID.ToString()));
                                            CreateSQLCommand(regional);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }, () => { InsertIntoMSDB(); crawler.Done(); });
                crawler.EnQueue(BeginTask());
                crawler.RunCrawler();
                base.NextTime(context);
            }
            catch (Exception ex)
            {
                log.Error(ex, $"{this.TaskName}同步出错");
            }
        }


        private string BaseUrl = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2018/";
        /// <summary>
        /// 将数据放入队列
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="ParentOID"></param>
        /// <param name="start"></param>
        public QueueModel CreateQueue(string Url, string ParentOID, bool start = false)
        {
            if (start)
            {
                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add("ParentOID", Guid.Empty.ToString());
                QueueModel queue = new QueueModel { Url = BaseUrl + Url, pairs = keys, Retry = 5 };
                return queue;
            }
            else
            {
                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add("ParentOID", ParentOID);
                QueueModel queue = new QueueModel { Url = BaseUrl + Url, pairs = keys, Retry = 5 };
                return queue;
            }
        }

        private CommandCollection collection = new CommandCollection();
        /// <summary>
        /// 创建SQL命令
        /// </summary>
        /// <param name="regional"></param>
        public void CreateSQLCommand(RegionalModel regional)
        {
            if (regional != null)
            {
                string sql = $"INSERT INTO RegionalData (RegionalDataOID,ID,Name,ParentOID) VALUES('{regional.RegionalDataOID}','{regional.ID}','{regional.Name}','{regional.ParentOID}')";
                log.Information(sql);
                collection.Add(new Command(sql, regional));
                dbContext.ExecuteSql(sql, regional);
            }
        }

        private QueueModel BeginTask()
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();
            keys.Add("ParentOID", Guid.Empty.ToString());
            return new QueueModel { Url = BaseUrl, pairs = keys };
        }
        /// <summary>
        /// 执行所有的命令
        /// </summary>
        private void InsertIntoMSDB()
        {
            DbContext dbContext = new DbContext();
            dbContext.ExecuteTransaction(collection);
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="TaskGroup"></param>
        /// <returns></returns>
        public override IJobDetail CreateJobDetail(string TaskGroup)
        {
            IJobDetail simpleJob = JobBuilder.Create<Job>().WithIdentity(TaskName, TaskGroup).Build();
            return simpleJob;
        }

        /// <summary>
        /// 每个插件在此自定义触发器
        /// </summary>
        /// <param name="TriggerName"></param>
        /// <param name="TriggerGroup"></param>
        /// <returns></returns>
        public override ITrigger CreateITrigger(string TriggerName, string TriggerGroup)
        {
            cronTrigger = base.cronTrigger;
            return cronTrigger;
        }
    }
}
