﻿using System;
using System.Collections.Generic;
using AppCommon.Interfaces;
using EmailSender.Interfaces;
using NLog;
using ReaderMails;
using StyletIoC;

namespace EmailSender.Services
{
    class ReaderMailsService : IReaderMails
    {
        
        [Inject] public  IMailAkk MainAccount;
        [Inject] private Logger.ILogger logger;
        [Inject] private  IConsts Consts;

        public IList<IMailAnswer> ReadMails(string stopWords, string emailBlackList)
        {
            try
            {
                var service = new MailKitReader(LogManager.GetLogger("Reader"));
                var res = service.ReaderMails(MainAccount, Consts.ReadFolder, Consts.TrashFolder, stopWords, emailBlackList);
                return res;
            }
            catch(Exception ex)
            {
                logger.ErrorReader($"ReaderMailsService {ex.Message}");
                return new List<IMailAnswer>();
            }            
        }
    }
}
