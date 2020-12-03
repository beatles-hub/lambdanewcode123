using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class RiskTechDocumentByIdModel
    {
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string CategoryCode { get; set; }
        [Required]
        public List<string> DocumentIds { get; set; }
    }

    public class RiskTechDocumentByIdResultModel
    {
        public string Document_Id { get; set; }
        public string FileUrl { get; set; }
    }
}
