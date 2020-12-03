using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class SearchOpeatorResultModels
    {
        public List<SearchOpeators> TextOperators { get; set; }
        public List<SearchOpeators> NumberOperators { get; set; }
        public List<SearchOpeators> DateOperators { get; set; }

    }
    public class SearchOpeators
    {
        public string OperatorName { get; set; }
        public string OperatorAlias { get; set; }
        public string ColumnType { get; set; }
    }
}
