using AppCommon.Interfaces;
using System.Collections.Generic;

namespace EmailSender.Interfaces
{
    public interface IReaderMails
    {
        IList<IMailAnswer> ReadMails(string SubjectStopWords, string BodyStopWords, string emailBlackList);
    }
}