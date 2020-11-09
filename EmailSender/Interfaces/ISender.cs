using EmailSender.Models;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface ISender
    {
        Task CheckAccount();
        Task SendEmail(Receiver receiver, Receiver hiddeReceiver, Letter letter);
    }
}