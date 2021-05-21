using AppCommon.Interfaces;

namespace AppCommon.MailObj
{
    public class MailAkk : IMailAkk
    {
        public string Protocol { get; set; }
        public string ServerStatus { get; set; }
        public string Server { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public int Port { get; set; }
        public string AccountStatus { get; set; }
        public bool ReaderOnStart { get; set; }
        public string ServerLabelName { get; set; }
    }
}
