using EmailSender.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface IReaderMails
    {
        ObservableCollection<Answer> ReadMails(string stopWords);
    }
}