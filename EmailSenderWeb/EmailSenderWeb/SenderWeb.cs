using System;
using System.Threading.Tasks;
using EmailReaderWeb.Models;

namespace EmailReaderWeb
{
    public class SenderWeb
    {
        private readonly EmailSendService _sendService;

        public SenderWeb()
        {
            _sendService = new EmailSendService();
        }
        
        public async Task CheckAuth(ServerAccount account)
        {
            //if auth will fale this method return exception and write error to account's status field
            try
            {
                await _sendService.Authentification(account);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        //this method can accept null hidden value and convert it to empty string
        public async Task SendMail(ServerAccount account, Receiver receiver, Receiver hiddenReceiver, Letter letter)
        {
            try
            {
                Validator.ChekParams(receiver, hiddenReceiver, letter);
                //if exist some problem we will write errors to status of instance
                await _sendService.SendAsync(account, receiver.EmailAddress, hiddenReceiver?.Status ?? "",
                    letter.Subject, letter.Text);
                //if some error exist it write to server's status
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
