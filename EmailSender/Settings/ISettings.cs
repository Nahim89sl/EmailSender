using EmailSender.Models;
using EmailSender.Settings;

namespace EmailSender.Interfaces
{
    public interface ISettings
    {
        AppSettingsModel Load();
        void Save(AppSettingsModel appSettings);
    }
}