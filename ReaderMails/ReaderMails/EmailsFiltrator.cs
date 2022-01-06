using ReaderMails.Enums;
using ReaderMails.Extations;
using ReaderMails.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ReaderMails
{
    public class EmailFiltrator
    {

        #region Private Fields

        private IEnumerable<IMailAnswer> _answers;

        //bad words - uncotegories words for delete
        private string _filterBadWordsTitle;
        private string _filterBadWordsBody;

        //spam words for action with spam signal
        private string _filterSpamWordsTitle;
        private string _filterSpamWordsBody;

        //unexist words for action with unexist email addreses
        private string _filterUnexistWordsTitle1;
        private string _filterUnexistWordsBody1;

        //unexist words for action with unexist email addreses
        private string _filterUnexistWordsTitle2;
        private string _filterUnexistWordsBody2;

        //Answer words for action with emails that need answer
        private string _filterAnswerWordsTitle;
        private string _filterAnswerWordsBody;

        //black list mailaddress words
        private string _filterBadEmails;

        #endregion

        #region Constructor

        public EmailFiltrator(string filterBadWordsTitle = null, string filterBadWordsBody = null, 
                              string filterSpamWordsTitle = null, string filterSpamWordsBody = null,
                              string filterUnexistWordsTitle1 = null, string filterUnexistWordsBody1 = null,
                              string filterUnexistWordsTitle2 = null, string filterUnexistWordsBody2 = null,
                              string filterAnswerWordsTitle = null, string filterAnswerWordsBody = null,
                              string filterBadEmails = null)
        {
            _filterBadWordsTitle = filterBadWordsTitle;
            _filterBadWordsBody = filterBadWordsBody;
            _filterSpamWordsTitle = filterSpamWordsTitle;
            _filterSpamWordsBody = filterSpamWordsBody;
            _filterUnexistWordsTitle1 = filterUnexistWordsTitle1;
            _filterUnexistWordsBody1 = filterUnexistWordsBody1;
            _filterUnexistWordsTitle2 = filterUnexistWordsTitle2;
            _filterUnexistWordsBody2 = filterUnexistWordsBody2;
            _filterAnswerWordsTitle = filterAnswerWordsTitle;
            _filterAnswerWordsBody = filterAnswerWordsBody;
            _filterBadEmails = filterBadEmails;
        }

        #endregion

        #region Properties
        
        public IEnumerable<IMailAnswer> GetMailsForDelete { get => _answers?.Where(x => x.Status != MailStatus.Good).ToList(); }
        public IEnumerable<IMailAnswer> GetMailsForSave { get => _answers?.Where(x => x.Status == MailStatus.Good).ToList(); }
        
        public IEnumerable<IMailAnswer> GetSpamMails { get => _answers?.Where(x => x.Status == MailStatus.Spam).ToList(); }
        public IEnumerable<IMailAnswer> GetAnswerMails { get => _answers?.Where(x => x.Status == MailStatus.AutoAnswer).ToList(); }
        public IEnumerable<IMailAnswer> GetUnexist1Mails { get => _answers?.Where(x => x.Status == MailStatus.Unexist1).ToList(); }
        public IEnumerable<IMailAnswer> GetUnexist2Mails { get => _answers?.Where(x => x.Status == MailStatus.Unexist2).ToList(); }
        public IEnumerable<IMailAnswer> GetBadMails { get => _answers?.Where(x => x.Status == MailStatus.Bad).ToList(); }
        public IEnumerable<IMailAnswer> GetBlackListMails { get => _answers?.Where(x => x.Status == MailStatus.BalckList).ToList(); }

        #endregion

        #region Public Methods
        
        public bool Filter(IEnumerable<IMailAnswer> answers)
        {
            if(answers == null)
                return false;

            foreach (var answer in answers)
            {
                //bad mails
                if (_filterBadWordsTitle.NotNull())
                {
                    if (answer.EmailSubject?.ExistWords(_filterBadWordsTitle) != string.Empty)
                        answer.Status = MailStatus.Bad;
                }
                if (_filterBadWordsBody.NotNull())
                {
                    if (answer.EmailText?.ExistWords(_filterBadWordsBody) != string.Empty)
                        answer.Status = MailStatus.Bad;
                }
                //spam mails
                if (_filterSpamWordsTitle.NotNull())
                {
                    if (answer.EmailSubject?.ExistWords(_filterSpamWordsTitle) != string.Empty)
                        answer.Status = MailStatus.Spam;
                }
                if (_filterSpamWordsTitle.NotNull())
                {
                    if (answer.EmailText?.ExistWords(_filterSpamWordsBody) != string.Empty)
                        answer.Status = MailStatus.Spam;
                }
                //unexist1
                if (_filterUnexistWordsTitle1.NotNull())
                {
                    if (answer.EmailSubject?.ExistWords(_filterUnexistWordsTitle1) != string.Empty)
                        answer.Status = MailStatus.Unexist1;
                }
                if (_filterUnexistWordsBody1.NotNull())
                {
                    if (answer.EmailText?.ExistWords(_filterUnexistWordsBody1) != string.Empty)
                        answer.Status = MailStatus.Unexist1;
                }
                //unexist2
                if (_filterUnexistWordsTitle2.NotNull())
                {
                    if (answer.EmailSubject?.ExistWords(_filterUnexistWordsTitle2) != string.Empty)
                        answer.Status = MailStatus.Unexist2;
                }
                if (_filterUnexistWordsBody2.NotNull())
                {
                    if (answer.EmailText?.ExistWords(_filterUnexistWordsBody2) != string.Empty)
                        answer.Status = MailStatus.Unexist2;
                }
                //auto answer
                if (_filterAnswerWordsTitle.NotNull())
                {
                    if (answer.EmailSubject?.ExistWords(_filterAnswerWordsTitle) != string.Empty)
                        answer.Status = MailStatus.AutoAnswer;
                }
                if (_filterAnswerWordsBody.NotNull())
                {
                    if (answer.EmailText?.ExistWords(_filterAnswerWordsBody) != string.Empty)
                        answer.Status = MailStatus.AutoAnswer;
                }
                //balck list
                if (_filterBadEmails.NotNull())
                {
                    if (answer.EmailText?.ExistWords(_filterBadEmails) != string.Empty)
                        answer.Status = MailStatus.BalckList;
                }
                //good mails
                if (answer.Status == MailStatus.Unknown)
                    answer.Status = MailStatus.Good;
            }
            _answers = answers;

            return true;
        }

        #endregion

    }
}
