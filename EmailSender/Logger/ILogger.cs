namespace EmailSender.Logger
{
    public interface ILogger
    {
        void Trace(string message);
        void Debug(string message);
        void InfoSender(string message);
        void InfoReader(string message);
        void Warn(string message);
        void ErrorSender(string message);
        void ErrorReader(string message);
        void Error(System.Exception exception, string message = null, params object[] args);
        void Fatal(string message);
        void Fatal(System.Exception exception, string message = null, params object[] args);      
    }
}
