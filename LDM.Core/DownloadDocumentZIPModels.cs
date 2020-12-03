using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class DownloadDocumentZIPModels
    {
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string CategoryCode { get; set; }
        [Required]
        public List<string> ItemIds { get; set; }
        [Required]
        public string DownloadMethod { get; set; }
    }
}
