using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class DocumentModels:BaseResponse
    {
        public string UniqueId { get; set; }       
        public string ItemId { get; set; }
        public string FileName { get; set; }
        public string FileURL { get; set; }
    }
}
