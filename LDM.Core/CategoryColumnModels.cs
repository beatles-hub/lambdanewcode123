using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class CategoryColumnModels
    {
        public int ColumnID { get; set; }
        public string ColumnName { get; set; }
        public string ColumnAlias { get; set; }
        public string Type { get; set; }
        public string MaxLength { get; set; }
        public bool InputRequiredForSearch { get; set; }
        public bool InputRequiredForUpload { get; set; }
    }
}
