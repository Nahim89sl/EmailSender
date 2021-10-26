using EmailSender.ViewModels;
using Stylet;
using StyletIoC;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using EmailSender.Services;
using ILogger = EmailSender.Logger.ILogger;
using System.Windows;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using System.Windows.Threading;
using AppCommon.Interfaces;
using AppCommon.Constants;
using AppCommon.Utilities;
using EmailSender.Utilities;
using ReaderMails.Interfaces;

namespace EmailSender
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {
        private IContainer ioc;
        private AppSettingsModel settings;

        protected override void OnStart()
        {
            Stylet.Logging.LogManager.Enabled = true;
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);
            // Bind your own types. Concrete types are automatically self-bound.
            
            //get settings service
            builder.Bind<ISettings>().To<AppSettingService>().InSingletonScope();
            builder.Bind<ILogger>().To<NLogLogger>().InSingletonScope();
            //builder.Bind<ILogger>().To<StyletLogger>().InSingletonScope();
            builder.Bind<IConsts>().ToFactory(container => new Consts());
            
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
            builder.Bind<IMailAkk>().ToFactory(container => settings.MainAccount);
            builder.Bind<AppSettingsModel>().ToFactory(container => settings);
            builder.Bind<IReaderMails>().To<ReaderMailsService>().InSingletonScope();
            builder.Bind<IDialogService>().To<DefaultDialogService>();
            builder.Bind<IStatuses>().To<Statuses>();
            //builder.Bind<ILoadReceivers>().To<ReceiverLoaderExel>();
            builder.Bind<ILoadReceivers>().To<LoadSaveReceiversSqlite>();
            builder.Bind<IExcelWorker>().To<ExcelWorker>();
            builder.Bind<IOurReceiversWorker>().To<OurReceiversWorker>();
            builder.Bind<IAutoAnswerService>().To<AutoAnswer>();

            builder.Bind<BindableCollection<Receiver>>().ToFactory(container => new BindableCollection<Receiver>()).InSingletonScope();

            //Utils
            builder.Bind<IEmailExtractor>().To<EmailExtractor>();

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
            //ioc.Get<ISettings>().Save(settings);
            var logger = ioc.Get<ILogger>();
            logger.ErrorReader($"{e.Exception.Message}");
        }

    }
}
