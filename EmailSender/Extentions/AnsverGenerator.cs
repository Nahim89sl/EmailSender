
using MimeKit;
using ReaderMails.Enums;
using ReaderMails.Interfaces;
using ReaderMails.Models;
using System.Collections.Generic;

namespace EmailSender.Extentions
{
    public class AnsverGenerator
    {
        public static List<IMailAnswer> GetAnswers(string goodwords, int goodCount, string needAnswWords, int needAnswCount, string spamWords, int spamCount, string blockWords, int blockCount)
        {
            var answers = new List<IMailAnswer>();

            #region Bad mails

            for (int i = 0; i < blockCount; i++)
            {
                var text = blockWords.Replace("|", " ");
                answers.Add(GenerateAnswer("Block answer", text, MailStatus.Bad));
            }

            for (int i = 0; i < spamCount; i++)
            {
                var text = spamWords.Replace("|", " ");
                answers.Add(GenerateAnswer("Spam answer", text, MailStatus.Spam));
            }

            #endregion

            #region Good mails

            for (int i = 0; i < goodCount; i++)
            {
                var text = goodwords.Replace("|", " ");
                answers.Add(GenerateAnswer("Good answer", text, MailStatus.Good));
            }

            for (int i = 0; i < needAnswCount; i++)
            {
                var text = needAnswWords.Replace("|", " ");
                answers.Add(GenerateAnswer("Need responce", text, MailStatus.Good));
            }

            #endregion

            return answers;
        }       
        
        public static IMailAnswer GenerateAnswer(string subject, string text, MailStatus status)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(StringEmailExtention.GenerateEmail()));
            message.To.Add(MailboxAddress.Parse(StringEmailExtention.GenerateEmail()));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = text
            };

            var answ = new MailAnswer(message, status);
            return answ;
        }        
    }
}
