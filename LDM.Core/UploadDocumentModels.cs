
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class UploadDocumentModels
    {
        [Required]
        public string UniqueId { get; set; }
        [Required]
        public string ItemId { get; set; }       
    }
}
