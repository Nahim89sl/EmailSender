namespace EmailSender.Interfaces
{
    public interface INotification
    {
        void SendInfoMessage(string info);
        void SendErrorMessage(string info);
        void SendWarningMessage(string info);
        void AnswerGetMessage(string info);
        void FinishSendMessage(string info);
        void ServerErrorMessage(string info);
        void AccountErrorMessage(string info);
    }
}