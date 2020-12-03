using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Core
{
    public class DownloadDocumentDetailsModels
    {
        public string ItemId { get; set; }
        public List<FileModels> Files { get; set; }
    }
    public class FileModels
    {
        public string FileName { get; set; }
        public string FileURL { get; set; }
    }
    public class EmailFileModels
    {
        public string FileName { get; set; }
        public string FileURL { get; set; }
        public bool EmailZip { get; set; }
    }
    public class SendEmailModel
    {        
        public string ToEmail { get; set; }       
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailConfiguration configuration { get; set; }

    }
    public class EmailConfiguration
    {
        public string SMTPEmail { get; set; }
        public string SMTPEmailPwd { get; set; }
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string EmailCC { get; set; }       
    }
}
