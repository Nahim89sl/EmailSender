using ARSoft.Tools.Net.Dns;
using EmailSender.Interfaces;
using EmailSender.Model;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EmailSender.Services
{
    class ValidateService : IValidate
    {

        Logger logger;

        public ValidateService()
        {
            logger = LogManager.GetCurrentClassLogger();
        }



        public async Task StartValidate(ObservableCollection<Receiver> recivers, string validationStatus)
        {
            Random rndd = new Random();
            
            Receiver receiver = recivers.Where(a => a.StatusEmailExist != "exist").FirstOrDefault();
            while (receiver != null)
            //foreach (var receiver in Receivers)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    validationStatus = receiver.Email;
                });

                await ChooseValidator(receiver);              
                Thread.Sleep(rndd.Next(1, 5) * 1000);
                receiver = recivers.Where(a => a.StatusEmailExist == "no").FirstOrDefault();
            }
            logger.Info("Finished validation");
        }
        
        public async Task ChooseValidator(Receiver receiver)
        {
            Regex rgx = new Regex("(?<=@).*");
            var match = rgx.Match(receiver.Email);
            if(match.Success)
            {
                if((match.Value.ToLower() == "mail.ru")||
                    (match.Value.ToLower() == "bk.ru")||
                    (match.Value.ToLower() == "list.ru")||
                    (match.Value.ToLower() == "inbox.ru"))
                { 
                   await MailRu(receiver); 
                }
                else
                {
                    if ((match.Value.ToLower() == "yandex.ru") || 
                        (match.Value.ToLower() == "ya.ru")||
                        (match.Value.ToLower() == "google.com")||
                        (match.Value.ToLower() == "protonmail.com")||
                        (match.Value.ToLower() == "rambler.ru")
                        )
                    {
                        //await YaRu(receiver); 
                        receiver.StatusEmailExist = "exist";
                    }
                    else
                    {
                        await Other(receiver);
                    }
                }
                

            }
        }

        public async Task Gmail(Receiver receiver)
        {

        }

        public async Task MailRu(Receiver receiver)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            handler.UseCookies = true;

            HttpClient client = new HttpClient(handler);
            var responce = await client.GetStringAsync("https://e.mail.ru/cgi-bin/passremind");

            var pais = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("email" , receiver.Email),
                        new KeyValuePair<string,string>("tab-time" , TUnix.Timestamp().ToString()),
                        new KeyValuePair<string,string>("htmlencoded" , "false"),
                        new KeyValuePair<string,string>("api" , "1"),
                    };

            var content = new FormUrlEncodedContent(pais);
            var res = await client.PostAsync("https://e.mail.ru/api/v1/user/password/restore", content);
            responce = await res.Content.ReadAsStringAsync();

            lock (receiver)
            {
                if (responce.Contains("full_phone"))
                {
                    receiver.StatusEmailExist = "exist";
                }
                else
                {
                    receiver.StatusEmailExist = "unknown";
                }
            }            
        }

        public async Task Other(Receiver receiver)
        {
            Regex rgx = new Regex("(?<=@).*");
            var match = rgx.Match(receiver.Email);
            if (match.Success)
            {
                var resolver = new DnsStubResolver();
                var records = resolver.Resolve<MxRecord>(match.Value, RecordType.Mx);
                if(records.Count>0)
                {
                    receiver.StatusEmailExist = "exist";
                }
                else
                {
                    receiver.StatusEmailExist = "not exist";
                }
            }                
        }

        public async Task YaRu(Receiver receiver)
        {

        }
    }
}
