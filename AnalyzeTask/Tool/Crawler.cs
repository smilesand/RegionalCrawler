using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AnalyzeTask.Tool
{
    public class Crawler
    {
        private static Object obj = new object();
        private ILogger log = Serilogs.logger;
        public int ThreadNum = 10;
        public int wait = 0;
        public int Timeout = 30000;
        public string encod = "GB2312";
        public int Retry = 3;
        private Queue<QueueModel> queue = new Queue<QueueModel>();
        private List<Task> tasks = new List<Task>();
        private bool TreadBreak = false;
        public Crawler()
        {

        }

        public Crawler(int ThreadNum = 10, int wait = 0, int Timeout = 30000, string encod = "UTF8")
        {
            this.ThreadNum = ThreadNum;
            this.wait = wait;
            this.Timeout = Timeout;
            this.encod = encod;
        }
        /// <summary>
        /// 获取当前任务队列中还有多少任务
        /// </summary>
        /// <returns></returns>
        public int GetQueueCount()
        {
            return this.queue.Count;
        }
        /// <summary>
        /// 任务工厂
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private List<Task> TasksFac(Action action)
        {
            for (int i = 0; i < ThreadNum; i++)
            {
                Task task = new Task(action);
                tasks.Add(task);
            }
            return tasks;
        }
        /// <summary>
        /// 开始所有任务
        /// </summary>
        public void RunCrawler()
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Start();
            }
        }
        /// <summary>
        /// 给任务下达具体命令
        /// </summary>
        /// <param name="action"></param>
        public void DoSomeThing(Action<string, QueueModel> action)
        {
            TasksFac(() =>
            {
                while (true)
                {
                    lock (obj)
                    {
                        if (queue.Count > 0)
                        {
                            if (TreadBreak)
                            {
                                break;
                            }
                            QueueModel model = queue.Dequeue();
                            string Html = GetWebContent(model.Url);
                            action(Html, model);
                        }
                        else
                        {
                            TreadBreak = true;
                        }
                        log.Error(this.GetQueueCount().ToString());
                    }
                }
            });
        }

        /// <summary>
        /// 完结所有任务
        /// </summary>
        public void Done()
        {
            TreadBreak = true;
        }

        //根据Url地址得到网页的html源码 
        public string GetWebContent(string Url)
        {
            if (wait > 0)
            {
                lock (obj)
                {
                    Thread.Sleep(wait);
                    return GetHmltContent(Url);
                }
            }
            else
            {
                return GetHmltContent(Url);
            }
        }

        private string GetHmltContent(string Url)
        {
            string strResult = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Timeout = 30000;
                request.Headers.Set("Pragma", "no-cache");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding(encod);
                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                strResult = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                if (Retry > 0)
                {
                    Retry--;
                    return GetHmltContent(Url);
                }
                log.Error($"错误URL：{Url}", e);
            }
            return strResult;
        }

        /// <summary>
        /// 将任务添加至队列
        /// </summary>
        /// <param name="Url"></param>
        public void EnQueue(QueueModel Que)
        {
            this.queue.Enqueue(Que);
        }
    }

    public class QueueModel
    {
        public string Url { get; set; }

        public Dictionary<string, string> pairs = new Dictionary<string, string>();
    }
}
