using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface IOurMails
    {
        void LoadAsync(string path, ObservableCollection<Receiver> receivers);
    }
}
