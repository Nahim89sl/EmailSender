using AppCommon.Interfaces;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Settings;
using StyletIoC;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using EmailSender.Models;

namespace EmailSender.Services
{
    public class AutoAnswer : IAutoAnswerService
    {
        #region Private fields

        private ILogger _logger;
        private ISender _sender;
        private AppSettingsModel _settingsGlobal;
        private Letter _answerLettrer;

        #endregion

        #region Constructor

        public AutoAnswer(IContainer ioc)
        {
            _logger = ioc.Get<ILogger>();
            _sender = ioc.Get<ISender>();
            _settingsGlobal = ioc.Get<AppSettingsModel>();
            _answerLettrer = _settingsGlobal.ReaderSettings.AnswerLetter;
        }

        #endregion

        public async Task<bool> SendAnswersAsync(List<IMailAnswer> mailAnswers)
        {
            foreach(var answer in mailAnswers)
            {
                var receiver = new Receiver() { Address = answer.EmailAddress, Letter = _answerLettrer };
                await _sender.SendEmail(receiver, null, _answerLettrer);
            }
            return true;
        }
    }
}
