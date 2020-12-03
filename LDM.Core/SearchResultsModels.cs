using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class SearchResultsModels
    {
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string CategoryCode { get; set; }
        [Required]
        public string Condition { get; set; }
      
        public bool UIRequest { get; set; }
        [Required]
        public List<SearchFieldsModels> SearchFields { get; set; }
    }
    public class SearchFieldsModels
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
    }
}
