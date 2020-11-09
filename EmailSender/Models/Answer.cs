using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Models
{
    public class Answer
    {
        public string From { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }
}
