using AppCommon.Interfaces;

namespace AppCommon.Constants
{
    public class Consts : IConsts
    {
        public string TrashFolder { get; set; } = "Trash";
        public string ReadFolder { get; set; } = "Read";

        public string ReceiverStatusAutoanswered { get; set; } = "AUTOANSWERED";
        public string ReceiverStatusAnswered { get; set; } = "ANSWERED";
        public string ReceiverStatusBlock { get; set; } = "BLOCK";
        public string ReceiverStatusSpam { get; set; } = "SPAM";
        public string ReceiverStatusNotExist { get; set; } = "NOT EXIST";
        public string ReceiverStatusWariant { get; set; } = "WARIANT";
        public string ReceiverStatusSended { get; set; } = "SENDED";
        public string ReceiverStatusNotSend { get; set; } = "no";
        public string ReceiverStatusWrongEmail { get; set; } = "Wrong email";

        public string ServerStatusOk { get; set; } = "ok";
        public string ServerStatusAuthErr { get; set; } = "Auth error";
        public string ServerStatusTimeOutErr { get; set; } = "Time out";

        public string AkkStatusOk { get; set; } = "ok";
        public string AkkStatusAuthErr { get; set; } = "Auth error";
        public string AkkStatusBlock { get; set; } = "BLOCK";

        public string DefaultStatusValue { get; set; } = "no";
    }
}
