using EmailSender.Models;
using EmailSender.Settings.Models;
using ReaderMails.Interfaces;
using Stylet;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface ILoadReceivers
    {
        ObservableCollection<Receiver> Load();
        void OpenAndLoad();
        void AddListToReport(string filename, ObservableCollection<Answer> letters, Receiver receiver, string serverName);
        void AddToReport(string filename, IMailAnswer letter, Receiver receiver, string serverName);
        Task SaveChangesAsync(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping);
        void SaveChanges(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping);
        void SaveReceiver(Receiver receivers);
        BindableCollection<Receiver> LoadOurReceivers(string dbPath);
        bool CheckStatusOfOurReceiver(Receiver receiver, string dbPath);
    }
}