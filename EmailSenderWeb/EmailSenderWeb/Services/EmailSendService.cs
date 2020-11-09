using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace EmailReaderWeb
{
    public class EmailSendService
    {
        private readonly HttpClient client;
        CookieContainer cookie;
        HttpClientHandler handler;
        private const string GoodState = "ok";


        public EmailSendService()
        {
            handler = new HttpClientHandler();
            cookie = new CookieContainer();
            handler.CookieContainer = cookie;
            handler.AllowAutoRedirect = true;
            handler.UseCookies = true;
            client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(60);
        }

        public async Task Authentification(ServerAccount mailServer)
        {
            mailServer.ServerStatus = GoodState;
            string token = "";
            HttpResponseMessage response;
            //#1 try connect to server
            try
            {
                //go to login page
                response = await client.GetAsync("http://" + mailServer.Server + "/webmail/");
                var responseContent = await response.Content.ReadAsStringAsync();
                //try get token from answer
                token = GetToken(responseContent);
                if (token == "")
                {
                    var err = "Can't find token in server answer. Work with server unpossible";
                    mailServer.ServerStatus = err;
                    throw new Exception(err);
                }                
            }
            catch (Exception ex)
            {
                mailServer.ServerStatus = ex.Message;
                throw ex;
            }
            //#2 try authentificate user on server
            try
            { 
                //set auth param for url 
                var data = new StringContent(
                            $"_token={token}&_task=login&_action=login&_timezone=Europe%2FMoscow&_url=" +
                            $"&_user={HttpUtility.UrlEncode(mailServer.Login)}" + 
                            $"&_pass={HttpUtility.UrlEncode(mailServer.Pass)}",
                            Encoding.UTF8, 
                            "application/x-www-form-urlencoded"
                            );
                //auth request
                response = await client.PostAsync($"http://{mailServer.Server}/webmail/?_task=login", data);
                var responseContent = await response.Content.ReadAsStringAsync();
                //check success authorization 
                if (responseContent.Contains("mbox=INBOX"))
                {
                    mailServer.AccountStatus = GoodState;
                    return;
                }
                //try understand what happand with authorization
                CheckAuthorizStatus(responseContent, response, mailServer);
            }
            catch (Exception ex)
            {
                mailServer.AccountStatus = ex.Message;
                throw ex;
            }

        }
        
        public async Task SendAsync(ServerAccount mailServer, string receiver, string hiddenReceiver, string subject, string mailText)
        {
            //go to compose mail
            var response = await client.GetAsync($"http://{mailServer.Server}/webmail/?_task=mail&_action=compose");
            var responseContent = await response.Content.ReadAsStringAsync();

            //check auth
            //session can live only limit time and sometimes we should make authorization again
            if (responseContent.Contains("task=login"))
            {
                
                await Authentification(mailServer);

                response = await client.GetAsync($"http://{mailServer.Server}/webmail/?_task=mail&_action=compose");
                responseContent = await response.Content.ReadAsStringAsync();
                if(responseContent.Contains("task=login"))
                {
                    mailServer.ServerStatus = $"Can't compose letter status code {response.StatusCode.ToString()}";
                    throw new Exception(mailServer.ServerStatus);
                }
            }


            //Set all parametrs to post request
            string id = GetId(responseContent);
            string token = GetToken(responseContent);

            StringContent fieldsData = new StringContent(
                        $"_token={token}" +
                        $"&_task=mail&_action=send&_id={id}" +
                        $"&_attachments=&_from=1&_to={receiver}" +
                        $"&_cc={hiddenReceiver}" + 
                        $"&_bcc={hiddenReceiver}" +  
                        "&_replyto=&_followupto=" +
                        $"&_subject={subject}" + 
                        "&editorSelector=plain&_priority=0&_store_target=Sent&_draft_saveid=&_draft=&_is_html=0&_framed=1" +
                        $"&_message={mailText}",
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded");


            // send  requiest
            response = await client.PostAsync("http://" + mailServer.Server + "/webmail/?_task=mail&_unlock=loading" + TUnix.Timestamp().ToString() + "&_framed=1&_lang=en_US", fieldsData);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                mailServer.ServerStatus = $"Sending error to email: {receiver}, server answer status:{response.StatusCode.ToString()}"; 
                throw new Exception(mailServer.ServerStatus);
            }
            responseContent = await response.Content.ReadAsStringAsync();
            
            //check success send message from server
            if (responseContent.Contains("sent_successfully"))
            {
                mailServer.ServerStatus = GoodState;
            }
            else
            {
                Regex rgx = new Regex("(?<=display_message\\().*?(?=\\);)");
                var match = rgx.Match(responseContent);
                if (match.Success)
                { 
                    mailServer.ServerStatus =
                                $"Sending error to email: {receiver}, server answer:{match.Value}";
                }
                else
                { 
                    mailServer.ServerStatus = $"Sending error to email: {receiver}";
                }
                throw new Exception(mailServer.ServerStatus);
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

        private void CheckAuthorizStatus(string responseContent, HttpResponseMessage response, ServerAccount account)
        {
            //wrong credy
            if (responseContent.Contains("task=login"))
            {                               
                throw new Exception("Can't authorize: wrong login or password");
            }
            //server's error
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {                
                throw new Exception("Server's internal error");
            }
            //unknown problem
            throw new Exception(response.StatusCode.ToString());
        }

    }
}
