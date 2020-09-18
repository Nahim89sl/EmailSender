using CommonServiceLocator;
using EmailSender.Interfaces;
using EmailSender.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using NLog;
using System.IO;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Web;
using Ookii.Dialogs.Wpf;

namespace EmailSender.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        Logger logger;
        //akaunt
        private Akkaunt akkaunt;
        public Akkaunt Akkaunt
        {
            get
            {
                return akkaunt;
            }
            set
            {
                Set(() => Akkaunt, ref akkaunt, value);
            }
        }

        //current reseiver for sending
        private Receiver currentReceiver;
        public Receiver CurrentReceiver
        {
            get
            {
                return currentReceiver;
            }
            set
            {
                Set(() => CurrentReceiver, ref currentReceiver, value);
            }
        }
        
        //receivers
        private string _receiversListPath;
        public string ReceiversListPath
        {
            get
            {
                return _receiversListPath;
            }
            set
            {
                Set(() => ReceiversListPath, ref _receiversListPath, value);
                Settings.ReceiversListPath = value;
                _settingsService.Save(Settings);
            }
        }
        public ObservableCollection<Receiver> Receivers { get; private set; }

        //pauses of sending mails
        private Pauses pauses;
        public Pauses Pauses
        {
            get
            {
                return pauses;
            }
            set
            {
                Set(() => Pauses, ref pauses, value);
            }
        }

        //our mails
        public ObservableCollection<Receiver> OurMails { get; private set; }
        public string _OurMailsListPath;
        public string OurMailsListPath
        {
            get
            {
                return _OurMailsListPath;
            }
            set
            {
                _OurMailsListPath = value;
                Settings.OurMailsListPath = value;
                _settingsService.Save(Settings);
            }
        }

        //text of email
        private string _Subject;
        private string _EmailText;

        //Property for fields mapping
        private FieldMapping fieldMapping;
        public FieldMapping FieldMapping
        {
            get
            {
                return fieldMapping;
            }
            set
            {
                Set(() => FieldMapping, ref fieldMapping, value);
            }
        }

        private int timestamp;
        //Telegramm
        private Telegram telegram;
        public Telegram Telegram
        {
            get { return telegram; }
            set { Set(()=>Telegram,ref telegram,value); }
        }
        //Answer
        private Answer answer;
        public Answer Answer
        {
            get { return answer; }
            set { Set(()=>Answer,ref answer,value); }
        }
        //letter fields
        public string Subject
        {
            get
            {
                return _Subject;
            }
            set
            {
                _Subject = value;
                Settings.Subject = value;
                _settingsService.Save(Settings);
            }
        }
        public string EmailText
        {
            get
            {
                return _EmailText;
            }
            set
            {
                _EmailText = value;
                Settings.EmailText = value;
                _settingsService.Save(Settings);
            }
        }

        //--end letter fields

        public bool StopToken;      

        private IFileService _fileService;
        private IDialogService _dialogService;
        private ISenderService _sendService;
        private ITextWorker _textRandomize;
        private IOurMails _ourMailsService;
        private ILoadRanges _loadRanges;
        private IValidate _Validate;
        private IReaderLetter _ReaderLetters;

        //read block
        private ObservableCollection<Letter> LettersAnsw;
        public bool StopReadTocken;
        private ReaderModel readerModel;
        public ReaderModel ReaderModel
        {
            get { return readerModel; }
            set { Set(()=>ReaderModel,ref readerModel, value); }
        }

        ///Settings block
        private ISettingsService _settingsService;
        public SettingsModel Settings;
        
        //hidden send
        int hiddenSend;
        int ourSend;

        //validate Status
        private bool validate;
        private string validateStatus;
        //validate Status
        public string ValidateStatus
        {
            get { return validateStatus; }
            set { Set(()=>ValidateStatus, ref validateStatus, value); }
        }
        //
        private string testEmail;
        public string TestEmail
        {
            get { return testEmail; }
            set { Set(() => TestEmail, ref testEmail, value); }
        }

        private string labelStatus;

        public string LabelStatus
        {
            get { return labelStatus; }
            set { Set(()=>LabelStatus,ref labelStatus,value); }
        }

        private string testEmailStatus;
        public string TestEmailStatus
        {
            get { return testEmailStatus; }
            set { Set(() => TestEmailStatus, ref testEmailStatus, value); }
        }

        //point of strting validate service with validate
        private bool isStartValidate;
        public bool IsStartValidate
        {
            get { return isStartValidate; }
            set { 
                Set(() => IsStartValidate, ref isStartValidate, value);
                Settings.IsStartValidate = value;
            }
        }
        //point of starting read service with reading
        private bool isStartRead;
        public bool IsStartRead
        {
            get { return isStartRead; }
            set { 
                Set(() => IsStartRead, ref isStartRead, value);
                Settings.IsStartRead = value;
            }
        }

        public RelayCommand StartSendingCommand
        {
            get;
            private set;
        }
        public RelayCommand CheckAkkauntCommand
        {
            get;
            private set;
        }
        public RelayCommand StopSendingCommand
        {
            get;
            private set;
        }
        public RelayCommand SendMailCommand
        {
            get;
            private set;
        }
        public RelayCommand LoadOurMialsCommand
        {
            get;
            private set;
        }
        public RelayCommand LoadReseiversCommand 
        { 
            get; 
            set; 
        }
        public RelayCommand LoadRangesCommand
        {
            get;
            set;
        }
        public RelayCommand LoadReportFile1Command
        {
            get;
            set;
        }
        public RelayCommand LoadReportFile2Command
        {
            get;
            set;
        }
        public RelayCommand StartReadMailCommand
        {
            get;
            set;
        }
        public RelayCommand StopReadMailCommand
        {
            get;
            set;
        }
        public RelayCommand ClearSettingsCommand
        {
            get;
            set;
        }
        public RelayCommand SaverSettingsCommand
        {
            get;
            set;
        }
        public RelayCommand StartValidateCommand
        {
            get;
            set;
        }
        public RelayCommand StartReaderServiceCommand
        {
            get;
            set;
        }
        public RelayCommand StopReaderServiceCommand
        {
            get;
            set;
        }

        private object _selectedViewModel;
        public object SelectedViewModel
        {
            get
            {
                return _selectedViewModel;
            }
            set
            {
                _selectedViewModel = value;
                Set(() => SelectedViewModel, ref _selectedViewModel, value);
            }
        }



        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public MainWindowViewModel()
        {
            DispatcherHelper.Initialize();

            SelectedViewModel = new TestController();

            _fileService = ServiceLocator.Current.GetInstance<IFileService>();
            _dialogService = ServiceLocator.Current.GetInstance<IDialogService>();
            _sendService = ServiceLocator.Current.GetInstance<ISenderService>();
            _textRandomize = ServiceLocator.Current.GetInstance<ITextWorker>();
            _settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            _ourMailsService = ServiceLocator.Current.GetInstance<IOurMails>();
            _loadRanges = ServiceLocator.Current.GetInstance<ILoadRanges>();
            _Validate = ServiceLocator.Current.GetInstance<IValidate>();
            _ReaderLetters = ServiceLocator.Current.GetInstance<IReaderLetter>();

            logger = LogManager.GetCurrentClassLogger();
            logger.Info("App 8.6.004 started");
            OurMails = new ObservableCollection<Receiver>();
            Settings = _settingsService.Load();
            validate = true;

            if(Settings!=null)
            {
                Akkaunt = Settings?.Akkaunt ?? new Akkaunt();
                Pauses = Settings?.Pauses ?? new Pauses();
                FieldMapping = Settings?.FieldMapping ?? new FieldMapping();
                Telegram = Settings?.Telegram ?? new Telegram();
                Answer = Settings?.Answer ?? new Answer();
                ReaderModel = Settings?.ReaderModel ?? new ReaderModel();

                _Subject = Settings.Subject;
                _EmailText = Settings.EmailText;

                //restore list of receivers
                if (File.Exists(Settings.ReceiversListPath))
                {
                    ReceiversListPath = Settings.ReceiversListPath;
                    Receivers = _fileService.Open(ReceiversListPath, FieldMapping);
                }
                else
                {
                    Receivers = new ObservableCollection<Receiver>();
                }
                //restore list of our mails
                if (File.Exists(Settings.OurMailsListPath))
                {
                    OurMailsListPath = Settings.OurMailsListPath;
                    _ourMailsService.LoadAsync(Settings.OurMailsListPath, OurMails);
                }
                logger.Info("Settings were loaded");
            }
            else
            {
                Akkaunt = new Akkaunt();
                Answer = new Answer();
                FieldMapping = new FieldMapping();
                Pauses = new Pauses();
                Receivers = new ObservableCollection<Receiver>();
                logger.Info("Settings were empty");
                Telegram = new Telegram();
                ReaderModel = new ReaderModel();
            }

            LoadReseiversCommand = new RelayCommand(ExecudeLoadResaivers);
            StartSendingCommand = new RelayCommand(ExecutStartSend, CanExecutStartSend);
            StopSendingCommand = new RelayCommand(ExecutStopSend, CanExecutStopSend);
            LoadOurMialsCommand = new RelayCommand(ExecuteLoadOurMialsAsync, CanExecuteLoadOurMials);
            CheckAkkauntCommand = new RelayCommand(ExecudeCheckAkkaunt, true);
            LoadRangesCommand = new RelayCommand(ExecudeLoadRanges);
            LoadReportFile1Command = new RelayCommand(ExecuteLoadReportFile1);
            LoadReportFile2Command = new RelayCommand(ExecuteLoadReportFile2);
            StartReadMailCommand = new RelayCommand(ExecuteReadMail, CanExecuteReadMail);
            StopReadMailCommand = new RelayCommand(ExecuteStopReadMail,CanExecuteStopReadMail);
            ClearSettingsCommand = new RelayCommand(ExecuteClearSettings);
            SaverSettingsCommand = new RelayCommand(ExecuteSaveSettings);
            StartValidateCommand = new RelayCommand(ExecudeStartValidateAsync, ExecuteStartValidate);
            StartReaderServiceCommand = new RelayCommand(ExecuteStartReaderService, CanExecuteStartReaderService);
            StopReaderServiceCommand = new RelayCommand(ExecuteStopReaderService, CanExecuteStopReaderService);
            SendMailCommand = new RelayCommand(ExecuteSendMail, CanExecuteSendMail);
            Akkaunt.Status = "";

            IsStartRead = Settings.IsStartRead;
            IsStartValidate = Settings.IsStartValidate;

            Settings.Answer = Answer;
            Settings.Akkaunt = Akkaunt;
            Settings.Pauses = Pauses;
            Settings.FieldMapping = FieldMapping;
            Settings.Telegram = Telegram;
            Settings.ReaderModel = ReaderModel;
            ReaderModel.IsReadNow = "wait reading";
            hiddenSend = 0;
            ourSend = 0;
            StopReadTocken = false;
            Pauses.CheckPauses();
        }

        private async void ExecuteLoadOurMialsAsync()
        {
            try
            {
                if (_dialogService.OpenFileDialog() == true)
                {
                    OurMails.Clear();
                    _ourMailsService.LoadAsync(_dialogService.FilePath, OurMails);
                    OurMailsListPath = _dialogService.FilePath;
                    RaisePropertyChanged(() => OurMails);
                    RaisePropertyChanged(() => OurMailsListPath);
                    Settings.OurReceiversListPath = _dialogService.FilePath;
                    _settingsService.Save(Settings);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExecuteLoadOurMialsAsync " + ex.Message);
            }
            
        }
        //report file load
        private void ExecuteLoadReportFile1()
        {
            try
            {               
                ReaderModel.ReportFolder1 = _dialogService.OpenFolder();                                
            }
            catch(Exception ex)
            {
                logger.Error("ExecuteLoadReportFile1 "+ex.Message);
            }
            
        }

        private void ExecuteLoadReportFile2()
        {
            try
            {
                ReaderModel.ReportFolder2 = _dialogService.OpenFolder();
            }
            catch (Exception ex)
            {
                logger.Error("ExecuteLoadReportFile2 " + ex.Message);
            }

        }
        private bool CanExecuteLoadOurMials()
        {
            return true;
        }
        //download list of receivers
        public void ExecudeLoadResaivers()
        {
            try
            {
                if (_dialogService.OpenFileDialog() == true)
                {
                    //load list of receivers   
                    Receivers.Clear();
                    Receivers = _fileService.Open(_dialogService.FilePath, FieldMapping);
                    RaisePropertyChanged("Receivers");

                    //update state of sending command
                    StartSendingCommand.RaiseCanExecuteChanged();

                    //update settings value of file path
                    Settings.ReceiversListPath = _dialogService.FilePath;
                    _receiversListPath = _dialogService.FilePath;
                    RaisePropertyChanged("ReceiversListPath");
                    _settingsService.Save(Settings);
                    logger.Info("Loaded list of receivers");
                }
            }catch(Exception ex)
            {
                logger.Error("ExecudeLoadResaivers " + ex.Message);
            }            
        }
        //start sending our mails
        public async void ExecutStartSend()
        {
            if(OurMails.Count<1)
            {
                MessageBox.Show("Вы не загрузили список своих почт");
                return;
            }

            if (IsStartValidate)
            {
                ExecudeStartValidateAsync();
            }
            if (IsStartRead)
            {
                ExecuteStartReaderService();
            }
            if ((Subject == "") || (EmailText == ""))
            {
                MessageBox.Show("Проверьте правильность заполнения темы и текста отправляемого письма");
                return;
            }


            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    //update state of button
                    StopToken = true;
                    StopSendingCommand.RaiseCanExecuteChanged();
                    StartSendingCommand.RaiseCanExecuteChanged();
                    logger.Info("Change status for buttons");
                });
            }catch(Exception ex)
            {
                logger.Error("update state of button " + ex.Message);
            }

            try
            {
                await Task.Run(async () =>
                {
                    
                    while (StopToken)
                    {
                        //take receiver from list
                        Receiver receiver = null;
                        try
                        {
                            receiver = Receivers.Where(a => a.StatusSend == "no").FirstOrDefault();
                        }catch(Exception ex)
                        {
                            logger.Error("Cant get new receiver "+ex.Message);
                        }

                        logger.Info("Start with receiver " + receiver.Email);
                        
                        if (receiver == null)
                        {
                            StopToken = false;
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                StopSendingCommand.RaiseCanExecuteChanged();
                                StartSendingCommand.RaiseCanExecuteChanged();
                                LabelStatus = "Finished";
                                logger.Info("Finish send mails");
                            });

                        }
                        else
                        {
                            string subject = "";
                            string mailText = "";
                            try
                            {
                                subject = _textRandomize.TextConverter(receiver, Subject);
                                logger.Info("Subject: " + subject);
                                mailText = _textRandomize.TextConverter(receiver, EmailText);
                                logger.Info("Subject: " + mailText);
                            }
                            catch(Exception ex)
                            {
                                logger.Error("TextConverter error " + ex.Message);
                            }                           

                            if (Pauses.SendHiddenCopy < hiddenSend)//if add hidden mail to main receiver
                            {
                                hiddenSend = 0;
                                var ourReceiver = OurMails.OrderBy(a => a.Count).FirstOrDefault();
                                receiver.Bcc = ourReceiver.Email;
                                ourReceiver.Count++;
                                logger.Info("Add hidden copy " + ourReceiver.Email);
                            }
                            //if send to our mail
                            if (Pauses.SendToOurMail < ourSend) 
                            {
                                ourSend = 0;
                                var ourReceiver = OurMails.OrderBy(a => a.Count).FirstOrDefault();
                                ourReceiver.Count++;
                                await _sendService.SendAsync(Akkaunt, ourReceiver, subject, mailText);
                                logger.Info("Sended to our mail " + ourReceiver.Email);
                                int p = 10;
                                try
                                {
                                    p = Pauses.GetPause();
                                }catch(Exception ex)
                                {
                                    logger.Error("GetPause "+ex.Message);
                                }                                
                                Thread.Sleep(p * 1000);
                            }
                            
                            //send mail
                            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                                CurrentReceiver = receiver;
                                LabelStatus = receiver.Email;
                            });
                            logger.Info("try send email to " + receiver.Email);
                            await _sendService.SendAsync(Akkaunt, receiver, subject, mailText);
                            logger.Info("Akkaunt status " + Akkaunt.Status);
                            logger.Info("Sended to " + receiver.Email);                            
                            receiver.StatusSend = "SENDED";
                            //get pause
                            int pause = 10;
                            try
                            {
                                pause = Pauses.GetPause();
                            }
                            catch (Exception ex)
                            {
                                logger.Error("GetPause " + ex.Message);
                            }
                            
                            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                                LabelStatus = "Pause "+pause.ToString();                                
                            });
                            Thread.Sleep(pause * 1000);

                            try
                            {
                                await Task.Run(() => {
                                    _fileService.SaveChanges(ReceiversListPath, receiver, FieldMapping);
                                });
                            }
                            catch(Exception ex)
                            {
                                logger.Error("SaveChanges "+ex.Message);
                            }
                            
                        }
                        hiddenSend++;
                        ourSend++;
                    }

                    await Task.Run(() => {
                        _settingsService.Save(Settings);
                    });


                });


            }catch(Exception ex)
            {
                logger.Error("ExecutStartSend " + ex.Message);
            }
        }
        private bool CanExecutStartSend()
        {
            if ((Receivers == null)||(StopToken))
            {
                return false;
            }
            return true;
        }
        //Stop sending mails
        private void ExecutStopSend()
        {
            try
            {
                StopToken = false;
                StopSendingCommand.RaiseCanExecuteChanged();
                StartSendingCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                logger.Error("ExecutStopSend " + ex.Message);
            }            
        }
        private bool  CanExecutStopSend()
        {
            return StopToken;
        }
        public void ExecuteShowMessage()
        {
            MessageBox.Show("Message");
        }
        public bool CanExecuteShowMessage()
        {
            return true;
        }
        //Check authorisation of akkaunt
        private void ExecudeCheckAkkaunt()
        {
            try
            {
                Task.Run(async () => {
                    await _sendService.Authentification(Akkaunt);
                    DispatcherHelper.CheckBeginInvokeOnUI(()=> {
                        RaisePropertyChanged(() => Akkaunt);
                        SendMailCommand.RaiseCanExecuteChanged();
                    });
                    
                });
            }catch(Exception ex)
            {
                logger.Error("ExecudeCheckAkkaunt " + ex.Message);
            }
            
        }
        private bool CanExecudeCheckAkkaunt()
        {
            return true;
        }
        //load reanges for pauses
        private void ExecudeLoadRanges()
        {
            try
            {
                if (_dialogService.OpenFileDialog() == true)
                {
                    Pauses.Ranges.Clear();
                    _loadRanges.Load(_dialogService.FilePath, Pauses);

                    pauses.RangePath = _dialogService.FilePath;
                }
                _settingsService.Save(Settings);
            }catch(Exception ex)
            {
                logger.Error("ExecudeLoadRanges "+ex.Message);
            }
            
        }
        //Send single mail for tests
        private async void ExecuteSendMail()
        {
            var testReceiver = new Receiver()
            {
                Bcc = "",
                Email = TestEmail,
                Address = "Address",
                CC = "",
                CompanyName = "",
                Count = 0,
                FieldAddress = "Address",
                FieldContractAmount = "",
                FieldDate1 = "FieldDate1",
                FieldDate2 = "FieldDate2",
                FieldDate3 = "FieldDate3",
                FieldInn = "FieldInn",
                FieldOkvd = "FieldOkvd",
                FieldPhone = "FieldPhone",
                FieldRecord1 = "FieldRecord1",
                FieldRecord2 = "FieldRecord2",
                FieldRecord3 = "FieldRecord3",
                IdReceiver = 0,
                PersonName = "PersonName",
            };
            string subject = _textRandomize.TextConverter(testReceiver, Subject);
            string mailText = _textRandomize.TextConverter(testReceiver, EmailText);
            await _sendService.SendAsync(Akkaunt, testReceiver, subject, mailText);
            RaisePropertyChanged(() => Akkaunt);
            TestEmailStatus = "Sended";
            RaisePropertyChanged(() => TestEmailStatus);
        }
        private bool CanExecuteSendMail()
        {
            if(Akkaunt.Status != "" )
            {
                return true;
            }
            return false;
        }
        //Read Mails 
        private void ExecuteReadMail()
        {
            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    ReaderModel.LastTimeRead = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    StartReadMailCommand.RaiseCanExecuteChanged();
                    ReaderModel.NextReadInt = TUnix.Timestamp() + ReaderModel.RederPauseInterval;
                    ReaderModel.NextTimeRead = TUnix.TimeStamToStr(ReaderModel.NextReadInt);
                    ReaderModel.IsReadNow = "Reading";
                    StartReadMailCommand.RaiseCanExecuteChanged();
                });
                Task.Run(async () => {
                    LettersAnsw = _ReaderLetters.ReadMails(Akkaunt, ReaderModel.StopWords);
                    foreach (var letter in LettersAnsw)
                    {
                        Receiver receiverAns = Receivers.Where(a => a.Email == letter.EmailSender).FirstOrDefault();
                        if (receiverAns != null)
                        {
                            //update status of receiver
                            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                                receiverAns.StatusSend = "answered";
                            });
                            letter.Id = receiverAns.IdReceiver;
                            //send answer to receiver
                            if (ReaderModel.CanSendAnswer)
                            {
                                await _sendService.SendAsync(Akkaunt, receiverAns, _textRandomize.TextConverter(receiverAns, ReaderModel.AnswerLetter.Subject), _textRandomize.TextConverter(receiverAns, ReaderModel.AnswerLetter.Text));
                                logger.Info("Send answer to " + receiverAns.Email);
                            }
                        }
                    }
                    //save changes to files
                    ReaderModel.ReportFilePath1 = ReaderModel.ReportFolder1 + "\\Report_" + Akkaunt.Domen + ".xlsx";
                    ReaderModel.ReportFilePath2 = ReaderModel.ReportFolder2 + "\\Report_" + Akkaunt.Domen + ".xlsx";
                    _fileService.AddToReport(ReaderModel.ReportFilePath1, LettersAnsw);
                    _fileService.AddToReport(ReaderModel.ReportFilePath2, LettersAnsw);
                    _fileService.Save(ReceiversListPath, Receivers, FieldMapping);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ReaderModel.IsReadNow = "wait for reading";
                        StartReadMailCommand.RaiseCanExecuteChanged();
                    });
                });
            }
            catch (Exception ex)
            {
                logger.Error("ExecuteReadMail " + ex.Message);
            }
        }

        private bool CanExecuteReadMail()
        {
            if(ReaderModel.IsReadNow == "Reading")
            {
                return false;
            }
            return true;
        }
        private void ExecuteStopReadMail()
        {

        }
        private bool CanExecuteStopReadMail()
        {
            return true;
        }
        //clear app's settings
        private void ExecuteClearSettings()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(()=> {
                try
                {
                    Settings = new SettingsModel();
                    _settingsService.Save(Settings);
                }
                catch (Exception ex)
                {
                    logger.Error("ExecuteClearSettings " + ex.Message);
                }
            });
        }
        private void ExecuteSaveSettings()
        {
            try
            {
                _settingsService.Save(Settings);
                MessageBox.Show("Настройки сохранены");
            }
            catch (Exception ex)
            {
                logger.Error("ExecuteSaveSettings " + ex.Message);
            }
            
        }
        private void ExecudeStartValidateAsync()
        {
            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(()=> {
                    validate = false;
                    StartValidateCommand.RaiseCanExecuteChanged();
                });
                
                Task.Run(async () =>
                {
                    Random rndd = new Random();
                    Receiver receiver = Receivers.Where(a => a.StatusEmailExist != "exist").FirstOrDefault();
                    while(receiver!=null)
                    //foreach (var receiver in Receivers)
                    {
                        await _Validate.ChooseValidator(receiver);
                        DispatcherHelper.CheckBeginInvokeOnUI(() => {
                            ValidateStatus = receiver.Email;
                        });
                        Thread.Sleep(rndd.Next(1, 5) * 1000);
                        receiver = Receivers.Where(a => a.StatusEmailExist == "no").FirstOrDefault();
                    }
                    logger.Info("Finished validation");

                    _fileService.Save(ReceiversListPath, Receivers, FieldMapping);
                    DispatcherHelper.CheckBeginInvokeOnUI(() => {
                        validate = true;
                        StartValidateCommand.RaiseCanExecuteChanged();
                    });
                });
            }            
            catch (Exception ex)
            {
                logger.Error("ExecudeStartValidateAsync "+ex.Message);
            }            
        }
        private bool ExecuteStartValidate()
        {                                  
            if (Receivers.Count < 0)
            {
                validate = false;
            }            
            return validate;
        }

        private void ExecuteStartReaderService()
        {
            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    StopReadTocken = true;
                    StartReaderServiceCommand.RaiseCanExecuteChanged();
                    StopReaderServiceCommand.RaiseCanExecuteChanged();
                });

                Task.Run(() =>
                {
                    while (StopReadTocken)
                    {
                        int stam = TUnix.Timestamp();
                        if (ReaderModel.NextReadInt < stam)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                ReaderModel.NextReadInt = TUnix.Timestamp() + ReaderModel.RederPauseInterval;
                            });
                            logger.Info("Read mails");
                            ExecuteReadMail();
                        }
                        Thread.Sleep(5000);
                    }
                });
            }catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        private bool CanExecuteStartReaderService()
        {
            return !StopReadTocken;
        }

        private void ExecuteStopReaderService()
        {
            try
            {
                StopReadTocken = false;
                StopReaderServiceCommand.RaiseCanExecuteChanged();
                StartReaderServiceCommand.RaiseCanExecuteChanged();
            }catch(Exception ex)
            {
                logger.Error("ExecuteStopReaderService " + ex.Message);
            }
            
        }
        private bool CanExecuteStopReaderService()
        {
            return StopReadTocken;
        }
    
    }
}
