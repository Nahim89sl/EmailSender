using EmailSender.ViewModels;
using NLog;
using Stylet;
using Stylet.Logging;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Services;
using ILogger = EmailSender.Logger.ILogger;
using System.Windows;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace EmailSender
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {
        private IContainer ioc;
        private AppSettingsModel settings;

        protected override void OnStart()
        {
           
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);
            // Bind your own types. Concrete types are automatically self-bound.
            
            //get settings service
            builder.Bind<ISettings>().To<AppSettingService>().InSingletonScope();
            builder.Bind<ILogger>().To<NLogLogger>().InSingletonScope();
            //load settings
            ioc = builder.BuildContainer();
            settings = ioc.Get<ISettings>().Load();
            //
            //bind logger service
            
            //bind Notification service with account settings value
            builder.Bind<INotification>().ToFactory(container => new NotificationTelegramService(settings.NotificationAccount, container.Get<ILogger>()));
            builder.Bind<ISender>().ToFactory(container => new SenderWebService(settings.MainAccount));
            //builder.Bind<ISender>().To<SenderTestService>();
            builder.Bind<NotificationAccountSettings>().ToFactory(container => settings.NotificationAccount);
            builder.Bind<MainAccount>().ToFactory(container => settings.MainAccount);
            builder.Bind<AppSettingsModel>().ToFactory(container => settings);
            builder.Bind<IReaderMails>().To<ReaderMailsService>().InSingletonScope();
            builder.Bind<IDialogService>().To<DefaultDialogService>();
            builder.Bind<IStatuses>().To<Statuses>();
            //builder.Bind<ILoadReceivers>().To<ReceiverLoaderExel>();
            builder.Bind<ILoadReceivers>().To<LoadSaveReceiversSqlite>();
            builder.Bind<IExcelWorker>().To<ExcelWorker>();

            builder.Bind<BindableCollection<Receiver>>().ToFactory(container => new BindableCollection<Receiver>()).InSingletonScope();

            ioc =  builder.BuildContainer();

            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ioc = base.Container;
            ioc.Get<ISettings>().Save(settings);
            //var saver = ioc.Get<ILoadReceivers>();
            //var receivers = ioc.Get<BindableCollection<Receiver>>();
            //saver.SaveChanges(receivers, settings.FielMappingSettings);
        }
        
        
        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            ioc = base.Container;
            ioc.Get<ISettings>().Save(settings);
            var logger = ioc.Get<ILogger>();
            logger.ErrorReader($"{e.Exception.Message}");
        }

    }
}
