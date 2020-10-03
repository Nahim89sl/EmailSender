using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.ViewModel
{
    public class TelegramViewModel
    {
        private string apiKey;

        public string ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }

        private string receiverId;

        public string ReceiverId
        {
            get { return receiverId; }
            set { receiverId = value; }
        }

        public TelegramViewModel()
        {
            ApiKey = "Apikey";
            ReceiverId = "ReceiverId";
        }



    }
}
