using AppCommon.Interfaces;
using EmailSender.Interfaces;
using NLog;
using ReaderMails;
using ReaderMails.Interfaces;
using StyletIoC;
using System;

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

        public void ReadMails(EmailFiltrator filtrator)
        {
                       
            try
            {
                if(kitReader == null)
                {
                    readerLogger.Info($"Reader created 1.0.1");
                    kitReader = new MailKitReader(readerLogger, Consts.ReadFolder, Consts.TrashFolder);
                }                
                kitReader.ReadMails(MainAccount, filtrator);
            }
            catch(Exception ex)
            {
                readerLogger.Error($"ReaderMailsService {ex.Message}");
            }            
        }
    }
}
