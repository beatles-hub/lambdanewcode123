using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class UserModels
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        //public string UserInfo { get; set; }
        //public string UserType { get; set; }
        //public string WindowsUser { get; set; }
        //public string WindowsEmail { get; set; }

    }
    public class UserAllDetailsModels
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public string UserInfo { get; set; }
        public string UserType { get; set; }
        public string WindowsUser { get; set; }
        public string WindowsEmail { get; set; }

    }
}
