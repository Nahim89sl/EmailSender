using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Interfaces
{
    interface ISettingsService
    {
        SettingsModel Load();
        void Save(SettingsModel model);
        void Export();
        void Import(string settingsPath);
    }
}
