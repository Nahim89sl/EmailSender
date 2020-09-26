using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EmailSender.Interfaces;
using EmailSender.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using NLog;

namespace EmailSender.Services
{
    public class ReaderMailKitService : IReaderLetter
    {
        private Logger logger;
        private string readFolderName;
        private string trashFolderName;
        private ImapClient client;
        private IMailFolder folderRead;
        private IMailFolder folderTrash;

        public ReaderMailKitService()
        {
            logger = LogManager.GetCurrentClassLogger();
            readFolderName = "Read";
            trashFolderName = "Trash";
        }
        public ObservableCollection<Letter> ReadMails(Akkaunt akkaunt,string stopWords)
        {
             client = new ImapClient();       
            //connect to server
            try 
                {
                    //connect to server
                    client.Connect(akkaunt.Domen, 143, SecureSocketOptions.None);
                    client.Authenticate(akkaunt.Login, akkaunt.Pass);
                    client.Inbox.Open(FolderAccess.ReadWrite);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            //check exist folder Read
            CheckReadFolder();
            //take trash folder
            CheckTrashFolder();
            //filtring letters
            var res = FilterMails(stopWords);
            client.Dispose();
            return res;
        }
        private ObservableCollection<Letter> FilterMails(string stopWords)
        {
            ObservableCollection<Letter> answerReceivers = new ObservableCollection<Letter>();
            var mailsUids = client.Inbox.Search(SearchQuery.All);
            int k = 0;
            foreach(var uidMail in mailsUids)
            {
                Letter letter = new Letter();
                MimeMessage message = client.Inbox.GetMessage(uidMail);
                letter.EmailSender = GetEmailSender(message);
                letter.Subject = GetEmailSubject(message);

                bool delLetter = (letter.Subject.Length > 0) ? FilterText(letter.Subject,stopWords) : FilterText(letter.Text,stopWords);

                if (delLetter)
                {
                    client.Inbox.MoveTo(uidMail, folderTrash);
                    logger.Info(letter.Subject + "move to Trash");
                }
                else
                {
                    client.Inbox.MoveTo(uidMail, folderRead);
                    answerReceivers.Add(letter);
                    logger.Info(letter.Subject + "move to Read");
                }
                k++;
                if (k > 300) { break; }
            }
            return answerReceivers;
        }
        private void CheckReadFolder()
        {
            //get all folders from server
            IList<IMailFolder> folders = client.GetFolders(client.PersonalNamespaces[0]);
            //try to find read folder
            folderRead = folders.Where(a => a.Name == readFolderName).FirstOrDefault();
            try
            {
                if(folderRead == null)
                {
                    var lok = client.GetFolder(client.PersonalNamespaces[0]);
                    folderRead = lok.Create(readFolderName, true);
                    logger.Info("Create Readfolder");
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            
        }
        private void CheckTrashFolder()
        {
            folderTrash = client.GetFolder(SpecialFolder.Trash);
            if(folderTrash == null)
            {
                IList<IMailFolder> folders = client.GetFolders(client.PersonalNamespaces[0]);
                folderTrash = folders.Where(a => a.Name == trashFolderName).FirstOrDefault();
                if(folderTrash == null)
                {
                    logger.Error("Cant finde trash folder");
                }
            }
        }
        private string GetEmailSender(MimeMessage message)
        {
            try
            {
                foreach (var mailbox in message.From.Mailboxes)
                {
                    
                    if(mailbox.Address.Contains("@"))
                    {
                        return mailbox.Address;
                    }
                    else
                    {
                        return mailbox.Name.Contains("@") ? mailbox.Name : "";
                    }                    
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);                
            }
            return "";
        }
        private string GetEmailSubject(MimeMessage message)
        {
            string subject = message.Subject;
            if(subject != null) 
            { 
                return subject; 
            } 
            return "";
        }
        
        
        private bool FilterText(string text, string stopWords)
        {
            Regex rgx = new Regex(stopWords);
            var match = rgx.Match(text);
            if (match.Success)
            {
                return true;
            }
            return false;    
        }

    }
}
