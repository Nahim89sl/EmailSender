using EmailSender.Interfaces;

namespace EmailSender.Models
{
    public class Statuses : IStatuses
    {
        public string mailSended => "SENDED";

        public string mailAnswer => "ANSWER";

        public string mailNotSend => "no";

        public string mailWrongFormat => "Wrong email";

        public string mailNotExist => "NOT EXIST";

        public string serverOk => "ok";

        public string serverAuthErr => "Auth error";

        public string serverTimeOutErr => "Time out";

        public string accOk => "ok";

        public string accBlock => "BLOCK";

        public string accAuthErr => "ERROR";

        public string defaultValue => "no";

        public string emptyField => string.Empty;
    }
}
