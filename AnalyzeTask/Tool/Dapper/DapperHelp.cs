using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Serilog;

namespace AnalyzeTask.Tool.Dapper
{
    public static class DapperHelp
    {
        #region 原生ADO
        //封装一个返回DataTable的方法
        public static DataTable ExecuteDataTable(this IDbContext context, string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, context.CurrentContext))
            {
                adapter.SelectCommand.CommandType = cmdType;
                if (pms != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                adapter.Fill(dt);
            }
            return dt;
        }
        #endregion
        private static ILogger log = Serilogs.logger;
        #region 通用查询系
        public static IEnumerable<T> Query<T>(this IDbContext context, string sql, object param = null, CommandType commandType = CommandType.Text, bool buffered = true)
        {
            IEnumerable<T> result;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                result = conn.Query<T>(sql, param, null, buffered, null, new CommandType?(commandType));
            }
            return result;
        }
        public static T QueryScalar<T>(this IDbContext context, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            T result;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                result = (T)conn.ExecuteScalar(sql, param, null, null, new CommandType?(commandType));
            }
            return result;
        }
        public static IEnumerable<TReturn> QueryData<TFirst, TSecond, TReturn>(this IDbContext context, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, string splitOn = "Id", CommandType? commandType = null) where TFirst : class where TSecond : class where TReturn : class
        {
            IEnumerable<TReturn> result;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                result = conn.Query(sql, map, param, null, true, splitOn, null, commandType);
            }
            return result;
        }
        public static int ExecuteSql(this IDbContext context, string sql, object param = null, bool isTransaction = false, CommandType commandType = CommandType.Text)
        {
            return ExecuteSql(context, new Command(sql, param, commandType), isTransaction);
        }
        public static int ExecuteSql(this IDbContext context, Command command, bool isTransaction = false)
        {
            int result;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                try
                {
                    if (isTransaction)
                    {
                        IDbTransaction dbTransaction = conn.BeginTransaction();
                        result = conn.Execute(command.SqlText, command.SqlParameter, dbTransaction, null, new CommandType?(command.CommandType));
                        if (result == 0)
                        {
                            dbTransaction.Rollback();
                        }
                        else
                        {
                            dbTransaction.Commit();
                        }
                    }
                    else
                    {
                        result = conn.Execute(command.SqlText, command.SqlParameter, null, null, new CommandType?(command.CommandType));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
        public static bool ExecuteTransaction(this IDbContext context, CommandCollection commands)
        {
            bool result;
            string sqltext = string.Empty;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                conn.Open();
                IDbTransaction dbTransaction = conn.BeginTransaction();
                try
                {
                    foreach (Command current in commands.GetCommands)
                    {
                        sqltext = current.SqlText;
                        int num = conn.Execute(current.SqlText, current.SqlParameter, dbTransaction, null, new CommandType?(current.CommandType));
                        if (num == 0 && !current.IsAllowZeroResult)
                        {
                            dbTransaction.Rollback();
                            result = false;
                            return result;
                        }
                    }
                    dbTransaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    log.Debug(sqltext);
                    throw ex;
                }
            }
            return result;
        }
        #endregion

        #region 查询系
        /// <summary>
        /// 获取Model-Key为int类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDbContext context, int id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                return await conn.GetAsync<T>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model-Key为long类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDbContext context, long id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                return await conn.GetAsync<T>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model-Key为Guid类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static T Get<T>(this IDbContext context, System.Guid id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                return conn.Get<T>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model-Key为string类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDbContext context, string id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                return await conn.GetAsync<T>(id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model集合（没有Where条件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> GetAllAsync<T>(this IDbContext context) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                return await conn.GetAllAsync<T>();
            }
        }
        #endregion

        #region 增删改
        /// <summary>
        /// 插入一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static long Insert<T>(this IDbContext context, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                return conn.Insert<T>(model, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 更新一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Update<T>(this IDbContext context, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                bool b = conn.Update<T>(model, transaction, commandTimeout);
                return b;
            }
        }

        /// <summary>
        /// 删除一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Delete<T>(this IDbContext context, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                bool b = conn.Delete<T>(model, transaction, commandTimeout);
                return b;
            }
        }
        #endregion

        #region 在同一命令中执行多个查询并映射结果
        public static IEnumerable<TReturn> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbContext context, string sqlText, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> func, object parameter = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    result = gridReader.Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(func, splitOn);
                }
            }
            return result;
        }
        public static IEnumerable<TReturn> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbContext context, string sqlText, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> func, object parameter = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    result = gridReader.Read<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(func, splitOn);
                }
            }
            return result;
        }
        public static IEnumerable<TReturn> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbContext context, string sqlText, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> func, object parameter = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    result = gridReader.Read<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(func, splitOn);
                }
            }
            return result;
        }
        public static IEnumerable<TReturn> QueryMultiple<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbContext context, string sqlText, Func<TFirst, TSecond, TThird, TFourth, TReturn> func, object parameter = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    result = gridReader.Read<TFirst, TSecond, TThird, TFourth, TReturn>(func, splitOn);
                }
            }
            return result;
        }
        public static IEnumerable<TReturn> QueryMultiple<TFirst, TSecond, TThird, TReturn>(this IDbContext context, string sqlText, Func<TFirst, TSecond, TThird, TReturn> func, object parameter = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    result = gridReader.Read<TFirst, TSecond, TThird, TReturn>(func, splitOn);
                }
            }
            return result;
        }
        public static IEnumerable<TReturn> QueryMultiple<TFirst, TSecond, TReturn>(this IDbContext context, string sqlText, Func<TFirst, TSecond, TReturn> func, object parameter = null, string splitOn = "Id")
        {
            IEnumerable<TReturn> result = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    result = gridReader.Read<TFirst, TSecond, TReturn>(func, splitOn);
                }
            }
            return result;
        }
        public static IEnumerable<TReturn> QueryMultiple<TReturn>(this IDbContext context, string sqlText, object parameter = null)
        {
            IEnumerable<TReturn> result = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    result = gridReader.Read<TReturn>();
                }
            }
            return result;
        }
        public static Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> QueryMultipleTable<TFirst, TSecond>(this IDbContext context, string sqlText, object parameter = null)
        {
            IEnumerable<TFirst> firstData = null;
            IEnumerable<TSecond> secondData = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    firstData = gridReader.Read<TFirst>();
                    secondData = gridReader.Read<TSecond>();
                }
            }
            return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>(firstData, secondData);
        }
        public static Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>> QueryMultipleTable<TFirst, TSecond, TThird>(this IDbContext context, string sqlText, object parameter = null)
        {
            IEnumerable<TFirst> firstData = null;
            IEnumerable<TSecond> secondData = null;
            IEnumerable<TThird> thirdData = null;
            using (var conn = new SqlConnection(context.CurrentContext))
            {
                using (var gridReader = conn.QueryMultiple(sqlText, parameter))
                {
                    firstData = gridReader.Read<TFirst>();
                    secondData = gridReader.Read<TSecond>();
                    thirdData = gridReader.Read<TThird>();
                }
            }
            return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>(firstData, secondData, thirdData);
        }

        #endregion

        #region Oracle数据库
        /// <summary>
        /// 执行sql执行增删改
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <returns></returns>
        public static int OracleExcuteSql(this IDbContext context, string cmdText, OracleParameter[] para = null)
        {
            using (OracleConnection conn = new OracleConnection(context.CurrentContext))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;
                if (para != null)
                {
                    cmd.Parameters.AddRange(para);
                }
                try
                {
                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw new Exception(ex.Message);
                    //return 0;
                }
            }
        }



        /// <summary>
        /// 执行sql获取数据集
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <returns></returns>
        public static DataTable OracleExcuteDatatable(this IDbContext context, string cmdText, OracleParameter[] para = null)
        {
            //log.Debug(cmdText);
            using (OracleConnection conn = new OracleConnection(context.CurrentContext))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;
                if (para != null)
                {
                    cmd.Parameters.AddRange(para);
                }
                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                try
                {
                    conn.Open();
                    DataSet ds = new DataSet();
                    oda.Fill(ds);
                    conn.Close();
                    return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw new Exception(ex.Message);
                }
                //return new DataTable();
            }
        }

        /// <summary>
        /// 执行sql获取数据集 前几条数据
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="top">前几条数据 默认为1</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <returns></returns>
        public static DataTable OracleExcuteDatatableByTop(this IDbContext context, string cmdText, int top = 1, OracleParameter[] para = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * from (  ");
            sb.Append(cmdText);
            sb.Append(" ) where rownum<=" + top.ToString() + " order by rownum asc ");
            return OracleExcuteDatatable(context, sb.ToString(), para);
        }
        #endregion

        #region 大数据拷贝
        /// <summary>  
        /// 注意：DataTable中的列需要与数据库表中的列完全一致。
        /// 已自测可用。
        /// </summary>  
        /// <param name="conStr">数据库连接串</param>
        /// <param name="strTableName">数据库中对应的表名</param>  
        /// <param name="dtData">数据集</param>  
        public static void SqlBulkCopyInsert(this IDbContext context, string strTableName, DataTable dtData)
        {
            try
            {
                Log.Debug($"批量插入1{context.CurrentContext}");
                using (SqlBulkCopy sqlRevdBulkCopy = new SqlBulkCopy(context.CurrentContext))
                {
                    Log.Debug($"批量插入2{context.CurrentContext}");
                    sqlRevdBulkCopy.DestinationTableName = strTableName;
                    sqlRevdBulkCopy.NotifyAfter = dtData.Rows.Count;
                    sqlRevdBulkCopy.ColumnMappings.Add("TEST_ITEM_ID", "TEST_ITEM_ID");
                    sqlRevdBulkCopy.ColumnMappings.Add("ENGLISH_NAME", "ENGLISH_NAME");
                    sqlRevdBulkCopy.ColumnMappings.Add("CHINESE_NAME", "CHINESE_NAME");
                    sqlRevdBulkCopy.ColumnMappings.Add("QUANTITATIVE_RESULT", "QUANTITATIVE_RESULT");
                    sqlRevdBulkCopy.ColumnMappings.Add("INPATIENT_ID", "INPATIENT_ID");
                    sqlRevdBulkCopy.ColumnMappings.Add("INSERT_TIME", "INSERT_TIME");
                    sqlRevdBulkCopy.WriteToServer(dtData);
                    sqlRevdBulkCopy.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "批量插入错误");
            }
        }
        #endregion

    }
}
