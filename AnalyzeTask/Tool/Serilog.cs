using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool
{
    /// <summary>
    /// 日志方法扩展
    /// </summary>
    public static class Serilogs
    {
        public static ILogger logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().WriteTo.File("logs\\Log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
    }
}
