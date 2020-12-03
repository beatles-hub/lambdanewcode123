using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
   public class MetaDataModels
    {
        public List<SearchParametersDisplay> Headers { get; set; }
        public List<dynamic> ResultLists { get; set; }
        public int TotalRows { get; set; }
    }
    public class SearchParametersDisplay
    {
        public string Field { get; set; }
        public string Header { get; set; }
        public bool Hidden { get; set; }
    }
    public class MetaDataRiskTechModels
    {        
        public List<dynamic> ResultLists { get; set; }
        public int TotalRows { get; set; }
    }
}
