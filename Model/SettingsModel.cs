using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmailSender.Model
{
    public class SettingsModel
    {

        public string OurMailsListPath { get; set; }
        public string OurReceiversListPath { get; set; }
        public string ReceiversListPath { get; set; }
   
        public Pauses Pauses { set; get; }
        public Akkaunt Akkaunt { get; set; }
        public FieldMapping FieldMapping { get; set; }
        public Telegram Telegram { get; set; }
        public Answer Answer { get; set; }
        public ReaderModel ReaderModel { get; set; }
        public Letter LetterTemplate { get; set; }


        public bool IsStartValidate { get; set; }
        public bool IsStartRead { get; set; }
        public bool IsStartSend { get; set; }

    }


}
