namespace Smarsy
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;

    public class EmailClient
    {
        public void SendEmail(Email email, IEnumerable<string> emailToList,  string emailFrom, string fromPassword, string subject)
        {
            var emailBody = email.GenerateEmailBody();

            SendEmail(emailToList, subject, emailBody.ToString(), emailFrom, fromPassword);
        }

        private void SendEmail(IEnumerable<string> emailTo, string subject, string body, string emailFrom, string fromPassword)
        {
            var fromAddress = new MailAddress(emailFrom, "Smarsy наблюдатель");

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage()
            {
                From = fromAddress,
                Subject = subject,
                Body = body
            })
            {
                foreach (var mail in emailTo)
                {
                    message.To.Add(mail);
                }

                message.IsBodyHtml = true;
                smtp.Send(message);
            }
        }
    }
}
