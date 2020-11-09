

using NUnit.Framework;
using ReaderMails;
using System.IO;
using System.Text.Json;

namespace ReaderMailTests
{
    public class MailKitReaderTests
    {
        private EmailBoxAkkaut acc;
        private MailKitReader reader;
        

        [SetUp]
        public void Setup()
        {
            //load setting account of emailbox
            string json = File.ReadAllText("testSettings.json");
            acc = JsonSerializer.Deserialize<EmailBoxAkkaut>(json);
            reader = new MailKitReader(NLog.LogManager.GetCurrentClassLogger());
        }

        [Test]
        public void ConnectToServer_RightValues()
        {
            reader.ConnectToServer(acc);
            Assert.AreEqual("ok",acc.ServerStatus);
        }

        [Test]
        public void ConnectToServer_Auth_RightValues()
        {
            reader.ConnectToServer(acc);
            Assert.AreEqual("ok", acc.AccountStatus);
        }

        [Test]
        public void ConnectToServer_BadValues()
        {
            var badAcc = new EmailBoxAkkaut(){Server = "some-url.com",Port = 222};
            reader.ConnectToServer(badAcc);
            Assert.AreNotEqual("ok", badAcc.ServerStatus);
        }

        [Test]
        public void ConnectToServer_Auth_BadValues()
        {
            var badAcc = new EmailBoxAkkaut() { Server = acc.Server, Port = acc.Port, Login = "login",Pass = "pass"};
            reader.ConnectToServer(badAcc);
            Assert.AreNotEqual("ok", badAcc.AccountStatus);
        }

        [Test]
        public void ExistStopWords_GoodValue()
        {
            string stopWords = "word1|word2 |word3";
            string searchString = "sdkjfhw lsdf wes word2 lsdfjqwsdf  wed;sfdq";
            Assert.IsTrue(reader.ExistStopWords(searchString, stopWords));
        }

        [Test]
        public void ExistStopWords_BadValue()
        {
            string stopWords = "word1|word2 |word3";
            string searchString = "sdkjfhw lsdf wes word2wes lsdfjqwsdf  wed;sfdq";
            Assert.IsFalse(reader.ExistStopWords(searchString, stopWords));
        }
    }
}
