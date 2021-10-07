
namespace ReaderMails.Interfaces
{
    public interface IMailAkk
    {
        string Protocol { get; set; }
        string ServerStatus { get; set; }
        string Server { get; set; }
        string Login { get; set; }
        string Pass { get; set; }
        int Port { get; set; }
        string AccountStatus { get; set; }
        bool ReaderOnStart { get; set; }
        string ServerLabelName { get; set; }
    }
}
