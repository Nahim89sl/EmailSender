using AppCommon.Interfaces;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Settings;
using StyletIoC;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using EmailSender.Models;
using Stylet;
using System.Linq;

namespace EmailSender.Services
{
    public class AutoAnswer : IAutoAnswerService
    {
        #region Private fields

        private ILogger _logger;
        private ISender _sender;
        private AppSettingsModel _settingsGlobal;
        private Letter _answerLettrer;
        private TextRundomizer _textConvertor;
        private BindableCollection<Receiver> _receivers;
        private ILoadReceivers _dbService;
        private IConsts _statuses;
        private INotification _notification;
        #endregion

        #region Constructor

        public AutoAnswer(IContainer ioc)
        {
            _logger = ioc.Get<ILogger>();
            _sender = ioc.Get<ISender>();
            _settingsGlobal = ioc.Get<AppSettingsModel>();
            _answerLettrer = _settingsGlobal.ReaderSettings.AnswerLetter;
            _textConvertor = new TextRundomizer();
            _receivers = ioc.Get<BindableCollection<Receiver>>();
            _dbService = ioc.Get<ILoadReceivers>();
            _statuses = ioc.Get<IConsts>();
            _notification = ioc.Get<INotification>();
        }

        #endregion

        #region Public methods

        public async Task<bool> SendAnswersAsync(List<IMailAnswer> mailAnswers)
        {
            _logger.InfoReader("Получили письма для автоответа");
            foreach(var answer in mailAnswers)
            {
                //Create object of receiver
                var receiver = _receivers.Where(r => r.Email.Equals(answer.EmailAddress)).FirstOrDefault();
                if(receiver == null)
                {
                    _logger.ErrorReader($"Не нашли {answer.EmailAddress} в нашем списке");
                    receiver = new Receiver() { Address = answer.EmailAddress, Letter = _answerLettrer };
                }
                
                //Randomize text of letter
                _textConvertor.LetterRandomizeText(receiver, _answerLettrer);

                //send letter with answer
                await _sender.SendEmail(receiver, new Receiver(), receiver.Letter);
                _logger.InfoReader($"Отправили автоответ {receiver.Address}");

                //change status of receiver 

                if (receiver != null)
                {
                    receiver.StatusSend = _statuses.ReceiverStatusAutoanswered;
                    
                    //change status in db
                    _dbService.SaveReceiver(receiver);
                    
                    //notify to telegram
                    _notification.AnswerGetMessage($"Отправили автоответ {receiver.Address}");
                }
            }
            return true;
        }

        #endregion
    }
}
