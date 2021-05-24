using AppCommon.Interfaces;
using System.Collections.Generic;

namespace EmailSender.Interfaces
{
    public interface IReaderMails
    {
        IList<IMailAnswer> ReadMails(string stopWords, string emailBlackList);
    }
}