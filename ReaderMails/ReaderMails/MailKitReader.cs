using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Search;
using ReaderMails.Models;
using ReaderMails.Interfaces;
using NLog;

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
    public class MailKitReader
    {
        private ImapClient _imapClient;
        private ILogger _logger;
        private IMailFolder _destFolder;
        private IMailFolder _trashFolder;
        private string _trashFolderName;
        private string _destFolderName;
        private string _libName;

        private readonly ServerStatus SrvState;

        public MailKitReader(ILogger logger, string destFolder, string trashFolder)
        {
            _logger = logger;
            _trashFolderName = trashFolder;
            _destFolderName = destFolder;
            _logger.Info($"{_libName} Load app MailReader");
            _libName = "ReaderMail";
            SrvState = new ServerStatus();
        }

        public bool ReadMails(IMailAkk akkaunt, EmailFiltrator filtrator)
        {
            try
            {
                ConnectToServer(akkaunt);
                if (_imapClient.IsConnected)
                {
                    CheckFolders(_destFolderName, _trashFolderName);
                    var mails = LoadAllMails();
                    _logger.Info($"Downloaded {mails.Count()} mails from Inbox");
                    if (mails.Any())
                    {
                        filtrator.Filter(mails);
                        _logger.Info($"Загрузили почты в фильтратор");
                        MoveToTresh(filtrator.GetMailsForDelete);
                        _logger.Info($"Отфильтровали письма для удаления");
                        MoveToRead(filtrator.GetMailsForSave);
                        _logger.Info($"Отфильтровали письма для работы");
                        _logger.Info($"Reading and filtration finished: Move to treash {filtrator.GetMailsForDelete.Count()} Move to Read {filtrator.GetMailsForSave.Count()}");
                    }                   
                }
                else
                {
                    _logger.Error($"{_libName} Akkaunt status is {akkaunt.AccountStatus}");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"{_libName} Method - ReadMails:{ex.Message}");
                return false;
            }                       
            return true;
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
                account.Port = 143;
                _imapClient.Connect(account.Server, account.Port, SecureSocketOptions.None);
                _logger.Info($"{_libName} Connected to server");
                account.ServerStatus = SrvState.OK;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                account.ServerStatus = ex.Message;
                _imapClient.Dispose();
                return;
            }
            //#2 try authenticate on server
            try{
                _imapClient.Authenticate(account.Login, account.Pass);
                _logger.Info($"{_libName} Try auth to server");
                _imapClient.Inbox.Open(FolderAccess.ReadWrite);
                _logger.Info($"{_libName} Authenticated");
                account.AccountStatus = SrvState.OK;
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
        
        private void CheckFolders(string destFolder, string trashFolder)
        {
            _destFolder = CheckFolderByName(destFolder);
            _trashFolder = CheckFolderByName(trashFolder);
            if (_destFolder == null)
            {
                _destFolder = CreateFolder(destFolder);
            }
            if (_trashFolder == null)
            {
                _trashFolder = CreateFolder(trashFolder);
            }
            //check foldes values
            if ((_destFolder == null) || (_trashFolder == null))
            {
                _logger.Error($"ConnectToserver some problem with folders {trashFolder} {trashFolder}");
            }
            _logger.Info($"{_libName} Destination folder - {_destFolder.FullName};  Trash folder - {_trashFolder.FullName}");
        }

        #region private Methods

        private IEnumerable<IMailAnswer> LoadAllMails()
        {
            var resList = new List<IMailAnswer>();
            try
            {
                var mailsUids = _imapClient.Inbox.Search(SearchQuery.All);
                int letters = 0;
                foreach (var uid in mailsUids)
                {
                    var message = _imapClient.Inbox.GetMessage(uid);
                    if (message != null)
                    {
                        var mail = new MailAnswer(message);
                        mail.Id = uid;                        
                        resList.Add(mail);
                        _logger.Info(mail.EmailAddress);
                    }
                    letters++;
                    if (letters > 200)
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"LoadAllMails {ex.Message}");
            }
                      
            return resList;
        }

        private bool MoveToTresh(IEnumerable<IMailAnswer> badmails)
        {
            bool result = true;
            foreach(var mail in badmails)
            {
                try
                {
                    _imapClient.Inbox.MoveTo(mail.Id, _trashFolder);
                    _imapClient.Inbox.AddFlags(mail.Id, MessageFlags.Seen, true); //mark is read
                }
                catch(Exception ex)
                {
                    _logger.Error($"MoveToTresh: mail.Id {mail.Id}, _trashFolder {_trashFolder} Error {ex.Message}");
                    result = false;
                }                
            }
            return result;
        }

        private bool MoveToRead(IEnumerable<IMailAnswer> goodmails)
        {
            bool result = true;
            foreach (var mail in goodmails)
            {
                try
                {
                    _imapClient.Inbox.MoveTo(mail.Id, _destFolder);
                    _imapClient.Inbox.AddFlags(mail.Id, MessageFlags.Seen, true); //mark is read
                }
                catch (Exception ex)
                {                    
                    _logger.Error($"MoveToRead: mail.Id {mail.Id}, _trashFolder {_trashFolder} Error {ex.Message}");
                    result = false;
                }                
            }
            return result;
        }

        #endregion
    }
}
