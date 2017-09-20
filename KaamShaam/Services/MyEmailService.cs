using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace KaamShaam.Services
{
    public static class EmailService
    {
        public static void SendEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage message = new System.Net.Mail.MailMessage();
                string fromEmail = "Support@kamsham.pk";
                string toEmail = to;
                message.From = new MailAddress(fromEmail);
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("link2naqvi@gmail.com", "@pacein_786");

                    smtpClient.Send(message.From.ToString(), message.To.ToString(),
                                    message.Subject, message.Body);
                }
            }
            catch (Exception ffg)
            {

            }
        }
    }
}