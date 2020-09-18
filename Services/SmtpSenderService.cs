using EmailSender.Interfaces;
using EmailSender.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace EmailSender.Services
{
    public class SmtpSenderService 
    {
        SmtpClient _client;
        //ImapClient _client;

        public SmtpSenderService()
        {
            _client = new SmtpClient();
            //_client = new ImapClient();

            //_client.Connect(mailDomen, 143, SecureSocketOptions.None);

            //_client.Authenticate(mailAddress, mailPass);

            //_client.Inbox.Open(FolderAccess.ReadWrite);
        }
        
        public void SendAsync(Akkaunt mailServer, Receiver receiver, string textMail, string subject)
        {
            try
            {            
                _client.Connect("185.227.109.114", 465, SecureSocketOptions.None);
                _client.Authenticate("sdelat@ydostovereniya.ru", "jICfEJrN4WcR");               
            }
            catch(Exception ex)
            {
                var k = ex.Message;
            }
                
            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            message.From.Add(new MailboxAddress(mailServer.Login, mailServer.Login));
            message.To.Add(new MailboxAddress(receiver.CompanyName, receiver.Email));
            //message.Cc.Add(new MailboxAddress(receiver.PersonName, hiddenMail));

            message.Subject = subject;
            bodyBuilder.TextBody = textMail;

            message.Body = bodyBuilder.ToMessageBody();

            _client.Send(message);
            _client.Disconnect(true);

            
           
        }

        private void Connect()
        {
            
            HttpClientHandler hendler = new HttpClientHandler();
            CookieContainer cookie = new CookieContainer();
            hendler.CookieContainer = cookie;
            HttpClient client = new HttpClient(hendler);

            
        }


    }
}
