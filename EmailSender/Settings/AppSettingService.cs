using EmailSender.Interfaces;
using EmailSender.Models;
using EmailSender.Settings.Models;
using System.IO;
using System.Text.Json;

namespace EmailSender.Settings


{
    public class AppSettingService : ISettings
    {
        private const string FileSettings = "AppConfig.json";
        public AppSettingsModel Load()
        {
            AppSettingsModel model;

            try
            {
                string json = File.ReadAllText(FileSettings);
                model = JsonSerializer.Deserialize<AppSettingsModel>(json);
            }
            catch
            {
                model = new AppSettingsModel();
            }
            
            //inicialize values
            if (model.MainAccount == null){ model.MainAccount = new MainAccount();}
            if (model.NotificationAccount == null){model.NotificationAccount = new NotificationAccountSettings();}
            if (model.ReaderSettings == null) { model.ReaderSettings = new ReaderSettings(); }
            if (model.SenderSettings == null) { model.SenderSettings = new SenderSettings(); }
            if (model.LetterTemplate == null) { model.LetterTemplate = new Letter(); }
            return model;
        }

        public void Save(AppSettingsModel appSettings)
        {
            string json = JsonSerializer.Serialize<AppSettingsModel>(appSettings);
            File.WriteAllText(FileSettings, json);
        }
    }
}
