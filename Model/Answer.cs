using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Model
{
    public class Answer :ObservableObject
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

	}
}
