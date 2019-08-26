using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool
{
    public class StringHelp
    {
        /// <summary>
        /// 补充左边
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="largest"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] StringPadLeft(char cr, int largest, string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = str[i].PadLeft(largest, cr);
            }
            return str;
        }
        /// <summary>
        /// 补充左边
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="largest"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringPadLeft(char cr, int largest, string str)
        {
            str = str.PadLeft(largest, cr);
            return str;
        }
        /// <summary>
        /// 从开始位置移除
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] StringRemoveLeft(char cr, string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = str[i].TrimStart(cr);
            }
            return str;
        }
        /// <summary>
        /// 从开始位置移除
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringRemoveLeft(char cr, string str)
        {
            str = str.TrimStart(cr);
            return str;
        }
    }
}
