
using ReaderMails.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface IAutoAnswerService
    {
        Task<bool> SendAnswersAsync(IEnumerable<IMailAnswer> mailAnswers);
    }
}
