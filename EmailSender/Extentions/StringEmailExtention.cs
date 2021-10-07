

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmailSender.Extentions
{
    public static class StringEmailExtention
    {
        /// <summary>
        /// Exstract  emal address from text 
        /// </summary>
        /// <param name="text"></param>
        /// <returns>email address or empty string</returns>
        public static string ExstractEmailFromText(this string text)
        {
            var exstractor = new Regex(@"[a-zA-Z0-9+._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+");
            var match = exstractor.Match(text);
            if (match.Success)
            {
                return match.Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get simple email address
        /// </summary>
        /// <returns>email addres in format name@domen</returns>
        public static string GenerateEmail()
        {
            var random = new Random();

            var mailDomens = new List<string>
            {
                "gmail.com","protonmail.com","mail.com","onet.pl","rambler.com","aol.com","zoho.com","yahoo.com","gmx.com"
            };

            var name = GetLogin(5, 10);
            string result = name + "@" + mailDomens.ElementAt(random.Next(mailDomens.Count));
            return result;
        }

        public static string GetPass(int minLength, int maxLength)
        {
            Random rnd = new Random();
            int sizePass = rnd.Next(minLength, maxLength);
            string rc = "qwertyuiopasdfghjklzxcvbnm0123456789QWERTYUIOPASDFGHJKLZXCVBNM_*";
            char[] letters = rc.ToCharArray();
            string pass = "";

            for (int i = 0; i < sizePass; i++)
            {
                pass += letters[rnd.Next(0, rc.Length)].ToString();
            }
            return pass;
        }

        public static string GetLogin(int minLength, int maxLength)
        {
            Random rnd = new Random();
            int sizePass = rnd.Next(minLength, maxLength);
            string rc = "qwertyuiopasdfghjklzxcvbnm";
            char[] letters = rc.ToCharArray();

            string pass = "";

            for (int i = 0; i < sizePass; i++)
            {
                pass += letters[rnd.Next(0, rc.Length)].ToString();
            }
            //add some didjets to word
            if (rnd.Next(10) > 7)
            {
                pass += rnd.Next(85,99).ToString();
            }

            return pass;
        }

    } 
}
