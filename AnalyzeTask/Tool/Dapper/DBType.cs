using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool.Dapper
{
    /// <summary>
    /// 数据库类型（为支持数据分离模式，对应配置文件中数据连接位）
    /// </summary>
    public class DBType
    {
        /// <summary>
        /// SQLSERVER
        /// </summary>
        public const string SQLSERVER = "SQLSERVER";
        /// <summary>
        /// Oracle
        /// </summary>
        public const string Oracle = "Oracle";
        /// <summary>
        /// OracleResult
        /// </summary>
        public const string OracleResult = "OracleResult";
    }
}
