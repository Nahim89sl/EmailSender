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
        private ILogger _logger;
        private IMailFolder _destFolder;
        private IMailFolder _trashFolder;
        private string _libName;
        private IConsts _consts;


        public MailKitReader(ILogger logger, IConsts consts)
        {
            _logger = logger;
            _consts = consts;
            _logger.Info($"{_libName} Load app MailReader");
            _libName = "ReaderMail";
        }

        public IList<IMailAnswer> ReaderMails(
            IMailAkk akkaunt, 
            string destFolderName, 
            string trashFolderName, 
            string subjectStopWords, 
            string bodyStopWords, 
            string emailBlackList)
        {
            //check stop words value
            if ((subjectStopWords.Length < 5)||(bodyStopWords.Length < 5))
            {
                _logger.Error($"ConnectToserverr you didn't have stop words {subjectStopWords}");
                return null;
            }


            //try connect to server
            ConnectToServer(akkaunt);

            if (_imapClient.IsConnected)
            {
                try
                {
                    CheckFolders(_consts.ReadFolder, _consts.TrashFolder);                   
                    return FilterMails(subjectStopWords, bodyStopWords, emailBlackList);
                
                }catch(Exception ex)
                {
                    _logger.Error($"ReaderMails {ex.Message}");
                    return null;
                }              
            }
            _logger.Error($"{_libName} Akkaunt status is {akkaunt.AccountStatus}");
            return new List<IMailAnswer>();
        }
        
        //public messages by subject
        public IList<IMailAnswer> FilterMails(string subjectStopWords, string bodyStopWords, string emailBlackList)
        {
            IList<IMailAnswer> answerReceivers = new List<IMailAnswer>();
            try
            {
                
                var mailsUids = _imapClient.Inbox.Search(SearchQuery.All);
                _logger.Info($"{_libName} Total mails in INBOX: {mailsUids.Count.ToString()}");
                int k = 0;

                foreach (UniqueId uidMail in mailsUids)
                {
                    var tempMessage = FilterMail(uidMail, subjectStopWords, bodyStopWords, emailBlackList);
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

        //sort mail 
        public IMailAnswer FilterMail(UniqueId uidMail, string subjectStopWords, string bodyStopWords, string emailBlackList)
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
                if ((ExistStopWords(subject, subjectStopWords))
                    ||(ExistStopWords(answer.EmailAddress, emailBlackList))
                    ||(ExistStopWords(answer.EmailText, bodyStopWords)))
                {
                    
                    _imapClient.Inbox.MoveTo(uidMail, _trashFolder);
                    _imapClient.Inbox.AddFlags(uidMail, MessageFlags.Seen, true); //mark is read
                    _logger.Info($"{_libName} Move to: {_consts.TrashFolder} - {subject}");
                    answer.Status = MailStatus.Block;
                }
                else
                {
                    _imapClient.Inbox.MoveTo(uidMail, _destFolder);
                    //_imapClient.Inbox.AddFlags(uidMail, MessageFlags.Seen, true); //mark is read
                    _logger.Info($"{_libName} Move to: {_consts.ReadFolder} Subject: {subject}");
                    answer.Status = MailStatus.Good;
                }
                return answer;
            }
            catch(Exception ex)
            {
                _logger.Error($"FilterMailBySubject {ex.Message} \n ");
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
                account.ServerStatus = _consts.ServerStatusOk;
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
                account.AccountStatus = _consts.ServerStatusOk;
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
            try
            {
                Regex rgx = new Regex(findWords);
                var match = rgx.Match(text);
                if (match.Success)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"ExistStopWords {ex.Message}\n         text {text}\n     findwords{findWords}");
            }           
            return false;
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

    }
}
