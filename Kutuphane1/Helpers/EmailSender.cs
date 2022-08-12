using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace Kutuphane1.Helpers
{
    public class EmailSender : IEmailSender
    {
        public int PORT { get; set; }
        public string HOST { get; set; }
        public bool EnableSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public EmailSender()
        {
            PORT = 587;
            HOST = "smtp.yandex.ru";
            EnableSSL = true;
            UserName = "aribilgiogretmen@yandex.ru";
            Password = "Ab2022@@@";
            From = "aribilgiogretmen@yandex.ru";
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = new()
            {
                Host = HOST,
                Port = PORT,
                EnableSsl = EnableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(UserName, Password)
            };
            return client.SendMailAsync(From, email, subject, htmlMessage);
        }

    }
}
