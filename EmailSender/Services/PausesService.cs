using EmailSender.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace EmailSender.Services
{
    public class PausesService
    {
        
        public int NextDopPause { get; set; }
        public int DopPauseTime { get; set; }
        public int NextChangeInterval { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        
        public ObservableCollection<PauseInterval> PauseIntervals { get; set; }


        public void MakePause()
        {
            //Check if we need change interval



            //Check if we need get dop pause           
            int pause = 2;
            if (NextDopPause < TimeUnixService.Timestamp())
            {
                pause = DopPauseTime;
            }

            //Make pause
            var rnd = new Random();
            pause += rnd.Next(Start, End);

            Thread.Sleep(pause * 1000);
        }


    }
}
