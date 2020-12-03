using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class AppSettingsModels
    {
        public string Server { get; set; }
        public string Uid { get; set; }
        public string Pwd { get; set; }
        public string Database { get; set; }
        public string ConnectionString { get; set; }
        public string UploadDocumentFilePath { get; set; }
        public string AllowExtensions { get; set; }
        public AWSModels AWS { get; set; }
    }
    public class AWSModels
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        //public string BucketURL { get; set; }
        public string Region { get; set; }

    }
    public class ConnectionStringsModels
    {
       
        public string Connection { get; set; }       

    }
}
