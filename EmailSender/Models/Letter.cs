using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Text;

namespace EmailSender.Models
{
    public class Letter
    {
        public string Subject { set; get; }
        public string Text { set; get; }
        public string Status { set; get; }
    }
}
