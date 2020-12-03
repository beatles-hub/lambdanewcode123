using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class SecretManagerModels
    {
        public string password { get; set; }
        public string dbname { get; set; }
        public string engine { get; set; }
        public string port { get; set; }
        public string host { get; set; }

        public string username { get; set; }
    }
}
