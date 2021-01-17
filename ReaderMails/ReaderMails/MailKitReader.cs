using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using MailKit.Search;


//make visible all internal methods for our test library

namespace ReaderMails
{
    public class MailKitReader
    {
        private ImapClient _imapClient;
        private Logger _logger;
        private IMailFolder _destFolder;
        private IMailFolder _trashFolder;
        private string _libName;
        private const string GoodState = "ok";

        public MailKitReader(Logger logger)
        {
            _logger = logger;

            //logger = LogManager.GetCurrentClassLogger();
            _logger.Info($"{_libName} Load app MailReader");
            _libName = "ReaderMail";
        }

        public ObservableCollection<MimeMessage> ReaderMails(EmailBoxAkkaut akkaunt, string destFolderName, string trashFolderName, string stopWords)
        {
            ObservableCollection<MimeMessage> resMessages = null;
            //try connect to server
            ConnectToServer(akkaunt);

            if (_imapClient.IsConnected)
            {
                try
                {
                    _destFolder = CheckFolderByName(destFolderName);
                    _trashFolder = CheckFolderByName(trashFolderName);
                    if (_destFolder == null)
                    {
                        _destFolder = CreateFolder(destFolderName);
                    }
                    if (_trashFolder == null)
                    {
                        _trashFolder = CreateFolder(trashFolderName);
                    }
                    //check foldes values
                    if ((_destFolder == null) || (_trashFolder == null))
                    {
                        _logger.Error($"ConnectToserver some problem with folders {trashFolderName} {destFolderName}");
                        return null;
                    }
                    //check stop wors value
                    if (stopWords.Length < 5)
                    {
                        _logger.Error($"ConnectToserver you didn't have stop words {stopWords}");
                        return null;
                    }
                    _logger.Info($"{_libName} Destination folder - {_destFolder.FullName};  Trash folder - {_trashFolder.FullName}");
                    resMessages = FilterMailsBySubject(stopWords);
                    return resMessages;
                }catch(Exception ex)
                {
                    _logger.Error($"ReaderMails {ex.Message}");
                    return resMessages;
                }              
            }
            _logger.Error($"{_libName} Akkaunt status is {akkaunt.AccountStatus}");
            return null;
        }
        
        //public messages by subject
        public ObservableCollection<MimeMessage> FilterMailsBySubject(string stopWords)
        {
            ObservableCollection<MimeMessage> answerReceivers = new ObservableCollection<MimeMessage>();
            try
            {
                
                var mailsUids = _imapClient.Inbox.Search(SearchQuery.All);
                _logger.Info($"{_libName} Total mails in INBOX: {mailsUids.Count.ToString()}");
                int k = 0;

                foreach (UniqueId uidMail in mailsUids)
                {
                    var tempMessage = FilterMailBySubject(uidMail, stopWords);
                    if (tempMessage != null)
                    {
                        answerReceivers.Add(tempMessage);
                    }
                    k++;
                    if (k > 200) { break; }
                }
                return answerReceivers;
            }catch(Exception ex)
            {
                _logger.Error($"FilterMailsBySubject {ex.Message}");
                return answerReceivers;
            }           
        }

        //sort mail by subject
        public MimeMessage FilterMailBySubject(UniqueId uidMail, string stopWords)
        {
            try
            {
                var message = _imapClient.Inbox.GetMessage(uidMail);
                string subject = GetEmailSubject(message);
                //if stop words exist than return false
                if (ExistStopWords(subject, stopWords))
                {
                    _imapClient.Inbox.MoveTo(uidMail, _trashFolder);
                    _imapClient.Inbox.AddFlags(uidMail, MessageFlags.Seen, true); //mark is read
                    _logger.Info($"{_libName} {subject} move to Trash");
                    return null;
                }
                else
                {
                    _imapClient.Inbox.MoveTo(uidMail, _destFolder);
                    //_imapClient.Inbox.AddFlags(uidMail, MessageFlags.Seen, true); //mark is read
                    _logger.Info($"{_libName}  Subject: {subject}  -- move to {_destFolder.FullName}");
                    return message;
                }
            }catch(Exception ex)
            {
                _logger.Error($"FilterMailBySubject {ex.Message}");
                return null;
            }            
        }
        
        //connect to server
        public void ConnectToServer(EmailBoxAkkaut account)
        {
            //#1 try connect to server on right port and domen
            try
            {
                _imapClient = new ImapClient();
                _imapClient.Timeout = 20000;
                _logger.Info($"{_libName} try connnect to server");
                _imapClient.Connect(account.Server, account.Port, SecureSocketOptions.None);
                _logger.Info($"{_libName} Connected to server");
                account.ServerStatus = GoodState;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                account.ServerStatus = ex.Message;
                return;
            }
            //#2 try authenticate on server
            try{
                _imapClient.Authenticate(account.Login, account.Pass);
                _logger.Info($"{_libName} Try auth to server");
                _imapClient.Inbox.Open(FolderAccess.ReadWrite);
                _logger.Info($"{_libName} Authenticated");
                account.AccountStatus = GoodState;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message); 
                account.AccountStatus = ex.Message;
                _imapClient.Dispose();
            }
        }
        
        //check if folder exist
        public IMailFolder CheckFolderByName(string folderName)
        {
            //get all folders from server
            IList<IMailFolder> folders = _imapClient.GetFolders(_imapClient.PersonalNamespaces[0]);
            var findFolder = folders.Where(a => a.Name.Equals(folderName)).FirstOrDefault();
            return findFolder;
        }
        
        //create folder
        public IMailFolder CreateFolder(string folderName)
        {
            try
            {
                var lok = _imapClient.GetFolder(_imapClient.PersonalNamespaces[0]);
                var resFolder = lok.Create(folderName, true);
                return resFolder;
            }
            catch (Exception ex)
            {
                _logger.Error($"{_libName}  {ex.Message}");
                return null;
            }
        }
        
        //Get Subject
        public string GetEmailSubject(MimeMessage message)
        {
            string subject = message?.Subject ?? "No_subject";
            if (subject != null)
            {
                return subject;
            }
            return "";
        }

        //find words in text
        public bool ExistStopWords(string text, string findWords)
        {
            Regex rgx = new Regex(findWords);
            var match = rgx.Match(text);
            if (match.Success)
            {
                return true;
            }
            return false;
        }

    }
}
