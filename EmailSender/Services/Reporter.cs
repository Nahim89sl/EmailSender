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
        private string _wordsNotExistMail;
        private string _wordsSpamMail;
        private IEmailExtractor _extractor;
        private AppSettingsModel _settingsGlobal;


        #endregion

        #region Constructor

        public Reporter(IContainer ioc)
        {
            _logger = ioc.Get<ILogger>();
            _settingsGlobal = ioc.Get<AppSettingsModel>();
            _settings = _settingsGlobal.ReaderSettings;
            _notification = ioc.Get<INotification>();
            _reporter = ioc.Get<ILoadReceivers>();
            _receivers = ioc.Get<BindableCollection<Receiver>>();
            _account = _settingsGlobal.MainAccount;
            _dbService = ioc.Get<ILoadReceivers>();
            _statuses = ioc.Get<IConsts>();
            _extractor = ioc.Get<IEmailExtractor>();
        }

        #endregion

        #region properties

        public string WordsNotExistMail => _settings.WordsNotExistMail;
        public string WordsSpamMail => _settings.WordsSpamMail;

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
                BadAnswerWorker(badAnswers);
            }
            catch(Exception ex)
            {
                _logger.ErrorReader($"Report error: {ex.Message}");
            }           
        }

        //work with good answers
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

        public void BadAnswerWorker(IList<IMailAnswer> answers)
        {
            //check unexist answers
            var unexistList = answers.Where(x => ExistWords(x.EmailText, WordsNotExistMail) != string.Empty).ToList();
            AddUnexistStatus(unexistList);
            //check spam answers
            var spamList = answers.Where(x => ExistWords(x.EmailText, WordsSpamMail) != string.Empty).ToList();
            if (spamList != null)
            {
                ChangeTimeInterval(spamList);
            }
        }

        public void AddUnexistStatus(IList<IMailAnswer> answers)
        {
            //change status in receivers list       
            foreach (var answer in answers)
            {
                _logger.InfoReader($"******** NOT EXIST Mail:");
                _logger.InfoReader($"Coincidence:  {ExistWords(answer.EmailText, WordsNotExistMail)}");
                _logger.InfoReader($"{answer.EmailSubject}");
                _logger.InfoReader($"{answer.EmailText}");

                var email = ExstractEmailFromText(answer.EmailText);
                if(email != string.Empty)
                {
                    _logger.InfoReader($"Exstract email address from letter's body {email}");
                    var receiver = _receivers.Where(r => r.Email.Equals(email)).FirstOrDefault();
                    if (receiver != null)
                    {
                        receiver.StatusSend = _statuses.ReceiverStatusBlock;
                        receiver.StatusEmailExist = _statuses.ReceiverMailNotExist;
                        _dbService.SaveReceiver(receiver);
                        _logger.InfoReader($"Change receiver's status in DB for {email}");
                    }
                    else
                    {
                        _logger.InfoReader($"Did not find {email} in list of receivers");
                    }           
                }
                else
                {
                    _logger.InfoReader($"Did not exstract mail adress from message body");
                }                       
            }           
        }

        public void ChangeTimeInterval(IList<IMailAnswer> answers)
        {
            if(answers.Count>0)
            {
                _logger.ErrorReader($"Определили что сервер в спаме ");
                if (File.Exists(_settingsGlobal.SenderSettings.IntervalsFilePath))
                {
                    var iterval = File.ReadAllLines(_settingsGlobal.SenderSettings.IntervalsFilePath).First();
                    string[] arr = iterval.Split('-');

                    if (arr.Length == 2)
                    {
                        int start;
                        int finish;
                        int.TryParse(arr.First(), out start);
                        int.TryParse(arr.Last(), out finish);
                        if (start>0 && finish>0)
                        {
                            _settingsGlobal.SenderSettings.CurrentInterval.Start = start;
                            _settingsGlobal.SenderSettings.CurrentInterval.Finish = finish;
                            _logger.ErrorReader($"Изменили интервал {iterval} так как сервер в спаме ");
                        }
                        
                    }
                }
                else
                {
                    _logger.ErrorReader($"Не найден файл интервалов");
                }
            }
        }

        /// <summary>
        /// Find in text some words from our list. List conatains words in format word1|word2|...
        /// </summary>
        public string ExistWords(string text, string findWords)
        {
            Regex rgx = new Regex(findWords);
            var match = rgx.Match(text);
            if (match.Success)
            {
                return match.Value;
            }
            return string.Empty;
        }       

        /// <summary>
        /// Exstract  emal address from text 
        /// </summary>
        /// <param name="text"></param>
        /// <returns>email address or empty string</returns>
        public string ExstractEmailFromText(string text)
        {
            var exstractor = new Regex(@"[a-zA-Z0-9+._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+");
            var match = exstractor.Match(text);
            if (match.Success)
            {
                return match.Value;
            }
            return string.Empty;
        }

        #endregion



    }
}
