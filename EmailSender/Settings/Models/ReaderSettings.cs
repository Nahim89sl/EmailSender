using EmailSender.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Settings.Models
{
    public class ReaderSettings
    {
        public int ReadInterval { get; set; }
        
        public string ReportFolder_1  { get; set; }
        public string ReportFolder_2 { get; set; }
        public Letter AnswerLetter { get; set; }        
        public string StopWords { get; set; }
        public string NotExistList_1 { get; set; }
        public string NotExistList_2 { get; set; }
        public string WordsSpamMail { get; set; }
        public string EmailBlackList { get; set; }
        public string AnswerTitleList { get; set; }
        public string AnswerBodyList { get; set; }

        public bool IsAutoStart { get; set; }
        public bool IsAnswer { get; set; }
    }
}
