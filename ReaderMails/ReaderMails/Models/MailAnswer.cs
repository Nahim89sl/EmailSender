using MailKit;
using MimeKit;
using ReaderMails.Enums;
using ReaderMails.Interfaces;
using System.Linq;

namespace ReaderMails.Models
{
    /// <summary>
    /// Class MailAnswer is the class for working with answer
    /// </summary>
    public class MailAnswer : IMailAnswer
    {
        #region Constructor

        public MailAnswer(MimeMessage mail, MailStatus status = MailStatus.Unknown)
        {
            Message = mail;
            Status = status;
        }

        #endregion

        #region Properties

        public MimeMessage Message { get; set; }
        public MailStatus Status { get; set; }
        public UniqueId Id { get; set; }

        public string From => GetFrom();
        public string EmailAddress => GetAddress();
        public string EmailSubject => Message?.Subject?.ToString() ?? string.Empty;
        public string EmailText => GetLetterText();

        
        #endregion

        #region Private Methods

        private string GetLetterText()
        {
            string noBody = "No body";
            if (Message == null)
                return "No body";
            try
            {
                if ((Message.TextBody != null) && (Message.TextBody.Length > 5))
                {
                    return Message.TextBody;
                }

                if ((Message.HtmlBody != null) && (Message.HtmlBody.Length > 5))
                {
                    return Message.HtmlBody;
                }

                if ((Message.Body != null))
                {
                    return Message.GetTextBody(MimeKit.Text.TextFormat.Text) ?? noBody;
                }
            }
            catch
            {
                return noBody;
            }
            return noBody;
        }

        private string GetFrom()
        {
            if (Message.From.Any())
            {
                return Message.From.First().ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetAddress()
        {
            if (Message.From.Any())
            {
                return Message.From.Mailboxes.First().Address.ToLower();
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
