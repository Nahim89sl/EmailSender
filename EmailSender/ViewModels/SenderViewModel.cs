using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Services;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace EmailSender.ViewModels
{
    public class SenderViewModel : PropertyChangedBase
    {
        private ILogger _logger;
        private ISender _sender;
        private SenderSettings _settings;
        private IDialogService _dialog;
        private BindableCollection<Receiver> _receivers;
        private Letter _templateLetter;
        private TextRundomizer textConv;
        private IWindowManager _windMng;
        private INotification _notification;
        private MainAccount _acc;
        private ILoadReceivers _saver;
        private FieldMappingSettingsModel _fieldMapping;
        private const string StatusNotSend = "no";
        private const string StatusSended = "SENDED";
        private const string StatusServerOk = "ok";
        private int ServerErrorCount;
        


        public SenderViewModel(IContainer ioc)
        {
            _sender = ioc.Get<ISender>();
            _logger = ioc.Get<ILogger>();
            _settings = ioc.Get<AppSettingsModel>().SenderSettings;
            _dialog = ioc.Get<IDialogService>();
            _receivers = ioc.Get<BindableCollection<Receiver>>();
            _templateLetter = ioc.Get<AppSettingsModel>().LetterTemplate;
            _windMng = ioc.Get<IWindowManager>();
            _notification = ioc.Get<INotification>();
            _acc = ioc.Get<MainAccount>();
            _saver = ioc.Get<ILoadReceivers>();
            _fieldMapping = ioc.Get<AppSettingsModel>().FielMappingSettings;            
            textConv = new TextRundomizer();
            LoadIntervals();
            LoadOurReceivers();           

            if (IsAutoStart)
            {
                StartSenderCommand();
            }
        }

        private int _mailListRounds;  //period for changing interval of pause
        public int MailListRounds
        {
            get
            {
                _mailListRounds = _settings.MailListRounds;
                return _mailListRounds;
            }
            set
            {
                SetAndNotify(ref this._mailListRounds, value);
                _settings.MailListRounds = value;
            }
        }

        int countToHidden;
        int countToOurMails;
        int countMailListRounds;
        int countLettersToSave;


        int nextDopPause;
        int nextChangeInterval;


        //pauses
        private int _changeIntTime;  //period for changing interval of pause
        public int ChangeIntTime
        {
            get
            {
                _changeIntTime = _settings.ChangeIntTime;
                return _changeIntTime;
            }
            set
            {
                SetAndNotify(ref this._changeIntTime, value);
                _settings.ChangeIntTime = value;
            }
        }

        private PauseInterval _currentInterval;
        public PauseInterval CurrentInterval
        {
            get
            {
                _currentInterval = _settings.CurrentInterval;
                return _currentInterval;
            }
            set
            {
                SetAndNotify(ref this._currentInterval, value);
                _settings.CurrentInterval = value;
            }
        }

        private string _intervalsFilePath;
        public string IntervalsFilePath
        {
            get
            {
                _intervalsFilePath = _settings.IntervalsFilePath;
                return _intervalsFilePath;
            }
            set
            {
                SetAndNotify(ref this._intervalsFilePath, value);
                _settings.IntervalsFilePath = value;
            }
        }

        //dop pauses
        private int _dopPauseTime;  // period for using dop pause
        public int DopPauseTime
        {
            get
            {
                _dopPauseTime = _settings.DopPauseTime;
                return _dopPauseTime;
            }
            set
            {
                SetAndNotify(ref this._dopPauseTime, value);
                _settings.DopPauseTime = value;
            }
        }

        private PauseInterval _dopPauseInterval;  // interval of dop pause
        public PauseInterval DopPauseInterval
        {
            get
            {
                _dopPauseInterval = _settings.DopPauseInterval;
                return _dopPauseInterval;
            }
            set
            {
                SetAndNotify(ref this._dopPauseInterval, value);
                _settings.DopPauseInterval = value;
            }
        }


        //our mails 
        private string _ourMailsFilePath;
        public string OurMailsFilePath
        {
            get
            {
                _ourMailsFilePath = _settings.OurMailsFilePath;
                return _ourMailsFilePath;
            }
            set
            {
                SetAndNotify(ref this._ourMailsFilePath, value);
                _settings.OurMailsFilePath = value;
            }
        }

        private int _sendToHidden;
        public int SendToHidden
        {
            get
            {
                _sendToHidden = _settings.SendToHidden;
                return _sendToHidden;
            }
            set
            {
                SetAndNotify(ref this._sendToHidden, value);
                _settings.SendToHidden = value;
            }
        }

        private int _receiverId; //this show to progressbar receiver's id
        public int ReceiverId
        {
            get
            {
                return _receiverId;
            }
            set
            {
                SetAndNotify(ref this._receiverId, value);
            }
        }

        private int _totalReceivers; //this show to progressbar total count of receivers
        public int TotalReceivers
        {
            get
            {
                return _totalReceivers;
            }
            set
            {
                SetAndNotify(ref this._totalReceivers, value);
            }
        }


        private int _sendOurMail;
        public int SendOurMail
        {
            get
            {
                _sendOurMail = _settings.SendOurMail;
                return _sendOurMail;
            }
            set
            {
                SetAndNotify(ref this._sendOurMail, value);
                _settings.SendOurMail = value;
            }
        }

        
        public ObservableCollection<PauseInterval> PauseIntervals { get; set; }
        public ObservableCollection<Receiver> OurReceivers { get; set; }


        private bool _isAutoStart;
        public bool IsAutoStart
        {
            get
            {
                _isAutoStart = _settings.IsAutoStart;
                return _isAutoStart;
            }
            set
            {
                SetAndNotify(ref this._isAutoStart, value);
                _settings.IsAutoStart = value;
            }
        }

        private bool isSenderRun;




        /// <summary>
        /// COMMANDS
        /// </summary>

        public void LoadIntervalsCommand()
        {
            if (_dialog.OpenFileDialog() == true)
            {
                IntervalsFilePath = _dialog.FilePath;
                LoadIntervals();
            }
        }

        public void LoadOurReceiversCommand()
        {
            if (_dialog.OpenFileDialog() == true)
            {
                OurMailsFilePath = _dialog.FilePath;
                LoadOurReceivers();
            }
        }

        
        //Start sender
        public void StartSenderCommand()
        {
            _logger.InfoSender("Start sending messages");
            SenderService();
        }
        public bool CanStartSenderCommand
        {
            get { return !isSenderRun; }
        }

        //Stop sender
        public void StopSenderCommand()
        {
            _logger.InfoSender("STOP sending messages");
            isSenderRun = false;
            NotifyOfPropertyChange(nameof(this.CanStopSenderCommand));
            _saver.SaveChangesAsync(_receivers, _fieldMapping);
        }
        public bool CanStopSenderCommand
        {
            get { return isSenderRun; }
        }


        private void LoadIntervals()
        {
            if (File.Exists(IntervalsFilePath))
            {
                var lines = File.ReadAllLines(IntervalsFilePath);
                PauseIntervals = new ObservableCollection<PauseInterval>();
                foreach (var line in lines)
                {
                    int startInt = 0;
                    int finishInt = 0;
                    var arrLine = line.Split('-');
                    if (arrLine.Length > 1)
                    {
                        int.TryParse(arrLine[0], out startInt);
                        int.TryParse(arrLine[1], out finishInt);
                    }
                    if (((startInt > 0) || (finishInt > 0)) && (startInt < finishInt))
                    {
                        PauseIntervals.Add(new PauseInterval(){Start = startInt, Finish = finishInt});
                    }
                    NotifyOfPropertyChange(nameof(this.PauseIntervals));
                }
            }
        }

        private void LoadOurReceivers()
        {
            if (File.Exists(OurMailsFilePath))
            {
                var lines = File.ReadAllLines(OurMailsFilePath);
                OurReceivers = new ObservableCollection<Receiver>();
                foreach (var line in lines)
                {
                    OurReceivers.Add(new Receiver()
                    {
                        Email = line,
                        Count = 0
                    });
                }
                NotifyOfPropertyChange(nameof(this.OurReceivers));
            }            
        }

        private  void SenderService()   //send all messages
        {
            ServerErrorCount = 0;
            isSenderRun = true;
            TotalReceivers = _receivers.Count;
            NotifyOfPropertyChange(nameof(this.TotalReceivers));
            NotifyOfPropertyChange(nameof(this.CanStartSenderCommand));
            NotifyOfPropertyChange(nameof(this.CanStopSenderCommand));
            Task.Run(async ()=> {
                while (isSenderRun)
                {
                    //take redy Receiver for send message
                    var receiver = _receivers.Where(a => a.StatusSend == StatusNotSend).FirstOrDefault();
                    if (receiver != null)
                    {
                        //change prop ReceiverId for progress bar visibility
                        Execute.OnUIThread(()=> {
                            ReceiverId = receiver.IdReceiver;
                            NotifyOfPropertyChange(nameof(ReceiverId));
                        });
                        
                        //check if we need to send to our mails
                        if(countToOurMails > SendOurMail)
                        {                            
                            var ourReceiver = GetOurMail(receiver);
                            _logger.InfoSender($"Try send to our mail {ourReceiver.Email}");
                            textConv.LetterRandomizeText(ourReceiver, _templateLetter);
                            await _sender.SendEmail(ourReceiver, new Receiver(), ourReceiver.Letter);
                            ourReceiver.Count++;
                            countToOurMails = 0;
                            CheckStatuses(ourReceiver);
                            MakePause();
                        }

                        _logger.InfoSender($"Try send to {receiver.Email}");
                        textConv.LetterRandomizeText(receiver, _templateLetter);
                        await _sender.SendEmail(receiver, GetHidden(), receiver.Letter);
                        CheckStatuses(receiver);
                        MakePause();
                        
                        countToHidden++;
                        countToOurMails++;

                        //check if we need to save changes to receivers list
                        countLettersToSave++;
                        if (countLettersToSave > 100)
                        {
                            _saver.SaveChangesAsync( _receivers, _fieldMapping);                            
                            countLettersToSave = 0;
                        }
                    }
                    else
                    {
                        if(countMailListRounds < MailListRounds)
                        {
                            countMailListRounds++;
                            UpdateMailList();
                        }
                        else
                        {
                            isSenderRun = false;
                            break;
                        }                        
                    }                    
                }
                _logger.InfoSender("Sending finished");
                _notification.FinishSendMessage($"{_acc.Server} finished sending mails");
                Execute.OnUIThread(()=> {
                    _windMng.ShowMessageBox("Sending finished");
                    isSenderRun = false;
                    NotifyOfPropertyChange(nameof(this.CanStartSenderCommand));
                    NotifyOfPropertyChange(nameof(this.CanStopSenderCommand));
                });
            });
        }

        private void MakePause()
        {
            try
            {
                Random rnd = new Random();
                //set values at first start
                if (nextChangeInterval == 0)
                {
                    nextChangeInterval = TimeUnixService.Timestamp() + ChangeIntTime;
                }
                if (nextDopPause == 0)
                {
                    nextDopPause = TimeUnixService.Timestamp() + DopPauseTime;
                }

                //change interval
                if (nextChangeInterval < TimeUnixService.Timestamp())
                {
                    var inter = PauseIntervals.Where(a => a.Start > CurrentInterval.Start).OrderByDescending(st => st.Start).FirstOrDefault();
                    if (inter != null)
                    {
                        CurrentInterval = inter;
                    }
                    _logger.InfoSender($"Change pause interval {CurrentInterval.Start} - {CurrentInterval.Finish}");
                    nextChangeInterval = TimeUnixService.Timestamp() + ChangeIntTime;
                }

                //set pause value
                int pause = rnd.Next(CurrentInterval.Start, CurrentInterval.Finish);

                //add dop pause to main pause
                if (nextDopPause < TimeUnixService.Timestamp())
                {
                    var tmp = rnd.Next(DopPauseInterval.Start, DopPauseInterval.Finish);
                    pause += tmp;
                    nextDopPause = TimeUnixService.Timestamp() + DopPauseTime;
                    _logger.InfoSender($"Add dop pause {tmp}");
                }
                _logger.InfoSender($"Set pause {pause} sec");

                //make pause
                Thread.Sleep(pause * 1000);
            }
            catch(Exception ex)
            {
                _logger.ErrorSender($"Block MakePause error {ex.Message}");
                throw ex;
            }
            
        }

        private Receiver GetOurMail(Receiver receiver)
        {
            var ourReceiver = OurReceivers.OrderBy(a => a.Count).FirstOrDefault();
            if (ourReceiver != null)
            {
                ourReceiver.Bcc = receiver.Bcc;
                ourReceiver.CC = receiver.CC;
                ourReceiver.CompanyName = receiver.CompanyName;
                ourReceiver.FieldAddress = receiver.FieldAddress;
                ourReceiver.FieldContractAmount = receiver.FieldContractAmount;
                ourReceiver.FieldDate1 = receiver.FieldDate1;
                ourReceiver.FieldDate2 = receiver.FieldDate2;
                ourReceiver.FieldDate3 = receiver.FieldDate3;
                ourReceiver.FieldInn = receiver.FieldInn;
                ourReceiver.FieldOkvd = receiver.FieldOkvd;
                ourReceiver.FieldPhone = receiver.FieldPhone;
                ourReceiver.FieldRecord1 = receiver.FieldRecord1;
                ourReceiver.FieldRecord2 = receiver.FieldRecord2;
                ourReceiver.FieldRecord3 = receiver.FieldRecord3;
                ourReceiver.Letter = receiver.Letter;
                ourReceiver.PersonName = receiver.PersonName;
                ourReceiver.Time = receiver.Time;
            }
            return ourReceiver;
        }

        private Receiver GetHidden()
        {
            if(countToHidden < SendToHidden)
            {
                return new Receiver();
            }            
            var ourReceiver = OurReceivers.OrderBy(a => a.Count).FirstOrDefault();
            _logger.InfoSender($"Hidden send to {ourReceiver.Email}");
            countToHidden = 0;
            return ourReceiver;
        }

        private void UpdateMailList()
        {
            _logger.InfoSender("Restart receiver list");
            Execute.OnUIThread(()=> {
                foreach(var reseiver in _receivers)
                {
                    if(reseiver.StatusSend == StatusSended) { reseiver.StatusSend = StatusNotSend; }
                }
                NotifyOfPropertyChange(nameof(_receivers));
            });
            _saver.SaveChangesAsync(_receivers, _fieldMapping);
        }


        private void CheckStatuses(Receiver receiver)
        {
            if((_acc.ServerStatus != StatusServerOk) || (_acc.AccountStatus != StatusServerOk) || (receiver.StatusSend != StatusSended))
            {
                if (ServerErrorCount > 10)
                {
                    Execute.OnUIThread(() => {                        
                        isSenderRun = false;
                        _windMng.ShowMessageBox("Ошибка работы сервера");
                        NotifyOfPropertyChange(nameof(this.CanStartSenderCommand));
                        NotifyOfPropertyChange(nameof(this.CanStopSenderCommand));
                        _notification.ServerErrorMessage($"Сервер {_acc.Server} выдал ошибку {_acc.ServerStatus} \n Работа рассыльщика остановлена");
                        _logger.ErrorSender($"Server error {_acc.ServerStatus}");
                    });
                }
                ServerErrorCount++;
                Thread.Sleep(600);
                _logger.ErrorSender($"Server error {_acc.ServerStatus}");
            }
            else
            {
                _logger.InfoSender($"{receiver.IdReceiver.ToString()} Mail to {receiver.Email} sended");               
            }
        }
    }
}
