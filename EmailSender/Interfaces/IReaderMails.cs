using ReaderMails;


namespace EmailSender.Interfaces
{
    public interface IReaderMails
    {
        void ReadMails(EmailFiltrator filtrator);
    }
}