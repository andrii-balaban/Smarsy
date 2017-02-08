using System.Net;
using System.Net.Mail;

namespace Smarsy.Logic
{
    public class EmailLogic
    {
        public void SendEmail(string emailTo, string subject, string body)
        {
            var fromAddress = new MailAddress("olxsender@gmail.com", "Smarsy наблюдатель");
            var toAddress = new MailAddress(emailTo, "Оценки");
            const string fromPassword = "1mCr3at1nG$3cur3P4$$";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
