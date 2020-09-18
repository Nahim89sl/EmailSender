using EmailSender.Interfaces;
using EmailSender.Model;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace EmailSender.Services
{
    class HttpSendService : ISenderService
    {
        private HttpClient client;
        private bool auth;
        private Logger logger;
        CookieContainer cookie;
        HttpClientHandler handler;
        private HttpResponseMessage response;

        public HttpSendService()
        {
            handler = new HttpClientHandler();           
            cookie = new CookieContainer();
            handler.CookieContainer = cookie;
            handler.AllowAutoRedirect = true;
            handler.UseCookies = true;
            client = new HttpClient(handler);            
            auth = false;
            logger = LogManager.GetCurrentClassLogger();
        }

        public bool ValidateReceiver(Receiver receiver)
        {
            try
            {
                string address = new MailAddress(receiver.Email).Address;
                return true;
            }
            catch (FormatException)
            {
                receiver.StatusEmailExist="Wrong Email";
                receiver.StatusSend = "Wrong Email";
            }
            return false;
        }

        public async Task<bool> Authentification(Akkaunt mailServer)
        {
            response = await client.GetAsync("http://" + mailServer.Domen + "/webmail/");
            var responseContent = await response.Content.ReadAsStringAsync();

            var token = GetToken(responseContent);
            if (token != "")
            {
               var data = new StringContent("_token=" + token + "&_task=login&_action=login&_timezone=Europe%2FMoscow&_url=&_user=" + HttpUtility.UrlEncode(mailServer.Login) + "&_pass=" + HttpUtility.UrlEncode(mailServer.Pass), 
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded");
                  
               response = await client.PostAsync("http://" + mailServer.Domen + "/webmail/?_task=login", data);
               responseContent = await response.Content.ReadAsStringAsync();

                if ((responseContent.Contains("task=login"))|| (response.StatusCode == HttpStatusCode.Unauthorized)|| response.StatusCode ==HttpStatusCode.InternalServerError)
                {
                    Debug.Print("not auth");
                    logger.Error("Auth error");
                    mailServer.Status = "Не авторизовались!";
                    auth = false;
                    return auth;
                }
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        logger.Info("Can autorise akk");
                        mailServer.Status = "Авторизовались";
                        auth = true;
                        return auth;
                    }
            }
            
            return false;
   
        }
  
        public async Task SendAsync(Akkaunt mailServer, Receiver receiver, string subject, string mailText)
        {
            if ((receiver.Email == "") || (subject == "") || (mailText == ""))
            {
                logger.Error("wrong input parameters"); 
            }
            
            //responce for compose letter                       
            response = await client.GetAsync("http://" + mailServer.Domen + "/webmail/?_task=mail&_action=compose");
                                                                                
            var responseContent = await response.Content.ReadAsStringAsync();
            //check auth
            if (responseContent.Contains("task=login"))
            {
                auth = await Authentification(mailServer);
                response = await client.GetAsync("http://" + mailServer.Domen + "/webmail/?_task=mail&_action=compose");
                responseContent = await response.Content.ReadAsStringAsync();
            }
            if(auth)
            { 
                

                if (!responseContent.Contains("task=login"))
                {
                    string id = GetId(responseContent);
                    string token = GetToken(responseContent);

                        StringContent fieldsData = new StringContent("_token=" + token + 
                            "&_task=mail&_action=send&_id="+ id + 
                            "&_attachments=&_from=1&_to=" + receiver.Email + 
                            "&_cc="+receiver.CC+
                            "&_bcc=" +receiver.Bcc+ 
                            "&_replyto=&_followupto=&_subject=" + subject + 
                            "&editorSelector=plain&_priority=0&_store_target=Sent&_draft_saveid=&_draft=&_is_html=0&_framed=1&_message=" + mailText,
                            Encoding.UTF8, 
                            "application/x-www-form-urlencoded");


                    // send mail requiest
                    response = await client.PostAsync("http://" + mailServer.Domen + "/webmail/?_task=mail&_unlock=loading" + TUnix.Timestamp().ToString() + "&_framed=1&_lang=en_US",fieldsData);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Debug.Print("status code" + response.StatusCode.ToString());
                    }
                    responseContent = await response.Content.ReadAsStringAsync();
                    if(responseContent.Contains("sent_successfully"))
                    {
                        logger.Info("Send successfully to "+receiver.Email);
                    }
                    else
                    {
                        Regex rgx = new Regex("(?<=display_message\\().*?(?=\\);)");
                        var match = rgx.Match(responseContent);
                        if(match.Success)
                        {
                            logger.Error("Send propblem: "+match.Value);
                            mailServer.Status = "Send propblem: " + match.Value;
                        }
                        else
                        {
                            logger.Error("Send propblem: " + responseContent);
                            mailServer.Status = "Server propblem: " + match.Value;
                        }
                    }
                }
                else
                {
                    logger.Error("Auth error "+ response.StatusCode.ToString());
                    mailServer.Status = "Auth error";
                }              
            }
        }

        private string GetId(string textPage)
        {
            var regex = new Regex("(?<=_id\" value=\").*?(?=\">)");
            var match = regex.Match(textPage);
            if (match.Success)
            {
                return match.Value;
            }
            return "";
        }

        private string GetToken(string textPage)
        {
            var regex = new Regex("(?<=_token\" value=\").*?(?=\">)");
            var match = regex.Match(textPage);
            if (match.Success)
            {
                return match.Value;
            }
            return "";
        }



    }
}
