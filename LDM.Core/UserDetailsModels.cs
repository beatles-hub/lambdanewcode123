using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace LDM.Core
{
    public class UserDetailsModels
    {
        [Required]        
        public string UserCode { get; set; }
    }
}
