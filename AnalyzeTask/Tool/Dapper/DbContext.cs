using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool.Dapper
{
    public class DbContext : IDbContext
    {
        private static ILogger log = Serilogs.logger;
        public static NameValueCollection _dbLinkPool = _dbLinkPool = new NameValueCollection();//连接位集合
        private string _currentContextName = null;//当前连接位
        /// <summary>
        /// 加载所有连接位
        /// </summary>
        static DbContext()
        {
            foreach (ConnectionStringSettings connStrSetting in ConfigurationManager.ConnectionStrings)
            {
                if (connStrSetting.Name.ToUpper() == "LocalSqlServer".ToUpper())
                    continue;
                _dbLinkPool.Add(connStrSetting.Name, connStrSetting.ConnectionString);
            }
        }
        /// <summary>
        /// 默认使用第一个连接位(如果不指定连接位)
        /// </summary>
        public DbContext()
        {
            if (_dbLinkPool.Count > 0)
            {
                _currentContextName = _dbLinkPool.Keys[0];
            }
        }
        /// <summary>
        /// 切换上下文
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <param name="useOnlyCurrent"></param>
        /// <returns></returns>
        public DbContext ChangeContext(string connectionStringName, bool useOnlyCurrent = false)
        {
            if (string.IsNullOrWhiteSpace(_dbLinkPool[connectionStringName]))
                throw new Exception("连接不存在");
            if (useOnlyCurrent)
            {
                new DbContext() { _currentContextName = connectionStringName };
                return this;
            }
            else
            {
                _currentContextName = connectionStringName;
            }
            return this;
        }
        /// <summary>
        /// 获取数据连接字符串
        /// </summary>
        public string CurrentContext
        {
            get { return _dbLinkPool[_currentContextName]; }
        }
        /// <summary>
        /// 获取数据连接字符串集合
        /// </summary>
        public NameValueCollection LinkPool
        {
            get { return _dbLinkPool; }
        }
    }
}
