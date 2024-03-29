﻿using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Logger
{
    public class NLogLogger : ILogger
    {
        public readonly NLog.Logger loggerSender;
        public NLog.Logger loggerReader;


        public NLogLogger()
        {
            //create new config for logger
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File 
            var logfile = new NLog.Targets.FileTarget("logfile") {
                FileName = ".\\Logs\\${logger}\\${shortdate}.log",
                Layout = "${longdate}|${logger}|${level: uppercase = true}| ${message}"
            };

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info,  LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
            
            loggerSender = LogManager.GetLogger("Sender");
            loggerReader = LogManager.GetLogger("Reader");            
        }

        public void Trace(string message)
        {
            loggerSender.Trace(message);
        }


        public void Debug(string message)
        {
            loggerSender.Debug("message");
            
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
            loggerReader.Error(message);
        }

        public void Error(Exception exception, string message = null, params object[] args)
        {
            loggerReader.Error(exception, message, args);
        }

        public void Fatal(string message)
        {
            loggerReader.Fatal(message);
        }

        public void Fatal(Exception exception, string message = null, params object[] args)
        {
            loggerReader.Fatal(exception,message,args);
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
            loggerSender.Error(message);
        }

        public void ErrorReader(string message)
        {
            loggerReader.Error(message);
        }
    }
}
