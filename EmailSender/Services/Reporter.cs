using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using Stylet;
using StyletIoC;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace EmailSender.Services
{
    public class Reporter
    {
        #region Private fields
        private ILogger _logger;
        private ReaderSettings _settings;
        private INotification _notification;
        private ILoadReceivers _reporter;
        private BindableCollection<Receiver> _receivers;
        private MainAccount _account;
        private ILoadReceivers _dbService;
        private const string AnswerStatus = "ANSWERED";
        #endregion

        #region Constructor

        public Reporter(IContainer ioc)
        {
            _logger = ioc.Get<ILogger>();
            _settings = ioc.Get<AppSettingsModel>().ReaderSettings;
            _notification = ioc.Get<INotification>();
            _reporter = ioc.Get<ILoadReceivers>();
            _receivers = ioc.Get<BindableCollection<Receiver>>();
            _account = ioc.Get<AppSettingsModel>().MainAccount;
            _dbService = ioc.Get<ILoadReceivers>();
        }

        #endregion

        #region Public Methods
        public void WorkWithResults(ObservableCollection<Answer> answers)
        {
            answers = GenerateResults();
            _logger.InfoReader($"Finish reading, get answers {answers.Count.ToString()}");
            try
            {
                foreach (var answ in answers)
                {
                    /////////
                    _logger.InfoReader("Start check answers");
                    var receiver = _receivers.Where(r => r.Email.Equals(answ.Email)).FirstOrDefault();
                    if (receiver != null)
                    {
                        receiver.StatusSend = AnswerStatus;
                        _dbService.SaveReceiver(receiver);
                        _notification.AnswerGetMessage($"{answ.Email}\n {answ.Subject}");
                    }
                    _reporter.AddToReport(Path.Combine(_settings.ReportFolder_1, $"{_account.Server}.xlsx"), answ, receiver);
                    _reporter.AddToReport(Path.Combine(_settings.ReportFolder_2, $"{_account.Server}.xlsx"), answ, receiver);
                    _logger.InfoReader($"Add to report!{answ.Email}");
                }
            }
            catch(Exception ex)
            {
                _logger.ErrorReader($"Report error: {ex.Message}");
            }           
        }

        private ObservableCollection<Answer>  GenerateResults()
        {
            ObservableCollection<Answer> answers = new ObservableCollection<Answer>();
            answers.Add(new Answer() { 
                Email = "apex.tk@mail.ru",
                From = "Herogivi <manager@i-novus.ru>",
                Subject = "Test Sub1",
                Text = "Body of answer this1"
            });
            answers.Add(new Answer()
            {
                Email = "trmsar@mail.ru",
                From = "trmsar <trmsar@mail.ru>",
                Subject = "Test Sub 2",
                Text = "Body of answer 2222"
            });
            answers.Add(new Answer()
            {
                Email = "kskgrupp2@bk.ru",
                From = "kskgrupp2 <kskgrupp2@bk.ru>",
                Subject = "Test Sub333",
                Text = "Body of answer this333"
            });
            answers.Add(new Answer()
            {
                Email = "zyf-92333@mail.ru",
                From = "Herogivi <zyf-92333@mail.ru>",
                Subject = "Test Sub 4444",
                Text = "Body of answer this 444"
            });
            return answers;
        }
        #endregion



    }
}
