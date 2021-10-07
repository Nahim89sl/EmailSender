using NUnit.Framework;
using ReaderMails;

namespace ReaderMailTests
{
    [TestFixture]
    public class EmailFiltratorTests
    {
        [Test]
        public void CreteObjetWithNullParams()
        {
            //arrange

            //act
            var obj = new EmailFiltrator();
            //assers
            Assert.IsInstanceOf<EmailFiltrator>(obj);
        }
    }
}
