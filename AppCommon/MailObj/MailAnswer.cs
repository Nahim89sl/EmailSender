using AppCommon.Interfaces;
using MimeKit;
using System;
using System.Linq;

namespace AppCommon.MailObj
{
    public class MailAnswer : IMailAnswer
    {
        public MimeMessage Message { get; set; }
        public MailStatus Status { get; set; }

        public string From => Message?.From.ToString();
        public string EmailAddress => Message?.From.Mailboxes.First().Address.ToLower();
        public string EmailSubject => Message?.Subject.ToString();
        public string EmailText => GetLetterText(Message);

        private string GetLetterText(MimeMessage letter)
        {
            if (letter == null)
                return "No body";
            try
            {
                if ((letter.TextBody != null) && (letter.TextBody.Length > 5))
                {
                    return letter.TextBody;
                }

                if ((letter.HtmlBody != null) && (letter.HtmlBody.Length > 5))
                {
                    return letter.HtmlBody;
                }

                if ((letter.Body != null))
                {
                    return letter.GetTextBody(MimeKit.Text.TextFormat.Text);
                }
            }
            catch (Exception ex)
            {
                return "No body";
            }
            return "No body";
        }
    }
}
