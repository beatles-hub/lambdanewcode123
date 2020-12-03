using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class UploadDocumentDetailsModels
    {
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string CategoryCode { get; set; }
        //[Required]
        //public ColumnModels UploadMetaData { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public List<UploadFieldsModels> UploadFields { get; set; }
    }
    public class UploadFieldsModels
    {
        public string Field { get; set; }
        public string Value { get; set; }       
    }
}
