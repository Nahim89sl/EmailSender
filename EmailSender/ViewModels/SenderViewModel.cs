using AppCommon.Interfaces;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Services;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using Stylet;
using StyletIoC;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EmailSender.ViewModels
{
    public class SenderViewModel : PropertyChangedBase
    {

        #region  private fields
        private IConsts _consts;

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
        private int ServerErrorCount;


        private int _mailListRounds;  //period for changing interval of pause
        int countToHidden;
        int countToOurMails;
        int countMailListRounds;

        private int _changeIntTime;  //period for changing interval of pause
        private PauseInterval _currentInterval;  //this interval now use sender
        private string _intervalsFilePath;      //this if file path for txt-file with the pauses intervals

        private PauseInterval _dopPauseInterval;  // interval of dop pause
        private int _dopPauseTime;  // period for using dop pause
        private string _ourMailsFilePath;
        private int _sendToHidden;
        private int _receiverId; //this show to progressbar receiver's id
        private int _totalReceivers; //this show to progressbar total count of receivers
        private int _sendOurMail;
        private bool _isAutoStart;
        private bool isSenderRun;
        private ISettings settingsService;
        private AppSettingsModel _globalSettings;

        private IOurReceiversWorker _ourReceiversWorker;
        PausesService Pause;


        #endregion

        #region Constructor

        public SenderViewModel(IContainer ioc)
        {
            _sender = ioc.Get<ISender>();
            _logger = ioc.Get<ILogger>();
            _globalSettings = ioc.Get<AppSettingsModel>();
            _settings = _globalSettings.SenderSettings;
            _dialog = ioc.Get<IDialogService>();
            _receivers = ioc.Get<BindableCollection<Receiver>>();
            _templateLetter = ioc.Get<AppSettingsModel>().LetterTemplate;
            _windMng = ioc.Get<IWindowManager>();
            _notification = ioc.Get<INotification>();
            _acc = _globalSettings.MainAccount;
            _saver = ioc.Get<ILoadReceivers>();
            _fieldMapping = ioc.Get<AppSettingsModel>().FielMappingSettings;
            _ourReceiversWorker = ioc.Get<IOurReceiversWorker>();
            settingsService = ioc.Get<ISettings>();
            textConv = new TextRundomizer();
            LoadIntervals();
            LoadOurReceivers();
            _consts = ioc.Get<IConsts>();

            Pause = new PausesService(_settings, PauseIntervals, _logger);

            if (IsAutoStart)
            {
                StartSenderCommand();
            }
            
        }
        
        #endregion
        
        #region  Public Props

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
        public BindableCollection<Receiver> OurReceivers { get; set; }



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

        #endregion

        #region Commands

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

        public void SaveSettingsCommand()
        {
            settingsService.Save(_globalSettings);
            MessageBox.Show("Настройки сохранены");
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
        }
        public bool CanStopSenderCommand
        {
            get { return isSenderRun; }
        }

        #endregion

        #region Private Methods

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
            if (OurReceivers == null)
            {
                OurReceivers = new BindableCollection<Receiver>();
            }            
            OurReceivers?.Clear();
            if (File.Exists(OurMailsFilePath))
            {         
                _ourReceiversWorker.Load(OurReceivers, OurMailsFilePath);
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
                    var receiver = _receivers.Where(a => ((a.StatusSend == _consts.ReceiverStatusNotSend)||(a.StatusSend == _consts.ReceiverStatusWariant))).FirstOrDefault();
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
                            CheckStatuses(ourReceiver);
                            
                            ourReceiver.Count++;
                            countToOurMails = 0;
                            Thread.Sleep(Pause.GetPause());
                        }

                        _logger.InfoSender($"Try send to {receiver.Email}");
                        textConv.LetterRandomizeText(receiver, _templateLetter);

                        await _sender.SendEmail(receiver, GetHidden(), receiver.Letter);                     
                        CheckStatuses(receiver);

                        Thread.Sleep(Pause.GetPause());
                        countToHidden++;
                        countToOurMails++;

                        //add status of receiver to database
                        receiver.StatusSend = _consts.ReceiverStatusSended;
                        _saver.SaveReceiver(receiver);
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


        private Receiver GetOurMail(Receiver receiver)
        {
            var ourReceiver = _ourReceiversWorker.GetReadyReceiverForSend(OurReceivers, receiver, OurMailsFilePath);
            if (ourReceiver == null)
            {
                _logger.ErrorSender($"Не можем получить ативный аккаунт из базы Имитатора для отправки");
            }
            _logger.InfoSender($"Отправили на свой ящик {ourReceiver?.Email}");
            countToHidden = 0;
            return ourReceiver;
        }

        private Receiver GetHidden()
        {
            if(countToHidden < SendToHidden)
            {
                return new Receiver();
            }
            var ourReceiver = _ourReceiversWorker.GetReadyReceiverForSend(OurReceivers,new Receiver(), OurMailsFilePath);
            if (ourReceiver == null)
            {
                _logger.ErrorSender($"Не можем получить ативный аккаунт из базы Имитатора для отправки скрытой копии");
            }
            _logger.InfoSender($"Взяли ящик для скрытой копии {ourReceiver?.Email}");
            countToHidden = 0;
            return ourReceiver;
        }

        private void UpdateMailList()
        {
            _logger.InfoSender("Restart receiver list");
            Execute.OnUIThread(()=> {
                foreach(var reseiver in _receivers)
                {
                    if(reseiver.StatusSend == _consts.ReceiverStatusSended) 
                    { 
                        reseiver.StatusSend = _consts.ReceiverStatusNotSend; 
                    }
                }
                NotifyOfPropertyChange(nameof(_receivers));
            });
            _saver.SaveChangesAsync(_receivers, _fieldMapping);
        }

        private void CheckStatuses(Receiver receiver)
        {
            if((_acc.ServerStatus != _consts.ServerStatusOk) || (_acc.AccountStatus != _consts.AkkStatusOk) || (receiver.StatusSend != _consts.ReceiverStatusSended))
            {
                if((ServerErrorCount > 100)&&(ServerErrorCount % 10 == 0))
                {
                    _notification.ServerErrorMessage($"Сервер {_acc.Server} выдал ошибку: {_acc.ServerStatus}\n устраните сбой работы сервера");
                }

                if (_acc.AccountStatus != _consts.AkkStatusOk)
                {
                    _notification.AccountErrorMessage($"Статус аккаунта {_acc.Login} изменился: {_acc.AccountStatus}");
                }

                if (ServerErrorCount > 100000)
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

        #endregion
    }
}
