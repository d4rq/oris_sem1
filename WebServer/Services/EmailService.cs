using System.Net.Mail;
using HttpServerLibrary.Configuration;

namespace WebServer.services
{
    internal class EmailService : IEmailService
    {
        private AppConfig config = AppConfig.GetInstance();
        public void SendEmail(string email, string subject, string message)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(config.NetworkCredential!.UserName);
            MailAddress to = new MailAddress(email);
            MailMessage m = new MailMessage(from, to);
            m.Subject = subject;
            m.Body = message;
            m.IsBodyHtml = true;
            //m.Attachments.Add(new Attachment("appconfig.json"));

            SmtpClient smtp = config.SmtpClient!;
            smtp.Credentials = config.NetworkCredential;
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
