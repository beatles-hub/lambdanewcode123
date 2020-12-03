
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class BaseResponse
    {
        public int ResponseCode { get; set; }  
        public string ResponseMessage { get; set; }      
        public dynamic Data { get; set; }
    }
}
