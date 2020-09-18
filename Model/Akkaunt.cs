using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Model
{
    public class Akkaunt : ObservableObject
    {
        public string Protocol { get; set; }
        public string Server { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public int Port { get; set; }
        public string Domen { get; set; }
        public string Status { get; set; }
    }
}
