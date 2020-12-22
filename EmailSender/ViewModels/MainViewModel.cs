using EmailSender.Logger;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Text;
using EmailSender.Interfaces;
using EmailSender.Models;
using Stylet;
using EmailSender.Settings.Models;

namespace EmailSender.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(IContainer ioc)
        {
            var logger = ioc.Get<ILogger>();
            logger.InfoSender("Start app 2.0");           
            ViewAccount = new AccountViewModel(ioc);           
            ViewNotification = new NotificationViewModel(ioc);
            FieldMapping = new FieldMappingViewModel(ioc);
            ViewLetterTemplate = new LetterTemplateViewModel(ioc);
            ViewSender = new SenderViewModel(ioc);
            ViewReader = new ReaderViewModel(ioc);
        }
        public SenderViewModel ViewSender { set; get; }
        public AccountViewModel ViewAccount { get; set; }
        public NotificationViewModel ViewNotification { get; set; }
        public ReaderViewModel ViewReader { get; set; }
        public FieldMappingViewModel FieldMapping { get; set; }
        public LetterTemplateViewModel ViewLetterTemplate { get; set; }


    }
}
