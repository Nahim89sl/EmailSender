using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Model
{
    public class RangePause : ObservableObject
    {
        int start;
        int end;

        public int Start
        {
            get
            {
                return start;
            }
            set
            {
                Set(() => Start, ref start, value);
            }
        }
        public int End
        {
            get
            {
                return end;
            }
            set
            {
                Set(() => End, ref end, value);
            }
        }

        public int Id { get; set; }
    }
}
