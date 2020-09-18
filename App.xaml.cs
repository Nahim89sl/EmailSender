using CommonServiceLocator;
using EmailSender.Interfaces;
using EmailSender.Services;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Unity.ServiceLocation;

namespace EmailSender
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
            this.Exit += new ExitEventHandler(App_exit);
            var container = new UnityContainer();
            container.RegisterType<IFileService, ExelFileService>();
            container.RegisterType<IDialogService, DefaultDialogService>();
            container.RegisterType<ITextWorker, TextWorkerService>();
            container.RegisterType<ISenderService , HttpSendService >();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<IOurMails, OurMailsTxtService>();
            container.RegisterType<ILoadRanges, RangesTxt>();
            container.RegisterType<IValidate, ValidateService>();
            container.RegisterType<IReaderLetter, ReaderMailKitService>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            SimpleIoc.Default.Register<ILoadRanges, RangesTxt>();

        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            Debug.WriteLine("----> Startup event");
        }

        void App_exit(object sender,ExitEventArgs e)
        {
            


            Debug.WriteLine("----> Exit event");            
        }
        

    }
}
