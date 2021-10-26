using Stylet.Logging;
using System;

namespace EmailSender.Logger
{
    public class StyletLogger : ILogger
    {
        public readonly Stylet.Logging.ILogger loggerSender;
        public readonly Stylet.Logging.ILogger loggerReader;

        public StyletLogger()
        {
            loggerSender = LogManager.GetLogger("Sender");
            loggerReader = LogManager.GetLogger("Reader");
        }

        public void Trace(string message)
        {
            loggerSender.Info(message);
        }


        public void Debug(string message)
        {
            loggerSender.Info("message");
        }

        public void Info(string message)
        {
            loggerSender.Info(message);
        }

        public void Warn(string message)
        {
            loggerSender.Warn(message);
        }

        public void Error(string message)
        {
            loggerReader.Error(null, message);
        }

        public void Error(Exception exception, string message = null, params object[] args)
        {
            loggerReader.Error(exception, message);
        }

        public void Fatal(string message)
        {
            loggerReader.Error(null, message);
        }

        public void Fatal(Exception exception, string message = null, params object[] args)
        {
            loggerReader.Error(exception, message);
        }

        public void InfoSender(string message)
        {
            loggerSender.Info(message);
        }

        public void InfoReader(string message)
        {
            loggerReader.Info(message);
        }

        public void ErrorSender(string message)
        {
            loggerSender.Error(null, message);
        }

        public void ErrorReader(string message)
        {
            loggerReader.Error(null, message);
        }
    }
}
