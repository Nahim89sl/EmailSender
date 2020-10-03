using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface INotification
    {
        string SendInfoMessage(string info);
        string SendErrorMessage(string info);
        string SendWarningMessage(string info);
    }
}
