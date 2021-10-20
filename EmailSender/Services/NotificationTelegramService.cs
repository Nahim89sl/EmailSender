using System;
using System.Threading.Tasks;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Settings.Models;
using Telegram.Bot;

namespace EmailSender.Services
{
    class NotificationTelegramService : INotification
    {
        private TelegramBotClient botik;
        private string _recaiver;
        private const string statusGood = "ok";
        private ILogger _logger;
        private NotificationAccountSettings _accountTg;

        public  NotificationTelegramService(NotificationAccountSettings account, ILogger logger)
        {
            _accountTg = account;
            _logger = logger;
            this.AuthBot(account);
        }

        public void AuthBot(NotificationAccountSettings account)
        {
            try
            {
                botik = new TelegramBotClient(account.ApiKey);
                _recaiver = account.Receiver;
            }
            catch (Exception ex)
            {
                account.Status = ex.Message;
                _logger.ErrorSender($"Auth telegram account error {ex.Message}");
                account.Status = ex.Message;
                return;
            }
            account.Status = statusGood;
        }

        public void SendErrorMessage(string info)
        {
            if (botik != null)
            {
                info = $"\u26d4 ERROR: {info}";
                var result = Task.Run(() => SendMessage(info));
            }
            else
            {
                _logger.ErrorSender($"Problem with telegram account");
            }
        }

        public void SendInfoMessage(string info)
        {
            if (botik != null)
            {
                info = $"\u2709 INFO: {info}";
                var result = Task.Run(() => SendMessage(info));
            }
            else
            {
                _logger.ErrorSender($"Problem with telegram account");
            }
        }

        public void SendWarningMessage(string info)
        {
            if (botik != null)
            {
                info = $"\u26a0 WARNING:  {info}";
                var result = Task.Run(() => SendMessage(info));
            }
            else
            {
                _logger.ErrorSender($"Problem with telegram account");
            }
        }

        private async Task<string> SendMessage(string info)
        {
            try
            {
                var message = await botik.SendTextMessageAsync(chatId: _recaiver, text: info);
                return message.Text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void AnswerGetMessage(string info)
        {
            if ((botik != null)&&(_accountTg.AnswerGetNotify))
            {
                info = $"\u2709 {info}";
                var result = Task.Run(() => SendMessage(info));
            }
            else
            {
                _logger.ErrorSender($"Problem with telegram account");
            }
        }

        public void FinishSendMessage(string info)
        {
            if ((botik != null) && (_accountTg.FinishSendNotify))
            {
                info = $"\u2709 INFO: {info}";
                var result = Task.Run(() => SendMessage(info));
            }
            else
            {
                _logger.ErrorSender($"Problem with telegram account");
            }
        }

        public void ServerErrorMessage(string info)
        {
            if ((botik != null) && (_accountTg.ServerErrorNotify))
            {
                info = $"\u26d4 ERROR: {info}";
                var result = Task.Run(() => SendMessage(info));
            }
            else
            {
                _logger.ErrorSender($"Problem with telegram account");
            }
        }

        public void AccountErrorMessage(string info)
        {
            if ((botik != null) && (_accountTg.AccountErrorNotify))
            {
                info = $"\u26d4 ERROR: {info}";
                var result = Task.Run(() => SendMessage(info));
            }
            else
            {
                _logger.ErrorSender($"Problem with telegram account");
            }
        }
    }
}
