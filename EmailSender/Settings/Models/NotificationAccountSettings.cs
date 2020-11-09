using Stylet;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Windows;

namespace EmailSender.Settings.Models
{
    public class NotificationAccountSettings
    {
        public string ApiKey { get; set; }
        public string Receiver { get; set; }
        public string Status { get; set; }
        public string Login { get;  set; }
        public string Pass { get;  set; }
        public bool ServerErrorNotify { get; set; }
        public bool AnswerGetNotify { get; set; }
        public bool FinishSendNotify { get; set; }
    }
}
