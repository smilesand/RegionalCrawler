using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Model
{
    public class RegionalModel
    {
        public Guid RegionalDataOID { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public string LevelEx { get; set; }

        public string fID { get; set; }

        public Guid ParentOID { get; set; }
    }
}
