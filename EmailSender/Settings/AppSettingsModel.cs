using EmailSender.Models;
using EmailSender.Settings.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Settings
{
    public class AppSettingsModel
    {
        public NotificationAccountSettings NotificationAccount { get; set; }
        public MainAccount MainAccount { get; set; }
        public ReaderSettings ReaderSettings { get; set; }
        public FieldMappingSettingsModel FielMappingSettings { get; set; }
        public SenderSettings SenderSettings { get; set; }
        public Letter LetterTemplate { get; set; }
    }
}
