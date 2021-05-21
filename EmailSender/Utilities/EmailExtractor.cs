using AppCommon.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailSender.Utilities
{
    public class EmailExtractor : IEmailExtractor
    {
        public IList<string> ExtractAddress(string sourceText)
        {
            IList<string> resultList = new List<string>();

            if (string.IsNullOrEmpty(sourceText))
                return resultList;

            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                RegexOptions.IgnoreCase);

            //find items that matches with our pattern
            MatchCollection emailMatches = emailRegex.Matches(sourceText);

            foreach (Match emailMatch in emailMatches)
            {
                resultList.Add(emailMatch.Value);
            }

            return resultList;
        }
    }
}
