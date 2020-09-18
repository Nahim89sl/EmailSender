using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Model
{
    public class ReaderModel : ObservableObject
    {
        public ReaderModel()
        {
            AnswerLetter = new Letter();
        }

        private string isReadNow;
        public string IsReadNow
        {
            get { return isReadNow; }
            set {Set(()=>IsReadNow,ref isReadNow,value); }
        }
 
        private Letter answerLetter;     
        public Letter AnswerLetter
        {
            get { return answerLetter; }
            set { Set(()=>AnswerLetter,ref answerLetter,value); }
        }

        private string lastTimeRead;
        public string LastTimeRead
        {
            get { return lastTimeRead; }
            set { Set(()=>LastTimeRead, ref lastTimeRead,value); }
        }

        private string nextTimeRead;
        public string NextTimeRead
        {
            get { return nextTimeRead; }
            set { Set(() => NextTimeRead, ref nextTimeRead, value); }
        }

        private int nextReadInt;
        public int NextReadInt
        {
            get { return nextReadInt; }
            set
            {
                Set(() => NextReadInt, ref nextReadInt, value);
            }
        }

        private bool canSendAnswer;
        public bool CanSendAnswer
        {
            get { return canSendAnswer; }
            set { Set(()=>CanSendAnswer, ref canSendAnswer, value); }
        }

        private int readerPauseInterval;
        public int RederPauseInterval
        {
            get { return readerPauseInterval; }
            set
            {
                Set(() => RederPauseInterval, ref readerPauseInterval, value);
            }
        }
        private string reportFilePath1;
        public string ReportFilePath1
        {
            get
            {
                return reportFilePath1;
            }
            set
            {
                Set(() => ReportFilePath1, ref reportFilePath1, value);
            }
        }
        private string reportFilePath2;
        public string ReportFilePath2
        {
            get
            {
                return reportFilePath2;
            }
            set
            {
                Set(() => ReportFilePath2, ref reportFilePath2, value);
            }
        }

        private string stopWords;
        public string StopWords
        {
            get { return stopWords; }
            set { Set(()=>StopWords, ref stopWords, value);}
        }

        private string reportFolder1;
        public string ReportFolder1
        {
            get { return reportFolder1; }
            set {                
                Set(() => ReportFolder1, ref reportFolder1, value);             
            }
        }
        private string reportFolder2;
        public string ReportFolder2
        {
            get { return reportFolder2; }
            set
            {
                Set(() => ReportFolder2, ref reportFolder2, value);
            }
        }

    }
}
