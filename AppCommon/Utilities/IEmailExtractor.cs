using System.Collections.Generic;

namespace AppCommon.Utilities
{
    public interface IEmailExtractor
    {
        IList<string> ExtractAddress(string sourceText);
    }
}
