using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailReaderWeb;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using MimeKit;
using NLog;
using ReaderMails;
using StyletIoC;
using Receiver = EmailSender.Models.Receiver;

namespace EmailSender.Services
{
    class ReaderMailsService : IReaderMails
    {

        [Inject] public MainAccount MainAccount;
        [Inject] private Logger.ILogger logger;
        private EmailBoxAkkaut emailBoxAkkaut;
        private ObservableCollection<Receiver> resultList;

        public ObservableCollection<Answer> ReadMails(string stopWords)
        {
            ConvertMainAccToBoxAcc();
            var service = new MailKitReader(LogManager.GetLogger("Reader")); 
            var res = service.ReaderMails(emailBoxAkkaut,"Read","Trash", stopWords);
            ConvertBoxAccToMainAcc();
            return ConvetResults(res);
        }

        private void ConvertMainAccToBoxAcc()
        {
            emailBoxAkkaut = new EmailBoxAkkaut()
            {
                Login = MainAccount.Login,
                Pass = MainAccount.Pass,
                Server = MainAccount.Server,
                Port = 143
            };
        }

        private void ConvertBoxAccToMainAcc()
        {
            MainAccount.ServerStatus = emailBoxAkkaut.ServerStatus;
            MainAccount.AccountStatus = emailBoxAkkaut.AccountStatus;
        }

        private ObservableCollection<Answer> ConvetResults(ObservableCollection<MimeMessage> mailList)
        {
            ObservableCollection<Answer> resList = new ObservableCollection<Answer>();
            foreach (var letter in mailList)
            {
                resList.Add(new Answer()
                {
                    From = letter.From.ToString(),
                    Email = letter.From.Mailboxes.First().Address.ToLower(),
                    Subject = letter.Subject.ToString(),
                    Text = letter.TextBody
                });
            }
            return resList;
        }
    }
}
