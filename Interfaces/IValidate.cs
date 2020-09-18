using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface IValidate
    {
        Task ChooseValidator(Receiver receiver);
        Task Gmail(Receiver receiver);
        Task MailRu(Receiver receiver);
        Task YaRu(Receiver receiver);
        Task Other(Receiver receiver);
        Task StartValidate(ObservableCollection<Receiver> recivers, string validationStatus);
    } 
}
