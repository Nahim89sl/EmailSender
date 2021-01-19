using EmailSender.Logger;
using NUnit.Framework;
using Moq;
using EmailSender.Settings.Models;
using EmailSender.Models;
using System.Collections.ObjectModel;
using EmailSender.Services;

namespace EmailSenderTests
{
    [TestFixture]
    public class PausesServiceTests
    {
       [Test]
       public void GetPause_Empty_intervals()
        {
            //arrange
            var logerMock = new Mock<ILogger>();

            var settings = new SenderSettings()
            {
                ChangeIntTime = 50,
                DopPauseTime = 50,
                DopPauseInterval = new PauseInterval() { Start = 1, Finish = 5 },
                CurrentInterval = new PauseInterval() { Start = 1, Finish = 5 }               
            };
            var intervals = new ObservableCollection<PauseInterval>();
         
            var service = new PausesService(settings, intervals, logerMock.Object);

            //act
            var res = service.GetPause();

            //assert
            Assert.Less(res, 5000);
        }

    }
}
