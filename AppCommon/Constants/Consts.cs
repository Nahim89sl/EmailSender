using AppCommon.Interfaces;

namespace AppCommon.Constants
{
    public class Consts : IConsts
    {
        public string TrashFolder { get; set; } = "Trash";
        public string ReadFolder { get; set; } = "Read";

        public string ReceiverStatusAnswered { get; set; } = "ANSWERED";
        public string ReceiverStatusBlock { get; set; } = "BLOCK";
        public string ReceiverStatusSpam { get; set; } = "SPAM";
        public string ReceiverStatusNotExist { get; set; } = "SPAM";

        public string ReceiverMailNotExist { get; set; } = "NOT EXIST";

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
