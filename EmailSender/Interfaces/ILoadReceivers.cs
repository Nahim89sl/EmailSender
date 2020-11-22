using EmailSender.Models;
using EmailSender.Settings.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface ILoadReceivers
    {
        ObservableCollection<Receiver> Load();
        void OpenAndLoad();
        void AddListToReport(string filename, ObservableCollection<Answer> letters, Receiver receiver);
        void AddToReport(string filename, Answer letter, Receiver receiver);
        Task SaveChangesAsync(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping);
        void SaveChanges(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping);
        void SaveReceiver(Receiver receivers);

    }
}