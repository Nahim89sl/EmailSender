using EmailSender.Interfaces;
using EmailSender.Models;
using EmailSender.Services;
using Moq;
using NUnit.Framework;
using Stylet;
using System.Collections.ObjectModel;

namespace EmailSenderTests
{
    [TestFixture]
    public class ReceiverWorkerTests
    {
        [Test]
        public void Create_Obj_Of_Class()
        {
            var loadReceiversMock = new Mock<ILoadReceivers>(); 
            var obj = new OurReceiversWorker(loadReceiversMock.Object);
            Assert.IsNotNull(obj);
        }

        [Test]        
        public void GetReadyReceiverForSend_With_Epmty_Collection()
        {
            // Arrange
            var loadReceiversMock = new Mock<ILoadReceivers>();  
            var receiversFromDb = new BindableCollection<Receiver>() { 
                new Receiver(),
                new Receiver(),
                new Receiver(),
                new Receiver(),
                new Receiver(),
                new Receiver(),
            };
            
            loadReceiversMock.Setup(o => o.LoadOurReceivers(It.IsAny<string>())).Returns(receiversFromDb);
            loadReceiversMock.Setup(o => o.CheckStatusOfOurReceiver(It.IsAny<Receiver>(), It.IsAny<string>())).Returns(true);
            
            var worker = new OurReceiversWorker(loadReceiversMock.Object);

            BindableCollection<Receiver> receiversStart = new BindableCollection<Receiver>();
            Receiver receiver = new Receiver();

            // Act
            receiver = worker.GetReadyReceiverForSend(receiversStart, receiver, It.IsAny<string>());

            // Assert
            Assert.IsNotNull(receiver);
            Assert.AreEqual(receiversStart.Count, 6);
            loadReceiversMock.Verify(o => o.CheckStatusOfOurReceiver(It.IsAny<Receiver>(), It.IsAny<string>()), Times.Once());
            loadReceiversMock.Verify(o => o.LoadOurReceivers( It.IsAny<string>()), Times.Once());
        }


        [Test]
        public void GetReadyReceiverForSend_With_Full_Collection()
        {
            // Arrange
            var loadReceiversMock = new Mock<ILoadReceivers>();
            var inputReceiversCollection = new BindableCollection<Receiver>() {
                new Receiver(),
                new Receiver(),
                new Receiver(),
                new Receiver(),
                new Receiver(),
                new Receiver(),
            };

            loadReceiversMock.Setup(o => o.CheckStatusOfOurReceiver(It.IsAny<Receiver>(), It.IsAny<string>())).Returns(true);

            var worker = new OurReceiversWorker(loadReceiversMock.Object);

            Receiver receiver = new Receiver();

            // Act
            receiver = worker.GetReadyReceiverForSend(inputReceiversCollection, receiver, It.IsAny<string>());

            // Assert
            Assert.IsNotNull(receiver);
            Assert.AreEqual(inputReceiversCollection.Count, 6);
            loadReceiversMock.Verify(o => o.CheckStatusOfOurReceiver(It.IsAny<Receiver>(), It.IsAny<string>()), Times.Once());
            loadReceiversMock.Verify(o => o.LoadOurReceivers(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetReadyReceiverForSend_With_accs_bas_status()
        {
            // Arrange
            var startReceiver = new Receiver() { Status = "bad"};
            var loadedReceiver = new Receiver() { Status = "ok" };

            var loadReceiversMock = new Mock<ILoadReceivers>();
            var  receiversStartCollection = new BindableCollection<Receiver>() {
                startReceiver,
                startReceiver,
                startReceiver,
                startReceiver,
                startReceiver,
                startReceiver,
            };

            var receiversFromDb = new BindableCollection<Receiver>() {
                loadedReceiver,
                loadedReceiver,
                loadedReceiver,
                loadedReceiver,
                loadedReceiver,
                loadedReceiver,
            };

            loadReceiversMock.Setup(o => o.LoadOurReceivers(It.IsAny<string>())).Returns(receiversFromDb);
            loadReceiversMock.Setup(o => o.CheckStatusOfOurReceiver(loadedReceiver, It.IsAny<string>())).Returns(true);
            loadReceiversMock.Setup(o => o.CheckStatusOfOurReceiver(startReceiver, It.IsAny<string>())).Returns(false);

            var worker = new OurReceiversWorker(loadReceiversMock.Object);

            Receiver receiver = new Receiver();

            // Act
            receiver = worker.GetReadyReceiverForSend(receiversStartCollection, receiver, It.IsAny<string>());

            // Assert
            Assert.IsNotNull(receiver);
            Assert.AreEqual(receiversStartCollection.Count,6);
            loadReceiversMock.Verify(o => o.CheckStatusOfOurReceiver(It.IsAny<Receiver>(), It.IsAny<string>()), Times.Exactly(3));
            loadReceiversMock.Verify(o => o.LoadOurReceivers(It.IsAny<string>()), Times.Once());
        }
        [Test]
        public void GetReadyReceiverForSend_When_no_receivers_res_null()
        {
            // Arrange
            var loadReceiversMock = new Mock<ILoadReceivers>();
            var inputReceiversCollection = new BindableCollection<Receiver>();
            var receiversFromDb = new BindableCollection<Receiver>();

            loadReceiversMock.Setup(o => o.LoadOurReceivers(It.IsAny<string>())).Returns(receiversFromDb);
            loadReceiversMock.Setup(o => o.CheckStatusOfOurReceiver(It.IsAny<Receiver>(), It.IsAny<string>())).Returns(false);

            var worker = new OurReceiversWorker(loadReceiversMock.Object);
            Receiver receiver = new Receiver();

            // Act
            receiver = worker.GetReadyReceiverForSend(inputReceiversCollection, receiver, It.IsAny<string>());

            // Assert
            Assert.IsNull(receiver);
            loadReceiversMock.Verify(o => o.CheckStatusOfOurReceiver(It.IsAny<Receiver>(), It.IsAny<string>()), Times.Never);
            loadReceiversMock.Verify(o => o.LoadOurReceivers(It.IsAny<string>()), Times.Once);
        }
    }
}
