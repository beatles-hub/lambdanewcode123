using LDM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Utility
{
    public class EmailClient
    {
        public void SendEmail(SendEmailModel emailModel)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress(emailModel.configuration.SMTPEmail);
                message.To.Add(new MailAddress(emailModel.ToEmail));
                if (!string.IsNullOrEmpty(emailModel.configuration.EmailCC))
                {
                    var ccValues = emailModel.configuration.EmailCC.Split(';');
                    foreach (var emailCC in ccValues)
                    {
                        if (!string.IsNullOrEmpty(emailCC))
                        {
                            message.CC.Add(new MailAddress(emailCC));
                        }
                    }
                }           
                message.Subject = emailModel.Subject;
                message.IsBodyHtml = true;
                message.Body = emailModel.Body;
                smtp.Port = emailModel.configuration.SMTPPort;
                smtp.Host = emailModel.configuration.SMTPHost;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(emailModel.configuration.SMTPEmail, emailModel.configuration.SMTPEmailPwd);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception)
            {

            }
        }

    }
}
