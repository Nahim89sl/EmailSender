
namespace AppCommon.Interfaces
{
    public interface IConsts
    {
        string TrashFolder { get; set; }
        string ReadFolder { get; set; }

        string ReceiverStatusAnswered { get; set; }
        string ReceiverStatusBlock { get; set; }
        string ReceiverStatusSpam { get; set; }
        string ReceiverStatusNotExist { get; set; }
        string ReceiverStatusWariant { get; set; }
        string ReceiverStatusSended { get; set; }
        string ReceiverStatusNotSend { get; set; }
        string ReceiverStatusWrongEmail { get; set; }

        string ServerStatusOk { get; set; }
        string ServerStatusAuthErr { get; set; }
        string ServerStatusTimeOutErr { get; set; }

        string AkkStatusOk { get; set; }
        string AkkStatusAuthErr { get; set; }
        string AkkStatusBlock { get; set; }

        string DefaultStatusValue { get; set; }
    }
}
