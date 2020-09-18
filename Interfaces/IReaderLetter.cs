using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface IReaderLetter
    {
        ObservableCollection<Letter> ReadMails(Akkaunt akkaunt, string stopWords);
    }
}
