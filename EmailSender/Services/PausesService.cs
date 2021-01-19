using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Settings.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace EmailSender.Services
{
    public class PausesService
    {
        private ObservableCollection<PauseInterval> _pauseIntervals { get; set; }
        private Random _rnd;
        
        private ObservableCollection<PauseInterval> _intervals;
        private PauseInterval _currentINterval;
        private SenderSettings _settings;
        private ILogger _logger;


        private int nextChangeInterval;
        private int nextDopPause;



        public PausesService(SenderSettings settings, ObservableCollection<PauseInterval> intervals, ILogger logger)
        {
            _rnd = new Random();
            nextChangeInterval = TimeUnixService.Timestamp() + settings.ChangeIntTime;
            nextDopPause = TimeUnixService.Timestamp() + settings.DopPauseTime;
            _intervals = intervals;
            _settings = settings;
            _logger = logger;
        }
        
        public int GetPause()
        {
            //Check if we need change interval
            if(nextChangeInterval < TimeUnixService.Timestamp())
            {
                nextChangeInterval = TimeUnixService.Timestamp() + _settings.ChangeIntTime;
                var inter = _intervals.Where(a => a.Start < _settings.CurrentInterval.Start).OrderByDescending(st => st.Start).FirstOrDefault();
                if (inter != null)
                {
                    _logger?.InfoSender($"Change pause interval {_settings.CurrentInterval.Start} - {_settings.CurrentInterval.Finish}");
                    _settings.CurrentInterval = inter;
                }
            }
            int pause = _rnd.Next(_settings.CurrentInterval.Start, _settings.CurrentInterval.Finish);

            //Check if we need get dop pause           
            if (nextDopPause < TimeUnixService.Timestamp())
            {
                var dopPause = _rnd.Next(_settings.DopPauseInterval.Start, _settings.DopPauseInterval.Finish);
                _logger?.InfoSender($"Add dop pause {dopPause}");
                nextDopPause = TimeUnixService.Timestamp() + _settings.DopPauseTime;
                pause += dopPause;
            }

            //Pause
            _logger?.InfoSender($"=============== Pause {pause} ==================");
            return pause*1000;
        }


    }
}
