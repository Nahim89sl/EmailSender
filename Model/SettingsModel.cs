using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmailSender.Model
{
    public class SettingsModel
    {
       public SettingsModel()
        {
            Pauses = new Pauses();
            Akkaunt = new Akkaunt();
            FieldMapping = new FieldMapping();
            Answer = new Answer();
            ReaderModel = new ReaderModel();
            Telegram = new Telegram();

        }
        public string SenderListPath { get; set; }
        public string OurMailsListPath { get; set; }
        public string OurReceiversListPath { get; set; }
        public string Subject { get; set; }
        public string EmailText { get; set; }
        public string ReceiversListPath { get; set; }
   
        public Pauses Pauses { set; get; }
        public Akkaunt Akkaunt { get; set; }
        public FieldMapping FieldMapping { get; set; }
        public Telegram Telegram { get; set; }
        public Answer Answer { get; set; }
        public ReaderModel ReaderModel { get; set; }

        //read mails
        public int RederPauseInterval { get; set; }
        public string LastReadTime { get; set; }
        public string ReportFilePath { get; set; }


        public bool IsStartValidate { get; set; }
        public bool IsStartRead { get; set; }
    }


}
