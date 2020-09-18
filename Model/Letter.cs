using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Model
{
    public class Letter : ObservableObject
    {
		private string subject;
		public string Subject
		{
			get { return subject; }
			set { Set(()=>Subject,ref subject,value); }
		}

		private string text;
		public string Text
		{
			get { return text; }
			set { Set(() => Text, ref text, value); }
		}

        private string emailSender;
        public string EmailSender
        {
            get { return emailSender; }
            set { Set(()=>EmailSender,ref emailSender,value); }
        }

        private int id;
        public int Id
        {
            get { return id; }
            set { Set(() => Id, ref id, value); }
        }
    }
}
