using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace KGERP.Utility
{
    public static class MailService
    {
        public static string Request
        {
            get
            {
                return ConfigurationManager.AppSettings["loginService"];
            }
        }
        public static bool SendMail(string senderEmail, string senderName, string receiverEmail, string receiverName, string ccEmail, string ccName, string subject, string body, out string sendStatus)
        {
            sendStatus = "Success";
            bool result = false;
            senderEmail = "kgerp19@gmail.com";
            senderName = "Krishibid Group";
            var password = "kfl@admin321";
            var fromEmail = new MailAddress(senderEmail, senderName);
            var toEmail = new MailAddress(receiverEmail, receiverName);

            //var ccEmail = new MailAddress("swashraf@krishibidgroup.com", "Md. Asraf");


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }
            )

            {

                try
                {
                    //mess.CC.Add(ccEmail);
                    smtp.Send(mess);
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                    sendStatus = "Failed";
                    throw;
                }

            }

            return result;
        }
    }
}
