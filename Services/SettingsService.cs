using EmailSender.Interfaces;
using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EmailSender.Services
{
    public class SettingsService : ISettingsService
    {
        public void Export()
        {
            throw new NotImplementedException();
        }


        public void Import(string settingsPath)
        {
            throw new NotImplementedException();
        }

        public SettingsModel Load()
        {
            SettingsModel model;
            
            try
            {
                string json = File.ReadAllText("config.json");
                model  = JsonSerializer.Deserialize<SettingsModel>(json);
            }
            catch
            {
                model = new SettingsModel();
            }
            
            return model;
        }

        public void Save(SettingsModel model)
        {
             string json = JsonSerializer.Serialize<SettingsModel>(model);
             File.WriteAllText("config.json", json);
        }

       
    }
}
