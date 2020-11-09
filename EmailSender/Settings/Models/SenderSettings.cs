using System;
using System.Collections.Generic;
using System.Text;
using EmailSender.Models;

namespace EmailSender.Settings.Models
{
    public class SenderSettings
    {
        public SenderSettings()
        {
            CurrentInterval = new PauseInterval();
            DopPauseInterval = new PauseInterval();
        }
        
        //sender params
        public int ChangeIntTime { get; set; }              //change main pause's interval
        public int DopPauseTime { get; set; }               //dop pause interval
        public PauseInterval CurrentInterval { get; set; }  //current interval of pauses+
        public PauseInterval DopPauseInterval { get; set; } //interval of dop pause
        public string IntervalsFilePath { get; set; }       //file path for intervals in file
        
        //send to our receivers
        public int SendOurMail { get; set; }
        public int SendToHidden { get; set; }
        public string OurMailsFilePath { get; set; }
        public int MailListRounds { get; set; }
        public bool IsAutoStart { get; set; }
    }
}
