using EmailSender.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EmailSender.Services
{
    public class TelegramNotificationService : INotification
    {

        private TelegramBotClient botik;
        private string recaiverId;


        public TelegramNotificationService(string apiKey,string sendToId)
        {
            botik = new TelegramBotClient(apiKey);
            recaiverId = sendToId;
        }

        public  string SendErrorMessage(string info)
        {
            info = $"\u26d4 ERROR: {info}";
            var result = Task.Run(() =>SendMessage(info) );
            return result.Result;
        }

        public string SendInfoMessage(string info)
        {
            info = $"\u2709 INFO: {info}";
            var result = Task.Run(() => SendMessage(info));
            return result.Result;
        }

        public string SendWarningMessage(string info)
        {
            info = $"\u26a0 WARNING:  {info}";
            var result = Task.Run(() => SendMessage(info));
            return result.Result;
        }


        private async Task<string> SendMessage(string info)
        {
            try
            {
                Message message = await botik.SendTextMessageAsync(chatId: recaiverId, text: info);
                return message.Text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


    }
}
