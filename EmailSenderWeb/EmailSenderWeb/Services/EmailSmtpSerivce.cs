using EmailReaderWeb;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading.Tasks;

namespace EmailSenderWeb.Services
{
    public class EmailSmtpSerivce
    {
        public async Task Authentification(ServerAccount mailServer)
        {
            // send email
            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect(mailServer.Server, mailServer.Port, SecureSocketOptions.None);
                    await smtp.AuthenticateAsync(mailServer.Login, mailServer.Pass);
                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    mailServer.ServerStatus = $"Auth error {ex.Message}";
                    throw new Exception(mailServer.ServerStatus);
                }
            }
        }

        public async Task SendAsync(ServerAccount mailServer, string receiver, string hiddenReceiver, string subject, string mailText)
        {
            var email = new MimeMessage();
            try
            {
                // create message
                
                email.From.Add(MailboxAddress.Parse(mailServer.Login));
                email.To.Add(MailboxAddress.Parse(receiver));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Text) { Text = mailText };
                if (hiddenReceiver != null && hiddenReceiver.Length > 3)
                    email.Bcc.Add(MailboxAddress.Parse(receiver));
            }
            catch(Exception ex)
            {
                mailServer.ServerStatus = $"Can't build email for sending {ex.Message}";
                throw new Exception(mailServer.ServerStatus);
            }
            

            // send email
            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect(mailServer.Server, mailServer.Port, SecureSocketOptions.None);
                    smtp.Authenticate(mailServer.Login, mailServer.Pass);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    mailServer.ServerStatus = $"Email sending for receiver {receiver} error {ex.Message}";
                    throw new Exception(mailServer.ServerStatus);
                }               
            }           
        }
    }
}
