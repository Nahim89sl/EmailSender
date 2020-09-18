using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface ISenderService
    {
        //string Send(string fromName, string fromMail, string resaiverName, string resaiverMail, string hiddenName, string hiddenMail, string textMail, string subject);
        Task<bool> Authentification(Akkaunt mailServer);
        Task SendAsync(Akkaunt mailServer, Receiver receiver, string subject, string mailText);
        bool ValidateReceiver(Receiver receiver);
    }
}
