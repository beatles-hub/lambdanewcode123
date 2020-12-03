using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class UploadSuccessModels
    {
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string CategoryCode { get; set; }
        [Required]
        public string ITEM_ID { get; set; }
    }
}
