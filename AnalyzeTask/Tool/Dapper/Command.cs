using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace AnalyzeTask.Tool.Dapper
{
    /// <summary>
    /// 命令对象(命令：增删改)
    /// </summary>
    public class Command
    {
        /// <summary>
        /// 是否允许空结果
        /// </summary>
        private bool _isAllowZeroResult = false;
        private string _sqlText = null;
        private object _sqlParameter = null;
        private CommandType _commandType = CommandType.Text;
        private string _dbType = null;

        /// <summary>
        /// 待执行的SQL语句
        /// </summary>
        public string SqlText
        {
            get { return _sqlText; }
        }
        /// <summary>
        /// 参数数组
        /// </summary>
        public object SqlParameter
        {
            get
            {
                return _sqlParameter;
            }
        }
        /// <summary>
        /// 是否运行结果为：0（默认不允许）
        /// </summary>
        public bool IsAllowZeroResult
        {
            get
            {
                return _isAllowZeroResult;
            }
        }
        /// <summary>
        /// 命令类型(默认:Text)
        /// </summary>
        public CommandType CommandType
        {
            get { return _commandType; }
        }
        /// <summary>
        /// 当前数据库的类型 
        /// </summary>
        public string DbType
        {
            get { return _dbType; }
        }
        /// <summary>
        /// 数据库操作命令
        /// </summary>
        /// <param name="sqlText">数据库操作语句</param>
        /// <param name="parameter">参数</param>
        /// <param name="isAllowZeroResult"> 是否运行结果为：0</param>
        public Command(string sqlText, object parameter=null, bool isAllowZeroResult = false, string dbType = null) : this(sqlText, parameter, CommandType.Text, isAllowZeroResult, dbType)
        {
        }
        /// <summary>
        /// 数据库操作命令
        /// </summary>
        /// <param name="sqlText">数据库操作语句</param>
        /// <param name="parameter">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="isAllowZeroResult">是否运行结果为：0</param>
        public Command(string sqlText, object parameter, CommandType commandType, bool isAllowZeroResult = false, string dbType = null)
        {
            _sqlText = sqlText;
            _sqlParameter = parameter;
            _commandType = commandType;
            _isAllowZeroResult = isAllowZeroResult;
            _dbType = dbType;
        }
    }
}
