﻿using EmailSender.Extentions;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Services;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using ReaderMails;
using Stylet;
using StyletIoC;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace EmailSender.ViewModels
{
    public class ReaderViewModel : PropertyChangedBase
    {
        #region Private fields
        private ILogger _logger;
        private ReaderSettings _settings;
        private MainAccount _account;
        private IDialogService _dialog;
        private IReaderMails _reader;

        private bool _isAnswer;
        private bool _isAutoStart;
        private int _readInterval;
        private string _textAnswer;
        private string _subjectLetter;

        private string _reportFolder_1;
        private string _reportFolder_2;
        private string _accountState;
        private string _serverState;
        //times
        private int nextTimeReadInt;
        private string _nextTimeRead;
        private string _lastTimeRead;
        //stop words
        private string _stopWords;
        private string _notExistList_1;
        private string _notExistList_2;
        private string _emailBlackList;
        private string _wordsSpamMailget;

        //marker of reading process now
        private bool isReadNow;
        //marker of reading service work
        private bool isReadServiceWork;
        private IContainer _ioc;
        private Reporter _reporter;
        private IWindowManager _windMng;
        //timer
        System.Timers.Timer aTimer;

        #endregion

        #region Constructor

        public ReaderViewModel(IContainer ioc)
        {
            _ioc = ioc;
            _logger = ioc.Get<ILogger>();
            _settings = ioc.Get<AppSettingsModel>().ReaderSettings;
            _account = ioc.Get<AppSettingsModel>().MainAccount;
            _dialog = ioc.Get<IDialogService>();
            _reader = ioc.Get<IReaderMails>();
            _windMng = ioc.Get<IWindowManager>();

            _reporter = new Reporter(ioc);


            //setting timer
            aTimer = new Timer(5000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Stop();

            //autostart reader service
            if (IsAutoStart)
            {
                aTimer.Start();
            }
        }
        #endregion

        #region Public fields

        //property of answering function to receivers        
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

        //property of start reading service
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

        //property reading interval
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

        //property text of answer letter
        public string TextLetter
        {
            get
            {
                _textAnswer = _settings?.AnswerLetter?.Text ?? string.Empty;
                return _textAnswer;
            }
            set
            {
                SetAndNotify(ref this._textAnswer, value);
                _settings.AnswerLetter.Text = value;
            }
        }

        //property subgect of answer letter
        public string SubjectLetter
        {
            get
            {
                _subjectLetter = _settings?.AnswerLetter?.Subject ?? string.Empty;
                return _subjectLetter;
            }
            set
            {
                SetAndNotify(ref this._subjectLetter, value);
                _settings.AnswerLetter.Subject = value;
            }
        }

        //Stop words for filtring email reading
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

        //Stop words for filtring unexist mails
        public string NotExistList_1
        {
            get
            {
                _notExistList_1 = _settings.NotExistList_1;
                return _notExistList_1;
            }
            set
            {
                SetAndNotify(ref this._notExistList_1, value);
                _settings.NotExistList_1 = value;
            }
        }

        public string NotExistList_2
        {
            get
            {
                _notExistList_2 = _settings.NotExistList_2;
                return _notExistList_2;
            }
            set
            {
                SetAndNotify(ref this._notExistList_2, value);
                _settings.NotExistList_2 = value;
            }
        }

        //Stop words for determination our mails in spam
        public string SpamList
        {
            get
            {
                _wordsSpamMailget = _settings.WordsSpamMail;
                return _wordsSpamMailget;
            }
            set
            {
                SetAndNotify(ref this._wordsSpamMailget, value);
                _settings.WordsSpamMail = value;
            }
        }

        //Stop List of balck list mails
        public string EmailBlackList
        {
            get
            {
                _emailBlackList = _settings.EmailBlackList;
                return _emailBlackList;
            }
            set
            {
                SetAndNotify(ref this._emailBlackList, value);
                _settings.EmailBlackList = value;
            }
        }

        //Report folder 1
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

        //report folder 2
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

        //State account of reading 
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
        #endregion

        #region COMMANDS

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
        
        //метод для тестов
        public void CheckAnswers()
        {
            var answers = AnsverGenerator.GetAnswers("hello| no |Glad to see you",2,"responce|answer",0,"",0,"",0);
        }

        public void StartReadServiceCommand()
        {
            isReadServiceWork = true;
            NotifyOfPropertyChange(nameof(this.CanStartReadServiceCommand));
            NotifyOfPropertyChange(nameof(this.CanStopReadServiceCommand));
            aTimer.Start();
        }
        public bool CanStartReadServiceCommand
        {
            get { return !isReadServiceWork; }
        }

        public void StopReadServiceCommand()
        {
            isReadServiceWork = false;
            NotifyOfPropertyChange(nameof(this.CanStartReadServiceCommand));
            NotifyOfPropertyChange(nameof(this.CanStopReadServiceCommand));
            aTimer.Stop();
        }
        public bool CanStopReadServiceCommand
        {
            get { return isReadServiceWork; }
        }

        public void ReadMailsCommand()
        {
            if (!(CheckFolder(ReportFolder_1) || CheckFolder(ReportFolder_2)))
            {
                return;
            }

            isReadNow = true;
            _logger.InfoReader("Start Read Command");
            NotifyOfPropertyChange(nameof(this.CanReadMailsCommand));

            Task.Run(() => {
                ReadMails();
            });
        }
        public bool CanReadMailsCommand
        {
            get { return !isReadNow; }
        }

        #endregion

        #region Private methods

        public IReaderMails Reader {
            get
            {
                if(_reader == null)
                {
                    _logger.ErrorReader($"Null value of _reader");
                    _reader = _ioc.Get<IReaderMails>();
                }
                return _reader;
            }
        }

        private void ReadMails()
        {
            _logger.InfoReader("Start task");
            try
            {
                var filtrator = new EmailFiltrator(StopWords, StopWords,
                                                    SpamList, SpamList, NotExistList_1, NotExistList_1,
                                                    NotExistList_2, NotExistList_2,
                                                    _settings.AnswerTitleList, _settings.AnswerBodyList, EmailBlackList);
                if (filtrator == null)
                    _logger.ErrorReader("Filtrator не создался корректно");

                Reader.ReadMails(filtrator);
                _reporter.WorkWithResults(filtrator);
            }
            catch(Exception ex)
            {
                _logger.ErrorReader($"ReadMails {ex.Message}");
            }
            finally
            {
                //set ui elements to next time read
                ChangeUiNextTimeRead();
                _logger.InfoReader("Finished reading");
            }            
        }

        private void ChangeUiNextTimeRead()
        {
            Execute.OnUIThread(() => {
                isReadNow = false;
                NotifyOfPropertyChange(nameof(this.CanReadMailsCommand));
                nextTimeReadInt = TimeUnixService.Timestamp() + ReadInterval;
                NextTimeRead = TimeUnixService.TimeStamToStr(nextTimeReadInt);
                LastTimeRead = TimeUnixService.TimeStamToStr(TimeUnixService.Timestamp());
                NotifyOfPropertyChange(nameof(this.NextTimeRead));
                NotifyOfPropertyChange(nameof(this.LastTimeRead));
            });

        }
               
        private bool CheckFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    _windMng.ShowMessageBox($"Папки {folder} не существует!");
                    _logger.ErrorReader("Пользователь пытался неправильно запустить чтение");
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (nextTimeReadInt < TimeUnixService.Timestamp()&&(!isReadNow))
            {
                ReadMailsCommand();
            }
        }

        private string BodyStopWords()
        {
            string resultList = string.Empty;
            try
            {
                if (NotExistList_1.Length > 3)
                    resultList = NotExistList_1;

                if (NotExistList_2.Length > 3)
                {
                    if (resultList == string.Empty)
                    {
                        resultList = NotExistList_2;
                    }
                    else
                    {
                        resultList = $"{resultList}|{NotExistList_2}";
                    }
                }
                if (SpamList.Length > 3)
                {
                    if (resultList == string.Empty)
                    {
                        resultList = SpamList;
                    }
                    else
                    {
                        resultList = $"{resultList}|{SpamList}";
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.ErrorReader($"BodyStopWords {ex.Message}\n NotExistList_1: {NotExistList_1}\n NotExistList_2: {NotExistList_2}\nSpamList:{SpamList}\nResult Filter:{resultList}");
            }
                       
            return resultList;
        }

        #endregion

        }
}
