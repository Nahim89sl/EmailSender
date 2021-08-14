
using AppCommon.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    public interface IAutoAnswerService
    {
        Task<bool> SendAnswersAsync(List<IMailAnswer> mailAnswers);
    }
}
