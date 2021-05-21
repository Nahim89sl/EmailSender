using AppCommon.MailObj;
using MimeKit;

namespace AppCommon.Interfaces
{
    public interface IMailAnswer
    {
        MimeMessage Message { get; set; }
        MailStatus Status { get; set; }

        string From { get; }
        string EmailAddress { get; }
        string EmailSubject { get; }
        string EmailText { get; }
    }
}
