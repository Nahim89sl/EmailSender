using EmailReaderWeb;
using EmailSenderWeb.Services;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EmailSenderTests
{
    public class EmailSmtpServiceTests
    {
        [Test]
        public async Task Check_work_of_service()
        {
            //arrange
            var smtpService = new EmailSmtpSerivce();
            var akk = new ServerAccount();
            akk.Server = "";
            akk.Port = 587;
            akk.Login = "";
            akk.Pass = "";
            //act
            await smtpService.SendAsync(akk, "", "", "Test subject", "Text letter");

            //assert
            Assert.IsTrue(true);
        }
    }
}
