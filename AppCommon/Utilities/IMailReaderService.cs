using AppCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCommon
{
    public interface IMailReaderService
    {
        IList<IMailAnswer> ReaderMails(IMailAkk akkaunt, string destFolderName, string trashFolderName, string stopWords, string emailBlackList);

        void ConnectToServer(IMailAkk account);
    }
}
