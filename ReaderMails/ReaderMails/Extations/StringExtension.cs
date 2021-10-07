using System;
using System.Text.RegularExpressions;

namespace ReaderMails.Extations
{
    public static class StringExtension
    {
        /// <summary>
        /// Find in text some words from our list. List conatains words in format word1|word2|...
        /// </summary>
        public static string ExistWords(this string text, string findWords)
        {
            Regex rgx = new Regex(findWords);
            var match = rgx.Match(text);
            if (match.Success)
            {
                return match.Value;
            }
            return string.Empty;
        }

        public static bool NotNull(this string text)
        {
            if (text == null)
                return false;
            if (text == string.Empty)
                return false;
            return true;
        }



        /// <summary>
        /// Rundomize text 
        /// Random varian of text we will take from string
        /// diffrent varian must be in next example {1 varian fo text|2 variant of text| ect}
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RandomizeText(this string text)
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
            return text;
        }
    }
}
