using AppCommon.Interfaces;
using AppCommon.MailObj;
using AppCommon.Utilities;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        private IConsts _statuses;
        private string _notExistMailWords;
        private IEmailExtractor _extractor;


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
            _statuses = ioc.Get<IConsts>();
            _extractor = ioc.Get<IEmailExtractor>();
        }

        #endregion

        #region Public Methods

        public void WorkWithResults(IList<IMailAnswer> answers)
        {
            //answers = GenerateResults();            
            _logger.InfoReader("Start check answers");
            try
            {
                var goodAnswers = answers.Where(a => a.Status == MailStatus.Good).ToList();
                goodAnswers.ForEach(a => GoodAnswerWorker(a));

                var badAnswers = answers.Where(a => a.Status == MailStatus.Block).ToList();
                badAnswers.ForEach(a => BadAnswerWorker(a));
            }
            catch(Exception ex)
            {
                _logger.ErrorReader($"Report error: {ex.Message}");
            }           
        }


        private void GoodAnswerWorker(IMailAnswer answer)
        {
            var receiver = _receivers.Where(r => r.Email.Equals(answer.EmailAddress)).FirstOrDefault();
            if (receiver != null)
            {
                receiver.StatusSend = _statuses.ReceiverStatusAnswered;
                _dbService.SaveReceiver(receiver);
                _notification.AnswerGetMessage($"{answer.EmailAddress}\n {answer.EmailSubject}");
            }
            _reporter.AddToReport(Path.Combine(_settings.ReportFolder_1, $"{_account.Server}.xlsx"), answer, receiver, _account.ServerLabelName);
            _reporter.AddToReport(Path.Combine(_settings.ReportFolder_2, $"{_account.Server}.xlsx"), answer, receiver, _account.ServerLabelName);
            _logger.InfoReader($"Add to report!{answer.EmailAddress}");
        }

        private void BadAnswerWorker(IMailAnswer answer)
        {
            //determinate that this answer is about unexitsting receiver
            if(ExistWords(answer.EmailSubject, _notExistMailWords))
            {
                //find email address from answer
                var emails = _extractor.ExtractAddress(answer.EmailText);

                foreach(var emailAddress in emails)
                {
                    //change status in receivers list
                    var receiver = _receivers.Where(r => r.Email.Equals(emailAddress)).FirstOrDefault();
                    if (receiver != null)
                    {
                        receiver.StatusSend = _statuses.ReceiverStatusBlock;
                        receiver.StatusEmailExist = _statuses.ReceiverMailNotExist;
                        _dbService.SaveReceiver(receiver);
                        _logger.InfoReader($"Mail server said that {emailAddress} - NOT EXIST");
                    }
                }               
            }

        }


        //just for test 
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


        /// <summary>
        /// Find in text some words from our list. List conatains words in format word1|word2|...
        /// </summary>
        public bool ExistWords(string text, string findWords)
        {
            Regex rgx = new Regex(findWords);
            var match = rgx.Match(text);
            if (match.Success)
            {
                return true;
            }
            return false;
        }
        #endregion



    }
}
