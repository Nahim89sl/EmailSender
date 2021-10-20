using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EmailSender.Interfaces;
using EmailSender.Models;

namespace EmailSender.Services
{
    public class SendProcess
    {
        private ObservableCollection<Receiver> receivers;
        private ObservableCollection<Receiver> ourReceivers;
        private ISender sender;
        private string runStatus;
        private int afterHidden;
        private int afterOurSend;
        private Letter templateLetter;


        public async Task Sending()
        {
            int beforeHidden = 0;
            int beforeOurSend = 0;
            Receiver hiddenReceiver = null;

            await Task.Run(() =>
            {
                foreach (var receiver in receivers)
                {
                    try
                    {
                        //create letter
                        var letter = LetterService.Create(receiver, templateLetter);
                        if (letter != null)
                        {
                            //try send letter
                            sender.SendEmail(receiver, hiddenReceiver, letter);
                            beforeHidden++;
                            beforeOurSend++;
                            hiddenReceiver = null;
                        }
                        else
                        {
                            receiver.Status = "Letter format error";
                        }
                        
                        //cheks if we need send to our email
                        if (beforeOurSend > afterOurSend)
                        {
                            var ourEmail = ourReceivers.OrderBy(a => a.Time).FirstOrDefault();
                            sender.SendEmail(receiver, hiddenReceiver, letter);
                            beforeOurSend = 0;
                        }
                        
                        //check if we need send hidden copy to our email
                        if (beforeHidden > afterHidden)
                        {
                            hiddenReceiver = ourReceivers.OrderBy(a => a.Time).FirstOrDefault();
                        }

                    }
                    catch (Exception ex)
                    {
                        //write to log
                        break;
                    }
                }
            });
            //do somethig if nessasary repeat sending
        }




    }
}
