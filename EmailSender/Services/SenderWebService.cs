using System;
using System.Threading.Tasks;
using EmailReaderWeb;
using EmailSender.Interfaces;
using EmailSender.Models;


namespace EmailSender.Services
{
    public class SenderWebService : ISender
    {
        private MainAccount _account;
        private SenderWeb _sender;
        private const string StatusSended = "SENDED";

        public SenderWebService(MainAccount account)
        {
            _account = account;
            _sender = new SenderWeb();
        }

        public async Task CheckAccount()
        {
            var akk = new ServerAccount()
                { Login = _account.Login, Pass = _account.Pass, Server = _account.Server };
            try
            {
                await _sender.CheckAuth(akk);
            }
            catch
            {
                _account.AccountStatus = akk.AccountStatus;
                _account.ServerStatus = akk.ServerStatus;
                //throw ex;
                return;
            }
            _account.ServerStatus = akk.ServerStatus;
            _account.AccountStatus = akk.AccountStatus;
        }

        public async Task SendEmail(Models.Receiver receiver, Models.Receiver hiddenReceiver, Letter letter)
        {
            var akk = new ServerAccount(){ Login = _account.Login, Pass = _account.Pass, Server = _account.Server };
            var receiverWeb = new EmailReaderWeb.Receiver() {EmailAddress = receiver.Email};
            var hiddenReceiverWeb =  new EmailReaderWeb.Receiver() { EmailAddress = hiddenReceiver.Email, Status = receiver.Status};
            var letterWeb = new EmailReaderWeb.Models.Letter() {Subject = letter.Subject, Text = letter.Text};
            
            try
            {
                await _sender.SendMail(akk, receiverWeb, hiddenReceiverWeb, letterWeb);
            }
            catch
            {
                _account.AccountStatus = akk.AccountStatus;
                _account.ServerStatus = akk.ServerStatus;
                receiver.Status = akk.ServerStatus;
                //throw ex;
                return;
            }
            receiver.StatusSend = StatusSended;
            receiver.Time = DateTime.Now.ToString("YYYY-MM-dd hh:mm:ss");
        }

    }
}
