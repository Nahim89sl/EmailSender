
namespace AppCommon.Interfaces
{
    public interface IConsts
    {
        string TrashFolder { get; set; }
        string ReadFolder { get; set; }

        string ReceiverStatusAnswered { get; set; }
        string ReceiverStatusBlock { get; set; }
        string ReceiverStatusSpam { get; set; }
        string ReceiverMailNotExist { get; set; }

    }
}
