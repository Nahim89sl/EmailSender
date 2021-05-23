﻿using EmailSender.Logger;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Text;
using EmailSender.Interfaces;
using EmailSender.Models;
using Stylet;
using EmailSender.Settings.Models;
using EmailSender.Settings;
using System.Windows;

namespace EmailSender.ViewModels
{
    public class MainViewModel
    {
        #region private fields 

        private ISettings _settingsService;
        private AppSettingsModel _globalSettings;

        #endregion
        public MainViewModel(IContainer ioc)
        {
            var logger = ioc.Get<ILogger>();
            logger.InfoSender("Start app 2.0.1");           
            ViewAccount = new AccountViewModel(ioc);           
            ViewNotification = new NotificationViewModel(ioc);
            FieldMapping = new FieldMappingViewModel(ioc);
            ViewLetterTemplate = new LetterTemplateViewModel(ioc);
            ViewSender = new SenderViewModel(ioc);
            ViewReader = new ReaderViewModel(ioc);

            _globalSettings = ioc.Get<AppSettingsModel>();
            _settingsService = ioc.Get<ISettings>();
        }

        #region Commands

        public void SaveSettingsCommand()
        {
            _settingsService.Save(_globalSettings);
            MessageBox.Show("Настройки сохранены");
        }

        #endregion

        #region properties
        public SenderViewModel ViewSender { set; get; }
        public AccountViewModel ViewAccount { get; set; }
        public NotificationViewModel ViewNotification { get; set; }
        public ReaderViewModel ViewReader { get; set; }
        public FieldMappingViewModel FieldMapping { get; set; }
        public LetterTemplateViewModel ViewLetterTemplate { get; set; }

        #endregion

    }
}
