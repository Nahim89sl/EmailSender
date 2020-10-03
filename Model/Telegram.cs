using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;

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

        private bool _isEndList;
        public bool IsEndList
        {
            get { return _isEndList; }
            set { Set(() => IsEndList, ref _isEndList, value); }
        }

        private bool _isServerProblem;
        public bool IsServerProblem
        {
            get { return _isServerProblem; }
            set { Set(() => IsServerProblem, ref _isServerProblem, value); }
        }

    }
}
