using System.Net.Mail;
using SmarsyEntities;

namespace Smarsy.Email
{
    public class EmailClient
    {
        public void SendEmail(Email email, SmarsyCredentials credentials)
        {
            using (var mail = email.CreateMessage())
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = credentials.GetNetworkCredentials()
                };

                smtp.Send(mail);
            }
        }
    }
}
