using AppCommon.Interfaces;
using EmailSender.Extentions;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using ReaderMails;
using ReaderMails.Interfaces;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
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
        private IConsts _statuses;
        private AppSettingsModel _settingsGlobal;
        private IAutoAnswerService _autoAnswerService;

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
            _autoAnswerService = ioc.Get<IAutoAnswerService>();
        }

        #endregion

        #region properties

        public string NotExistList_1 => _settings.NotExistList_1;
        public string NotExistList_2 => _settings.NotExistList_2;
        public string WordsSpamMail => _settings.WordsSpamMail;
        public string AnswerBodyFilter => _settings.AnswerBodyList;

        #endregion

        #region Public Methods

        public void WorkWithResults(EmailFiltrator results)
        {                       
            //good answer
            //There we marked all good answers in our db and exel
            if (results.GetMailsForSave == null)
                return;
            foreach(var answer in results.GetMailsForSave)
            {
                GoodAnswerWorker(answer);
            }

            //auto responce action
            //GetAnswerMails - it is inversion filter for making autoanswer
            if (_settingsGlobal.ReaderSettings.IsAnswer)//Auto answer work
            {
                var autoAnswers = GetMailsForAutoAnswer(results.GetMailsForSave.ToList(), results.GetAnswerMails.ToList());
                _logger.InfoReader($"Писем для автоответа: {autoAnswers.Count}");
                if(autoAnswers.Any())
                    _autoAnswerService.SendAnswersAsync(autoAnswers);
            }

            //span action
            if (results.GetSpamMails.Any())
            {
                ChangeTimeInterval();
            }

            //unexist actions
            List<IMailAnswer> unxistList = new List<IMailAnswer>();
            if(results.GetUnexist1Mails.Any() || results.GetUnexist2Mails.Any())
            {
                unxistList.AddRange(results.GetUnexist1Mails);
                unxistList.AddRange(results.GetUnexist2Mails);
                AddUnexistStatus(unxistList);
            }
        }

        private void GoodAnswerWorker(IMailAnswer answer)
        {
            var receiver = _receivers.Where(r => r.Email.Equals(answer.EmailAddress)).FirstOrDefault();
            if (receiver != null)
            {
                receiver.StatusSend = _statuses.ReceiverStatusAnswered;
                //change status in db
                _dbService.SaveReceiver(receiver);
                _notification.AnswerGetMessage($"{answer.EmailAddress}\n {answer.EmailSubject}");
            }
            //add record to report files
            _reporter.AddToReport(Path.Combine(_settings.ReportFolder_1, $"{_account.Server}.xlsx"), answer, receiver, _account.ServerLabelName);
            _reporter.AddToReport(Path.Combine(_settings.ReportFolder_2, $"{_account.Server}.xlsx"), answer, receiver, _account.ServerLabelName);
            _logger.InfoReader($"Add to report: {answer.EmailAddress}");
        }

        public void AddUnexistStatus(IList<IMailAnswer> answers)
        {
            //change status in receivers list       
            foreach (var answer in answers)
            {
                //chek list 1
                var inList1 = answer.EmailText.ExistWords(NotExistList_1);
                var inList2 = answer.EmailText.ExistWords(NotExistList_2);
               
                _logger.InfoReader($"\n******** NOT EXIST Mail:**********************");
                _logger.InfoReader($"Coincidence list 1:  {inList1}");
                _logger.InfoReader($"Coincidence list 2:  {inList2}");
                //_logger.InfoReader($"{answer.EmailSubject}");
                //_logger.InfoReader($"{answer.EmailText}");
              
                var email = answer.EmailText.ExstractEmailFromText();
                if(email != string.Empty)
                {
                    _logger.InfoReader($"Exstract email address from letter's body {email}");
                    var receiver = _receivers.Where(r => r.Email.Equals(email)).FirstOrDefault();
                    if (receiver != null)
                    {
                        if ((inList1 != string.Empty)&&(inList2 != string.Empty))
                        {
                            receiver.StatusSend = _statuses.ReceiverStatusWariant;                            
                        }
                        if ((inList1 != string.Empty) && (inList2 == string.Empty))
                        {
                            receiver.StatusSend = _statuses.ReceiverStatusBlock;
                            receiver.StatusEmailExist = _statuses.ReceiverStatusNotExist;
                        }
                        _dbService.SaveReceiver(receiver);
                        _logger.InfoReader($"Change receiver's status in DB for {email} {receiver.StatusSend}");
                    }
                    else
                    {
                        _logger.InfoReader($"Did not find {email} in receivers");
                    }           
                }
                else
                {
                    _logger.InfoReader($"Did not exstract mail adress from message body");
                }
                _logger.InfoReader($"******************************\n");
            }           
        }

        public void ChangeTimeInterval()
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

        private IList<IMailAnswer> GetMailsForAutoAnswer(IList<IMailAnswer> goodAnswers, IList<IMailAnswer> deleteAnswers)
        {
            var resList = new List<IMailAnswer>();
            if (goodAnswers != null)
            {
                if (deleteAnswers.Count < 1)
                    return goodAnswers;

                foreach (var answer in goodAnswers)
                {
                    if(!deleteAnswers.Any(x => x.Id == answer.Id))
                    {
                        //if (_receivers.Any(x => x.Email == answer.EmailAddress))
                        resList.Add(answer);
                    }
                }
            }
            return resList;
        }

        #endregion

    }
}
