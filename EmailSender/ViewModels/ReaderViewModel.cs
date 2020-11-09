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

namespace EmailSender.ViewModels
{
    public class ReaderViewModel : PropertyChangedBase
    {
        private ILogger _logger;
        private ISender _sender;
        private ReaderSettings _settings;
        private MainAccount _account;
        private INotification _notification;
        private IDialogService _dialog;
        private IReaderMails _reader;
        private ILoadReceivers _reporter;
        private BindableCollection<Receiver> _receivers;





        public ReaderViewModel(IContainer ioc)
        {
            _sender = ioc.Get<ISender>();
            _logger = ioc.Get<ILogger>();
            _settings = ioc.Get<AppSettingsModel>().ReaderSettings;
            _account = ioc.Get<AppSettingsModel>().MainAccount;
            _notification = ioc.Get<INotification>();
            _dialog = ioc.Get<IDialogService>();
            _reader = ioc.Get<IReaderMails>();
            _reporter = ioc.Get<ILoadReceivers>();
            _receivers = ioc.Get<BindableCollection<Receiver>>();

            //autostart reader service
            if (IsAutoStart)
            {
                ReadService();
            }
        }

        //property of answering function to receivers
        private bool _isAnswer;
        public bool IsAnswer
        {
            get
            {
                _isAnswer = _settings.IsAnswer;
                return _isAnswer;
            }
            set
            {
                SetAndNotify(ref this._isAnswer, value);
                _settings.IsAnswer = value;
            }
        }

        //propperty of start reading service
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

        private int _readInterval;
        public int ReadInterval
        {
            get
            {
                _readInterval = _settings.ReadInterval;
                return _readInterval;
            }
            set
            {
                SetAndNotify(ref this._readInterval, value);
                _settings.ReadInterval = value;
            }
        }

        private string _textAnswer;
        public string TextLetter
        {
            get
            {
                _textAnswer = _settings.AnswerLetter.Text;
                return _textAnswer;
            }
            set
            {
                SetAndNotify(ref this._textAnswer, value);
                _settings.AnswerLetter.Text = value;
            }
        }

        private string _subjectLetter;
        public string SubjectLetter
        {
            get
            {
                _subjectLetter = _settings.AnswerLetter.Subject;
                return _subjectLetter;
            }
            set
            {
                SetAndNotify(ref this._subjectLetter, value);
                _settings.AnswerLetter.Subject = value;
            }
        }

        private string _stopWords;
        public string StopWords
        {
            get
            {
                _stopWords = _settings.StopWords;
                return _stopWords;
            }
            set
            {
                SetAndNotify(ref this._stopWords, value);
                _settings.StopWords = value;
            }
        }

        private string _reportFolder_1;
        public string ReportFolder_1
        {
            get
            {
                _reportFolder_1 = _settings.ReportFolder_1;
                return _reportFolder_1;
            }
            set
            {
                SetAndNotify(ref this._reportFolder_1, value);
                _settings.ReportFolder_1 = value;
            }
        }

        private string _reportFolder_2;
        public string ReportFolder_2
        {
            get
            {
                _reportFolder_2 = _settings.ReportFolder_2;
                return _reportFolder_2;
            }
            set
            {
                SetAndNotify(ref this._reportFolder_2, value);
                _settings.ReportFolder_2 = value;
            }
        }

        

        private string _accountState;
        public string AccountState
        {
            get
            {
                _accountState = _account.AccountStatus;
                return _accountState;
            }
            set
            {
                SetAndNotify(ref this._accountState, value);
                _account.AccountStatus = value;
            }
        }

        private string _serverState;
        public string ServerState
        {
            get
            {
                _serverState = _account.ServerStatus;
                return _serverState;
            }
            set
            {
                SetAndNotify(ref this._serverState, value);
                _account.ServerStatus = value;
            }
        }

        //times
        private int nextTimeReadInt;
        private string _nextTimeRead;
        public string NextTimeRead
        {
            get
            {
                return _nextTimeRead;
            }
            set
            {
                SetAndNotify(ref this._nextTimeRead, value);
            }
        }

        private string _lastTimeRead;
        public string LastTimeRead
        {
            get
            {
                return _lastTimeRead;
            }
            set
            {
                SetAndNotify(ref this._lastTimeRead, value);
            }
        }
        ///


        public void SetFolderCommand1()
        {
            ReportFolder_1 = _dialog.OpenFolder();
            NotifyOfPropertyChange(nameof(this.ReportFolder_1));

        }
        public void SetFolderCommand2()
        {
            ReportFolder_2 = _dialog.OpenFolder(); 
            NotifyOfPropertyChange(nameof(this.ReportFolder_2));
        }
        
        public void StartReadServiceCommand()
        {
            isReadServiceWork = true;
            NotifyOfPropertyChange(nameof(this.CanStartReadServiceCommand));
            NotifyOfPropertyChange(nameof(this.CanStopReadServiceCommand));
            ReadService();
        }
        public bool CanStartReadServiceCommand
        {
            get { return !isReadServiceWork; }
        }


        public void StopReadServiceCommand()
        {
            isReadServiceWork = false;
        }
        public bool CanStopReadServiceCommand
        {
            get { return isReadServiceWork; }
        }

        private bool isReadNow;
        public void ReadMailsCommand()
        {
            isReadNow = true;
            _logger.InfoReader("Start Read Command");
            NotifyOfPropertyChange(nameof(this.CanReadMailsCommand));
            Task.Run(() => {
                var answers = _reader.ReadMails(StopWords);
                _logger.InfoReader($"Finish reading, get answers {answers.Count.ToString()}");
                foreach (var answ in answers)
                {
                    /////////
                    _logger.InfoReader("Start check answers");
                    var receiver = _receivers.Where(r => r.Email == answ.Email).FirstOrDefault();
                    if (receiver != null)
                    {                        
                        _notification.AnswerGetMessage($"{answ.Email}\n {answ.Subject}");
                    }
                    _reporter.AddToReport(Path.Combine(ReportFolder_1, $"{_account.Server}.xlsx"), answ, receiver);
                    _reporter.AddToReport(Path.Combine(ReportFolder_2, $"{_account.Server}.xlsx"), answ, receiver);
                    _logger.InfoReader($"Add to report!{answ.Email}");
                }
                Execute.OnUIThread(()=> {
                    isReadNow = false;
                    NotifyOfPropertyChange(nameof(this.CanReadMailsCommand));
                    nextTimeReadInt = TimeUnixService.Timestamp() + ReadInterval;
                    NextTimeRead = TimeUnixService.TimeStamToStr(nextTimeReadInt);
                    LastTimeRead = TimeUnixService.TimeStamToStr(TimeUnixService.Timestamp());
                    NotifyOfPropertyChange(nameof(this.NextTimeRead));
                    NotifyOfPropertyChange(nameof(this.LastTimeRead));
                });
                _logger.InfoReader("Finished reading");
            });




        }
        public bool CanReadMailsCommand
        {
            get { return !isReadNow; }
        }

        private bool isReadServiceWork;

        private void ReadService()
        {
            isReadServiceWork = true;
            _logger.InfoReader("Start read service");
            NotifyOfPropertyChange(nameof(this.CanStartReadServiceCommand));
            NotifyOfPropertyChange(nameof(this.CanStopReadServiceCommand));
            Task.Run(()=> {
                while (isReadServiceWork)
                {
                    if (nextTimeReadInt < TimeUnixService.Timestamp())
                    {
                        if (!isReadNow)
                        {
                            ReadMailsCommand();
                        }                                            
                    }
                    Thread.Sleep(5000);
                }
                NotifyOfPropertyChange(nameof(this.CanStartReadServiceCommand));
                NotifyOfPropertyChange(nameof(this.CanStopReadServiceCommand));
            });
        }


    }
}
