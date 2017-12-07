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
        public static void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                const string fromEmail = "support@kamsham.pk";
                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    To = { toEmail },
                    Subject = subject,
                    Body = body,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };
                using (SmtpClient smtpClient = new SmtpClient("webmail.kamsham.pk"))
                {
                    smtpClient.Credentials = new NetworkCredential("support@kamsham.pk", "vakR69~0");
                    smtpClient.Port = 25;
                    smtpClient.EnableSsl = false;
                    smtpClient.Send(message);
                }
            }
            catch (Exception ffg)
            {

            }
        }
    }
    
}