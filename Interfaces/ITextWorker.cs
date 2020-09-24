using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface ITextWorker
    {
        string TextConverter(Receiver receiver, string text);
        Letter LetterRandomizeText(Receiver receiver, Letter letter);
    }
}
