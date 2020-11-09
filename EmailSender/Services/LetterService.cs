using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using EmailSender.Models;

namespace EmailSender.Services
{
    public static class LetterService
    {
        public static Letter Create(Receiver receiver, Letter letter)
        {
            var resLetter = new Letter();
            resLetter.Subject = TextConverter(receiver, letter.Subject);
            resLetter.Text = TextConverter(receiver, letter.Text);
            if ((resLetter.Subject.Length < 3) || (resLetter.Text.Length < 3))
            {
                return null;
            }

            return resLetter;
        }

        private static string TextConverter(Receiver receiver, string text)
        {
            //randomaze string expressin in text
            string regExpression = @"{.*?}";
            Random rnd = new Random();
            var regex = new Regex(regExpression);
            var matches = regex.Matches(text);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    var strValues = match.Value.Split('|');
                    string rndString = strValues[rnd.Next(strValues.Length)].Replace("{", "").Replace("}", "");
                    text = text.Replace(match.Value, rndString);
                }
            }

            //replase main field in text
            text = text.Replace("[Email]", receiver.Email);
            text = text.Replace("[Company]", receiver.CompanyName);
            text = text.Replace("[PersonName]", receiver.PersonName);

            text = text.Replace("[Inn]", receiver.FieldInn);
            text = text.Replace("[Okvd]", receiver.FieldOkvd);
            text = text.Replace("[Phone]", receiver.FieldPhone);
            text = text.Replace("[ContractAmount]", receiver.FieldContractAmount);
            text = text.Replace("[Address]", receiver.FieldAddress);
            text = text.Replace("[Date1]", receiver.FieldDate1);
            text = text.Replace("[Date2]", receiver.FieldDate1);
            text = text.Replace("[Date3]", receiver.FieldDate1);
            text = text.Replace("[Record1]", receiver.FieldRecord1);
            text = text.Replace("[Record2]", receiver.FieldRecord2);
            text = text.Replace("[Record3]", receiver.FieldRecord3);

            return text;
        }
    }
}
