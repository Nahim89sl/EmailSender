using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Settings.Models;
using Stylet;
using StyletIoC;

namespace EmailSender.ViewModels
{
    public class NotificationViewModel : PropertyChangedBase
    {
        private ILogger Logger { get; set; }
        private INotification Notification { get; set; }
        public NotificationAccountSettings Account { get; set; }
        
        public NotificationViewModel(IContainer ioc)
        {
            Logger = ioc.Get<ILogger>();
            Notification = ioc.Get<INotification>();
            Account = ioc.Get<NotificationAccountSettings>();
        }


        private string _apiKey;

        private string _recaiver;

        public string ApiKey
        {
            get
            {
                _apiKey = Account.ApiKey;
                return _apiKey;
            }
            set
            {
                SetAndNotify(ref this._apiKey, value);
                Account.ApiKey = value;
            }
        }

        public string Recaiver
        {
            get
            {
                _recaiver = Account.Receiver;
                return _recaiver;
            }
            set
            {
                SetAndNotify(ref this._recaiver, value);
                Account.Receiver = value;
            }
        }

        private bool _serverErrorNotify;
        public bool ServerErrorNotify
        {
            get
            {
                _serverErrorNotify = Account.ServerErrorNotify;
                return _serverErrorNotify;
            }
            set
            {
                SetAndNotify(ref this._serverErrorNotify, value);
                Account.ServerErrorNotify = value;
            }
        }

        private bool _answerGetNotify;
        public bool AnswerGetNotify
        {
            get
            {
                _answerGetNotify = Account.AnswerGetNotify;
                return _answerGetNotify;
            }
            set
            {
                SetAndNotify(ref this._answerGetNotify, value);
                Account.AnswerGetNotify = value;
            }
        }

        private bool _finishSendNotify;
        public bool FinishSendNotify
        {
            get
            {
                _finishSendNotify = Account.FinishSendNotify;
                return _finishSendNotify;
            }
            set
            {
                SetAndNotify(ref this._finishSendNotify, value);
                Account.FinishSendNotify = value;
            }
        }



        public void TestNotificationCommand()
        {
            Notification.SendErrorMessage("Test");
            Notification.SendInfoMessage("Test");
            Notification.SendWarningMessage("Test");
        }
    }
}
