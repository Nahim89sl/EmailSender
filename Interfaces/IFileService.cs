using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface IFileService
    {
        ObservableCollection<Receiver> Open(string filename, FieldMapping FieldMapping);
        void Save(string filename, ObservableCollection<Receiver> receivers, FieldMapping FieldMapping);
        void SaveChanges(string filename, Receiver receiver, FieldMapping FieldMapping);
        void AddToReport(string filename, ObservableCollection<Letter> letters);
    }
}