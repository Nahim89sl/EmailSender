

using AppCommon.Interfaces;
using EmailSender.Interfaces;
using NLog;
using ReaderMails;
using StyletIoC;
using System;
using System.Collections.Generic;

namespace EmailSender.Services
{
    class ReaderMailsService : IReaderMails
    {
        
        [Inject] public  IMailAkk MainAccount;
        [Inject] private  IConsts Consts;

        private MailKitReader kitReader;
        private ILogger readerLogger;

        #region Constructor

        public ReaderMailsService()
        {
            readerLogger = LogManager.GetLogger("Reader");
        }

        #endregion

        public IList<IMailAnswer> ReadMails(string SubjectStopWords, string BodyStopWords, string emailBlackList)
        {
                       
            try
            {
                if(kitReader == null)
                {
                    readerLogger.Info($"Reader created 1.0.0");
                    kitReader = new MailKitReader(readerLogger, Consts);
                }                
                var res = kitReader.ReaderMails(MainAccount, Consts.ReadFolder, Consts.TrashFolder, SubjectStopWords, BodyStopWords, emailBlackList);
                return res;
            }
            catch(Exception ex)
            {
                readerLogger.Error($"ReaderMailsService {ex.Message}");
                return new List<IMailAnswer>();
            }            
        }
    }
}
