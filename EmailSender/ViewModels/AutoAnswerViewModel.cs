using EmailSender.Extentions;
using EmailSender.Models;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using Stylet;
using StyletIoC;

namespace EmailSender.ViewModels
{
    public class AutoAnswerViewModel : PropertyChangedBase
    {
        #region Private fields

        private ReaderSettings _settings;
        private string _answerTitleFilter;
        private string _answerBodyFilter;
        private string _answerLetterTitle;
        private string _answerLetterBody;



        #endregion

        #region Constructor

        public AutoAnswerViewModel(IContainer ioc)
        {
            _settings = ioc.Get<AppSettingsModel>().ReaderSettings;
            if (_settings.AnswerLetter == null) _settings.AnswerLetter = new Letter();
        }

        #endregion

        #region Properties       

        public string AnswerTitleFilter
        {
            get
            {
                _answerTitleFilter = _settings.AnswerTitleList;
                return _answerTitleFilter;
            }
            set
            {
                SetAndNotify(ref _answerTitleFilter, value);
                _settings.AnswerTitleList = value;
            }
        }

        public string AnswerBodyFilter
        {
            get
            {
                _answerBodyFilter = _settings.AnswerBodyList;
                return _answerBodyFilter;
            }
            set
            {
                SetAndNotify(ref _answerBodyFilter, value);
                _settings.AnswerBodyList = value;
            }
        }

        public string AnswerLetterTitle
        {
            get
            {
                _answerLetterTitle = _settings.AnswerLetter.Subject;
                return _answerLetterTitle;
            }
            set
            {
                SetAndNotify(ref _answerLetterTitle, value);
                _settings.AnswerLetter.Subject = value;
            }
        }

        public string AnswerLetterBody
        {
            get
            {
                _answerLetterBody = _settings.AnswerLetter.Text;
                return _answerLetterBody;
            }
            set
            {
                SetAndNotify(ref _answerLetterBody, value);
                _settings.AnswerLetter.Text = value;
            }
        }

        #endregion

        #region Commands

        

        #endregion
    }
}
