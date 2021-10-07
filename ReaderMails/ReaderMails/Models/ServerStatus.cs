using System;
using System.Collections.Generic;
using System.Text;

namespace ReaderMails.Models
{
    public class ServerStatus
    {
        public string OK { get; set; } = "ok";
        public string AuthFail { get; set; } = "Auth error";
        public string TimeOut { get; set; } = "Time out";

    }
}
