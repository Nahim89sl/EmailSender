using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using EmailReaderWeb;
using NUnit.Framework;

namespace EmailSenderWebTests
{
    
    public class SenderWebTests
    {
        private ServerAccount account;
        private SenderWeb sender;
        [SetUp]
        public void Setup()
        {
           sender = new SenderWeb();
            //load setting account of emailbox
            string json = File.ReadAllText("testSettings.json");
            account = JsonSerializer.Deserialize<ServerAccount>(json);

        }

        [Test]
        public async System.Threading.Tasks.Task CheckAuth_RightValueAsync()
        {
            await sender.CheckAuth(account);
            Assert.AreEqual("ok",account.ServerStatus);
        }

        [Test]
        public async System.Threading.Tasks.Task CheckAuth_BadSrvValueAsync()
        {
            ServerAccount badAcc = new ServerAccount()
            {
                Server = "some-bad-url.net"
            };
            try
            {
                await sender.CheckAuth(badAcc);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }
    }
}
