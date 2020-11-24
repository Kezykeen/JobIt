using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace JobIT.web.Services
{
    public class SendMailNotification : ISendMailNotification
    {
        public async Task SendMail(string recieverEmail, string messageBody, string subject)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = "kezydevapps@gmail.com",
                    Password = "keenslee"
                };
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage mailMessage = new MailMessage()
                {
                    To = { recieverEmail },
                    Body = messageBody,
                    From = new MailAddress("okorokingsley250@gmail.com"),
                    Subject = subject
                };

                await smtpClient.SendMailAsync(mailMessage);
                smtpClient.Dispose();
            }
            catch (Exception e) { Console.WriteLine(e); }


        }
    }
}