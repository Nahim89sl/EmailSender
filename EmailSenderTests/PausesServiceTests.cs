using EmailSender.Logger;
using NUnit.Framework;
using Moq;
using EmailSender.Settings.Models;
using EmailSender.Models;
using System.Collections.ObjectModel;
using EmailSender.Services;
using System.Threading;

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


        [Test]
        public void GetPause_Change_smollest_interval()
        {
            //arrange
            var logerMock = new Mock<ILogger>();

            var settings = new SenderSettings()
            {
                ChangeIntTime = 1,
                DopPauseTime = 50,
                DopPauseInterval = new PauseInterval() { Start = 1, Finish = 5 },
                CurrentInterval = new PauseInterval() { Start = 1, Finish = 2 }
            };
            var intervals = new ObservableCollection<PauseInterval>() { 
                new PauseInterval(){Start = 10 ,  Finish = 20},
                new PauseInterval(){Start = 20 ,  Finish = 30},
            };

            var service = new PausesService(settings, intervals, logerMock.Object);

            //act
            Thread.Sleep(3000);
            var res1 = service.GetPause();

            Thread.Sleep(3000);
            var res2 = service.GetPause();

            //assert
            Assert.Less(res1, 20000);
            Assert.Greater(res1, 9999);
            Assert.Less(res2, 20000);
            Assert.Greater(res2, 9999);

        }

        [Test]
        public void GetPause_Change_all_interval()
        {
            //arrange
            var logerMock = new Mock<ILogger>();

            var settings = new SenderSettings()
            {
                ChangeIntTime = 1,
                DopPauseTime = 50,
                DopPauseInterval = new PauseInterval() { Start = 1, Finish = 5 },
                CurrentInterval = new PauseInterval() { Start = 40, Finish = 50 }
            };
            var intervals = new ObservableCollection<PauseInterval>() {
                new PauseInterval(){Start = 10 ,  Finish = 20},
                new PauseInterval(){Start = 20 ,  Finish = 30},
            };

            var service = new PausesService(settings, intervals, logerMock.Object);

            //act
            Thread.Sleep(3000);
            var res1 = service.GetPause();

            Thread.Sleep(3000);
            var res2 = service.GetPause();

            Thread.Sleep(3000);
            var res3 = service.GetPause();

            //assert
            // interval 20-30
            Assert.Less(res1, 30000);
            Assert.Greater(res1, 19000);
            //interval 10-20
            Assert.Less(res2, 20000);
            Assert.Greater(res2, 9999);
            //interval 10-20
            Assert.Less(res3, 20000);
            Assert.Greater(res3, 9999);

        }


        [Test]
        public void GetPause_With_DopPause()
        {
            //arrange
            var logerMock = new Mock<ILogger>();

            var settings = new SenderSettings()
            {
                ChangeIntTime = 1,
                DopPauseTime = 3,
                DopPauseInterval = new PauseInterval() { Start = 2, Finish = 2 },
                CurrentInterval = new PauseInterval() { Start = 1, Finish = 1 }
            };
            var intervals = new ObservableCollection<PauseInterval>();

            var service = new PausesService(settings, intervals, logerMock.Object);
            
            Thread.Sleep(2000);
            //act
            var res = service.GetPause();
            
            Thread.Sleep(3000);

            var res2 = service.GetPause();
            //assert
            Assert.AreEqual(1000, res);
            Assert.AreEqual(3000, res2);
        }
    }
}
