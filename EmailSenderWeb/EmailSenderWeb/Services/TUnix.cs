using System;

namespace EmailReaderWeb
{
    public static class TUnix
    {
        public static int Timestamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static string TimeStampToStr(int timestamp)
        {
            var firstDate = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            firstDate = firstDate.AddSeconds(timestamp).ToLocalTime();
            return firstDate.ToString("dd.MM.yyyy HH:mm");
        }

    }
}
