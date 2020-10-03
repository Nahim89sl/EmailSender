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
        private RangePause _stockRange;
        
        public Pauses()
        {            
            Ranges = new ObservableCollection<RangePause>();
            _dopPauseRange = new RangePause();
            ActiveRange = new RangePause();
            _dopPauseStart = 0;
            _mainRangeChange = 0;
            _stockRange = new RangePause() { Id = 0, Start = 300, End = 400 };
        }

        public ObservableCollection<RangePause> Ranges { get; set; }
        public int MainPauseChange { get; set; }
        public int DopPause { get; set; }
        public int SendToOurMail { get; set; }
        public int SendHiddenCopy { get; set; }

        private int _mainRangeChange; //time value for change Active Range
        private int _dopPauseStart;   //time value for use dop pause

        //It is for random generation pause at that time
        private RangePause _activeRange;
        public RangePause ActiveRange
        {
            get
            {
                return _activeRange;
            }
            set
            {
                Set(()=>ActiveRange, ref _activeRange, value);
            }

        }

        //the range of dop pause for random generation
        private RangePause _dopPauseRange;
        public RangePause DopPauseRange
        {
            get
            {
                return _dopPauseRange;
            }
            set
            {
                Set(() => DopPauseRange, ref _dopPauseRange, value);
            }

        }

        //File path for downloading ranges
        private string _rangePath;
        public string RangePath 
        {
            get 
            {
                return _rangePath;
            }
            set
            {
                Set(()=>RangePath,ref _rangePath, value);
            } 
                
        }
        

        public int GetPause()
        {
            ILogger logger = LogManager.GetCurrentClassLogger();
            var resPause = 0;
            var rnd = new Random();
            var nowTime = TUnix.Timestamp();
            try
            {
                if (_mainRangeChange == 0) { _mainRangeChange = nowTime + MainPauseChange; }  //at first time we do not need change interval
                if (_dopPauseStart == 0) { _dopPauseStart = nowTime + DopPause; }             //at first time we set new dop pause
                
                if (nowTime > _mainRangeChange)     //check if necessary change range
                {
                    var resRange = ChangeRange(Ranges,ActiveRange);
                    if (resRange != null)
                    {
                        ActiveRange = resRange;
                    }
                    _mainRangeChange = nowTime + MainPauseChange; //update timestamp for next time                
                }

                logger.Info("Active Range " + ActiveRange.Start.ToString() + "-" + ActiveRange.End.ToString());
                resPause = rnd.Next(ActiveRange.Start, ActiveRange.End);

                if (nowTime > _dopPauseStart)                                       //check if necessary take dop pause
                {
                    int dop = rnd.Next(DopPauseRange.Start, DopPauseRange.End);   
                    resPause += dop;                                                //just add dop pause to main
                    _dopPauseStart = nowTime + DopPause;                            //update timestamp for dop pause for next time
                    logger.Info("Add dop pause " + dop.ToString());
                }
                logger.Info("App pause " + resPause.ToString());
                return resPause;
            }
            catch (Exception ex)
            {
                logger.Error("GetPause "+ex.Message);
            }
            return resPause;
        }

        public  RangePause ChangeRange(ObservableCollection<RangePause> pauseRanges, RangePause activePauseRange)
        {                          
            if ((pauseRanges.Count>0)&&(activePauseRange!=null))
            {
                RangePause resRange = pauseRanges.Where(a => a.Id > activePauseRange.Id)?.FirstOrDefault() ?? pauseRanges.Last();
                return resRange;
            }
            return null;                   
        }

        public void CheckPauses()
        {
            if(ActiveRange == null)
            {
                ActiveRange = _stockRange;
            }
            if (DopPauseRange == null)
            {
                DopPauseRange = _stockRange;
            }
            if(Ranges == null)
            {
                Ranges = new ObservableCollection<RangePause>();
            }
        }
    }
}
