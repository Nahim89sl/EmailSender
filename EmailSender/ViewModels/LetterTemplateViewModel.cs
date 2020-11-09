using EmailSender.Models;
using EmailSender.Settings;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.ViewModels
{
    public class LetterTemplateViewModel : PropertyChangedBase
    {
        private Letter _templateLetter;
        public LetterTemplateViewModel(IContainer ioc)
        {
            _templateLetter = ioc.Get<AppSettingsModel>().LetterTemplate;
        }

        private string _subject;
        public string Subject
        {
            get
            {
                _subject = _templateLetter.Subject;
                return _subject;
            }
            set
            {
                SetAndNotify(ref this._subject, value);
                _templateLetter.Subject = _subject;
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                _text = _templateLetter.Text;
                return _text;
            }
            set
            {
                SetAndNotify(ref this._text, value);
                _templateLetter.Text = _text;
            }
        }

    }
}
