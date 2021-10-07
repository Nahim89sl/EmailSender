

using EmailSender.Logger;
using EmailSender.Services;
using EmailSender.Settings;
using Moq;
using NUnit.Framework;
using StyletIoC;

namespace EmailSenderTests
{
    [TestFixture]
    public class ReporterTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var mockIoc = new Mock<IContainer>();
            var mockLogger = new Mock<ILogger>();

            mockIoc.Setup(x => x.Get<ILogger>("")).Returns(mockLogger.Object);




            var reporter = new Reporter(mockIoc.Object);
        }
        
        [Test]
        public void WorkWithResults_emptyCollection_work_as_aspected()
        {
            //arrange
            var mockIoc = new Mock<IContainer>();
            var mockLogger = new Mock<ILogger>();


            mockIoc.Setup(x => x.Get<ILogger>("")).Returns(mockLogger.Object);

            var reporter = new Reporter(mockIoc.Object);


            //act
            //assert

        }
    }
}
