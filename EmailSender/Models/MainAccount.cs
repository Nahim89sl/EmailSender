using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Models
{
    public class MainAccount
    {
        public string Protocol { get; set; }
        public string Server { get; set; }
        public string ServerStatus { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public string AccountStatus { get; set; }
        public int Port { get; set; }
        public bool ReaderOnStart { get; set; }
    }
}
