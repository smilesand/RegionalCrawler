using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool
{
    public class DataTableTool
    {
        /// <summary>
        /// 表合并
        /// </summary>
        /// <param name="target">合入表</param>
        /// <param name="Passive"><被合入表/param>
        /// <returns></returns>
        public static DataTable DataTableMerge(DataTable target, List<DataTable> Passive)
        {
            DataTable newtable = target.Copy();
            for (int j = 1; j < Passive.Count; j++)
            {
                for (int i = 0; i < Passive[j].Rows.Count; i++)
                {
                    newtable.Rows.Add(Passive[j].Rows[i].ItemArray);
                }
            }
            return newtable;
        }
    }
}
