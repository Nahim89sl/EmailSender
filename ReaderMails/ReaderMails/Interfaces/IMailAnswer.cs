using MailKit;
using MimeKit;
using ReaderMails.Enums;

namespace ReaderMails.Interfaces
{
    public interface IMailAnswer
    {
        UniqueId Id { get; set; }
        MimeMessage Message { get; set; }
        MailStatus Status { get; set; }

        string From { get; }
        string EmailAddress { get; }
        string EmailSubject { get; }
        string EmailText { get; }
    }
}
