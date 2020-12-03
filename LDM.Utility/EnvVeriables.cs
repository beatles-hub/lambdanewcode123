using Amazon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Utility
{
    public static class EnvVeriables
    {
        // ***** Dev Debug *******

        //public static string ConnectionString = @"SERVER=database-2-instance-1.csvxhlqropdh.ap-south-1.rds.amazonaws.com;DATABASE=LDM_DB_QA1_Dev1;UID=admin;PASSWORD=password;convert zero datetime=True;";
        public static string AllowExtensions = "txt,doc,docx,pdf,tiff,tif,xls,xlsx";
        //public static int DownloadUrlExpires = 180;
        public static int UploadUrlExpires = 5;
        //public static string Region = "ap-south-1";
        //public static string DBName = "LDM_DB_QA1_Dev1";
        //public static string SecretManagerKey = string.Empty;
        //public static long DownloadLimit = 20;//In Mb
        //public static string AllowOrigin = "*";//In Mb
        //public static string EmailFunction = Environment.GetEnvironmentVariable(Constants.EmailFunction);
        //public static string SendEmailFunction = Environment.GetEnvironmentVariable(Constants.SendEmailFunction);



        // ****** On AWS ******
        public static string AllowOrigin = Environment.GetEnvironmentVariable(Constants.CORSAllowUrl);       
        public static string EmailFunction = Environment.GetEnvironmentVariable(Constants.EmailFunction);
        public static string SendEmailFunction = Environment.GetEnvironmentVariable(Constants.SendEmailFunction);
        public static string DownloadLimit = Environment.GetEnvironmentVariable(Constants.BrowserDownloadLimit);       
        public static string DBName = Environment.GetEnvironmentVariable(Constants.DBName);       
        public static int DownloadUrlExpires = Convert.ToInt32(Environment.GetEnvironmentVariable(Constants.DownloadUrlTimeOut));      
        public static string Region = Environment.GetEnvironmentVariable(Constants.Region);
        public static string SecretManagerKey = Environment.GetEnvironmentVariable(Constants.SecretManagerKey);
        public static string ConnectionString
        {
            get
            {
                return Constants.GetConnectionString();
            }
        }

    }
}
