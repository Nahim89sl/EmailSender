using EmailSender.Interfaces;
using EmailSender.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender.Services
{
    class SenderTestService : ISender
    {
        public async Task CheckAccount()
        {
            return;
        }

        public async Task SendEmail(Receiver receiver, Receiver hiddeReceiver, Letter letter)
        {           
            
            receiver.StatusSend = "SENDED";   

            
        }
    }
}
