using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MailKit.Search;
using AppCommon.Interfaces;
using AppCommon.MailObj;
using AppCommon;


namespace ReaderMails
{
    
    /// <summary>
    /// This class loggin in mail server
    /// than read all mails in INBOX
    /// and sort mails by stop words in subject
    /// if mail contains stop word than mail move to TRASH
    /// else to Our mail folder
    /// 
    /// class return list of all mails with diffrent statuses
    /// </summary>
    public class MailKitReader : IMailReaderService
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

        public IList<IMailAnswer> ReaderMails(IMailAkk akkaunt, string destFolderName, string trashFolderName, string stopWords, string emailBlackList)
        {
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
                        _logger.Error($"ConnectToserverr you didn't have stop words {stopWords}");
                        return null;
                    }
                    _logger.Info($"{_libName} Destination folder - {_destFolder.FullName};  Trash folder - {_trashFolder.FullName}");
                    return FilterMailsBySubject(stopWords, emailBlackList);
                }catch(Exception ex)
                {
                    _logger.Error($"ReaderMails {ex.Message}");
                    return null;
                }              
            }
            _logger.Error($"{_libName} Akkaunt status is {akkaunt.AccountStatus}");
            return null;
        }
        
        //public messages by subject
        public IList<IMailAnswer> FilterMailsBySubject(string stopWords, string emailBlackList)
        {
            IList<IMailAnswer> answerReceivers = new List<IMailAnswer>();
            try
            {
                
                var mailsUids = _imapClient.Inbox.Search(SearchQuery.All);
                _logger.Info($"{_libName} Total mails in INBOX: {mailsUids.Count.ToString()}");
                int k = 0;

                foreach (UniqueId uidMail in mailsUids)
                {
                    var tempMessage = FilterMailBySubject(uidMail, stopWords, emailBlackList);
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
        public IMailAnswer FilterMailBySubject(UniqueId uidMail, string stopWords, string emailBlackList)
        {
            try
            {
                var message = _imapClient.Inbox.GetMessage(uidMail);
                if (message == null) 
                {
                    _logger.Error($"Can't get message from uid {uidMail}");
                    return null; 
                }
                var answer = new MailAnswer() { Message = message, Status = MailStatus.Empty };
                string subject = answer.EmailSubject;



                //Filtration with stop words
                if ((ExistStopWords(subject, stopWords))||(ExistStopWords(answer.EmailAddress, emailBlackList)))
                {
                    _imapClient.Inbox.MoveTo(uidMail, _trashFolder);
                    _imapClient.Inbox.AddFlags(uidMail, MessageFlags.Seen, true); //mark is read
                    _logger.Info($"{_libName} {subject} move to Trash");
                    answer.Status = MailStatus.Block;
                }
                else
                {
                    _imapClient.Inbox.MoveTo(uidMail, _destFolder);
                    //_imapClient.Inbox.AddFlags(uidMail, MessageFlags.Seen, true); //mark is read
                    _logger.Info($"{_libName}  Subject: {subject}  -- move to {_destFolder.FullName}");
                    answer.Status = MailStatus.Good;
                }
                return answer;
            }
            catch(Exception ex)
            {
                _logger.Error($"FilterMailBySubject {ex.Message}");
                return null;
            }            
        }
        
        //connect to server
        public void ConnectToServer(IMailAkk account)
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
