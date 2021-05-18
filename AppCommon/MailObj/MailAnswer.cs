using AppCommon.Interfaces;
using MimeKit;


namespace AppCommon.MailObj
{
    public class MailAnswer : IMailAnswer
    {
        public MimeMessage Message { get; set; }
        public MailStatus Status { get; set; }
    }
}
