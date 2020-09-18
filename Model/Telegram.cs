using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Model
{
    public class Telegram : ObservableObject
    {
        private string tgApiKey;
        public string TgApiKey
        {
            get { return tgApiKey; }
            set
            {
                Set(() => TgApiKey, ref tgApiKey, value);                
            }
        }

        private string tgIdUser;
        public string TgIdUser
        {
            get { return tgIdUser; }
            set { Set(()=>TgIdUser,ref tgIdUser,value);}
        }
    }
}
