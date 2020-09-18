using CommonServiceLocator;
using EmailSender.Interfaces;
using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EmailSender.Model
{
    public class Pauses : ObservableObject
    {
        public Pauses()
        {            
            Ranges = new ObservableCollection<RangePause>();
            dopPauseRange = new RangePause();
            ActiveRange = new RangePause();
            dopPauseStart = 0;
            mainRangeChange = 0;          
            stockRange = new RangePause() { Id = 0, Start = 30, End = 400 };
        }

        private RangePause stockRange;

        public ObservableCollection<RangePause> Ranges { get; set; }


        
        public int MainPauseChange { get; set; }
        public int MainPauseStart { get; set; }
        public int MainPauseEnd { get; set; }
        public int DopPause { get; set; }
        public int DopPauseStart { get; set; }
        public int DopPauseEnd { get; set; }
        public int SendToOurMail { get; set; }
        public int SendHiddenCopy { get; set; }



        
        public RangePause activeRange;
        public RangePause ActiveRange
        {
            get
            {
                return activeRange;
            }
            set
            {
                Set(()=>ActiveRange, ref activeRange, value);
            }

        }

        private RangePause dopPauseRange;
        public RangePause DopPauseRange
        {
            get
            {
                return dopPauseRange;
            }
            set
            {
                Set(() => DopPauseRange, ref dopPauseRange, value);
            }

        }


        private string rangePath;
        public string RangePath 
        {
            get 
            {
                return rangePath;
            }
            set
            {
                Set(()=>RangePath,ref rangePath, value);
            } 
                
        }

        
        private int mainRangeChange; //point of start work active range



        private int dopPauseStart;  //poin of start work dop pause

        public int GetPause()
        {
            ILogger logger = LogManager.GetCurrentClassLogger();
            try
            {
                Random rnd = new Random();
                int nowTime = TUnix.Timestamp();

                if (mainRangeChange == 0) { mainRangeChange = nowTime + MainPauseChange; } // at first time we do not need change interval
                if (dopPauseStart == 0) { dopPauseStart = nowTime + DopPause; } //at first time we set new dop pause
                if (nowTime > mainRangeChange)
                {
                    //change active range
                    ChangeRange();
                    mainRangeChange = nowTime + MainPauseChange; //update timestamp for next time                
                }

                logger.Info("Active Range " + ActiveRange.Start.ToString() + "-" + ActiveRange.End.ToString());
                int mainPause = rnd.Next(ActiveRange.Start, ActiveRange.End);

                if (nowTime > dopPauseStart)
                {
                    //add dop pause to main pause
                    int dop = rnd.Next(DopPauseRange.Start, DopPauseRange.End);
                    mainPause += dop;
                    dopPauseStart = nowTime + DopPause; //update timestamp for next time
                    logger.Info("Add dop pause " + dop.ToString());
                }
                logger.Info("App pause " + mainPause.ToString());
                return mainPause;
            }
            catch (Exception ex)
            {
                logger.Error("GetPause "+ex.Message);
            }
            return 0;
        }

        private void ChangeRange()
        {
            ILogger logger = LogManager.GetCurrentClassLogger();
            try
            {
                if (Ranges.Count>0)
                {
                    int id = ActiveRange.Id;
                    ActiveRange = Ranges.Where(a => a.Id > ActiveRange.Id)?.FirstOrDefault() ?? null;

                    if (id != ActiveRange.Id)
                    {
                        logger.Info("Change main pause Interval " + ActiveRange.Start.ToString() + "-" + ActiveRange.End.ToString());
                    }

                    if(ActiveRange == null)
                    {
                        ActiveRange = Ranges.Last();
                        logger.Error("Set Last range value " + ActiveRange.Start.ToString() + "-" + ActiveRange.End.ToString());
                    }
                }
                else
                {
                    logger.Error("ChangeRange pause ranges = 0");
                }
            }catch(Exception ex)
            {
                logger.Error("ChangeRange" + ex.Message);
                ActiveRange = stockRange;
                logger.Error("Set Last range value "+ActiveRange.Start.ToString()+"-"+ActiveRange.End.ToString());
            }            
        }

        public void CheckPauses()
        {
            if(ActiveRange == null)
            {
                ActiveRange = new RangePause() { Id = 0, Start = 10, End = 11 };
            }
            if (DopPauseRange == null)
            {
                DopPauseRange = new RangePause() { Id = 0, Start = 10, End = 11 };
            }
            if(Ranges == null)
            {
                Ranges = new ObservableCollection<RangePause>();
            }
        }
    }
}
