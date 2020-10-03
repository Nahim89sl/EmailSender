using System.Collections.ObjectModel;
using System.Linq;
using EmailSender.Interfaces;
using EmailSender.Model;
using MimeKit;
using NLog;
using ReaderMails;

namespace EmailSender.Services
{
    public class ReaderMailKitService : IReaderLetter
    {
        private string readFolderName;
        private string trashFolderName;

        public ReaderMailKitService()
        {
            readFolderName = "Read";
            trashFolderName = "Trash";
        }

        public ObservableCollection<Letter> ReadMails(Akkaunt akkaunt, string stopWords)
        {
            ObservableCollection<Letter> resList = new ObservableCollection<Letter>();
            var rederakk = new EmailBoxAkkaut(){Server = akkaunt.Domen,Login = akkaunt.Login,Pass = akkaunt.Pass,Port = 143};
            var mailReader = new MailKitReader();
            var lettersList = mailReader.ReaderMails(rederakk, "Read", "Trash", stopWords);
            if (lettersList != null)
            {
                return ConvertLetters(lettersList);
            }
            return null;
        }

        private ObservableCollection<Letter> ConvertLetters(ObservableCollection<MimeMessage> letterList)
        {
            var resList = new ObservableCollection<Letter>();
            foreach (var letter in letterList)
            {
                resList.Add(new Letter()
                {
                    //Id = letter.MessageId
                    EmailSender = letter.From.Mailboxes.First().Address,
                    Subject = letter.Subject,
                    Text = letter.TextBody
                });
            }
            return resList;
        }
    }
}
