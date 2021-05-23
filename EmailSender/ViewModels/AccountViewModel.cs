using StyletIoC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmailSender.Interfaces;
using EmailSender.Logger;
using EmailSender.Models;
using Stylet;
using AppCommon.Interfaces;

namespace EmailSender.ViewModels
{
    public class AccountViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private ILogger _logger;
        private ISender _sender;
        public IMailAkk MainAccount { get; set; }

        public AccountViewModel(IContainer ioc)
        {
            _logger = ioc.Get<ILogger>();
            _windowManager = ioc.Get<IWindowManager>();
            MainAccount = ioc.Get<IMailAkk>();
            _sender = ioc.Get<ISender>();
            _logger = ioc.Get<ILogger>();
        }
        
        
        //login
        private string _login;
        public string Login
        {
            get
            {
                _login = MainAccount.Login;
                return _login;
            }
            set
            {
                SetAndNotify(ref this._login, value);
                MainAccount.Login = _login;
            }
        }
        //pass
        private string _pass;
        public string Pass
        {
            get
            {
                _pass = MainAccount.Pass;
                return _pass;
            }
            set
            {
                SetAndNotify(ref this._pass, value);
                MainAccount.Pass = _pass;
            }
        }
        //ip / domen
        private string _server;
        public string Server
        {
            get
            {
                _server = MainAccount.Server;
                return _server;
            }
            set
            {
                SetAndNotify(ref this._server, value);
                MainAccount.Server = _server;
            }
        }
        //acc status
        private string _accState;
        public string AccState
        {
            get
            {
                _accState = MainAccount.AccountStatus;
                return _accState;
            }
            set
            {
                SetAndNotify(ref this._accState, value);
                MainAccount.AccountStatus = _accState;
            }
        }
        //server status
        private string _srvState;
        public string SrvState
        {
            get
            {
                _srvState = MainAccount.ServerStatus;
                return _srvState;
            }
            set
            {
                SetAndNotify(ref this._srvState, value);
                MainAccount.ServerStatus = _srvState;
            }
        }

        private string _serverLabelName;

        public string ServerLabelName 
        {
            get
            {                
                return MainAccount.ServerLabelName;
            }
            set
            {
                SetAndNotify(ref this._serverLabelName, value);
                MainAccount.ServerLabelName = _serverLabelName;
            }
        }

        //test receiver address
        public String ReceiverAddress { get; set; }
        public string TestSubject { get; set; }
        public string TextLetter { get; set; }

        //try authorize account
        public void TestAccountCommand()
        {
            Execute.OnUIThreadAsync(async () =>
             {
                 await _sender.CheckAccount();
                 NotifyOfPropertyChange(nameof(this.AccState));
                 NotifyOfPropertyChange(nameof(this.SrvState));
                 _windowManager.ShowMessageBox("Account tested");
             });
        }

        public void TestSendLetterCommand()
        {
            Execute.OnUIThreadAsync(async () =>
            {
                await _sender.SendEmail(
                    new Receiver() {Email = ReceiverAddress}, new Receiver(),
                    new Letter() {Subject = TestSubject,Text = TextLetter}
                    );
                NotifyOfPropertyChange(nameof(this.AccState));
                NotifyOfPropertyChange(nameof(this.SrvState));
                _windowManager.ShowMessageBox("Email Sended");
            });
        }

    }
}
