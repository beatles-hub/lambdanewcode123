using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class CategoryDetailsModels
    {
        public string CategoryName { get; set; }
        public string CategoryAlias { get; set; }
        public string CategoryDesc { get; set; }
        public string TableName { get; set; }
        public string ViewName { get; set; }
        public string Main_Folder_Column { get; set; }
        public string Folder_Column { get; set; }
        public string S3BucketName { get; set; }
    }
}
