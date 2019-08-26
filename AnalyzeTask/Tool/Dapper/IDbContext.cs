using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool.Dapper
{
    public interface IDbContext
    {
        string CurrentContext { get; }
        DbContext ChangeContext(string connectionStringName, bool useOnlyCurrent = false);
    }
}
