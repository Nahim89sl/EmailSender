using EmailSender.Interfaces;
using EmailSender.Models;
using Stylet;
using System.Linq;


namespace EmailSender.Services
{
    /// <summary>
    /// This class need for some operations with receivers
    /// </summary>
    public class OurReceiversWorker : IOurReceiversWorker
    {
        private readonly ILoadReceivers _dbWorker;

        #region Constructor

        public OurReceiversWorker(ILoadReceivers dbWorker)
        {
            _dbWorker = dbWorker;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Get ready receiver 
        /// </summary>
        /// <param name="receivers"></param> - list of our receivers
        /// <param name="receiver"></param> - receiver with all necesary values
        /// <returns>receiver ready for receive mail</returns>
        public Receiver GetReadyReceiverForSend(BindableCollection<Receiver> receivers, Receiver receiver, string dbPath)
        {
            Receiver ourReceiver = null;
            int count = 0;
            int maxCount = 10;
            while (ourReceiver == null)
            {
                //if list if receivers too short than we need reload it
                if (receivers.Count < 5)
                {
                    var newReceivers = _dbWorker.LoadOurReceivers(dbPath);
                    if (newReceivers.Count < 1)
                    {
                        return null;
                    }
                    else
                    {
                        UpdateCollection(receivers, newReceivers);
                    }
                }
                //get receiver from list
                ourReceiver = receivers.OrderBy(a => a.Count).FirstOrDefault();

                //check the status of this receiver in db
                if (ourReceiver != null)
                {
                    if (_dbWorker.CheckStatusOfOurReceiver(ourReceiver, dbPath))
                    {
                        ourReceiver.Count++;
                        return CloneReceivcerData(ourReceiver, receiver);
                    }
                    else
                    {
                        //if receiver is blocked than we need to del it and take enother receiver
                        receivers.Remove(ourReceiver);
                        ourReceiver = null;
                    }
                }
                if (count > maxCount)
                {
                    return receivers.OrderBy(a => a.Count).FirstOrDefault();
                }
                count++;
            }
            return null;
        }

        /// <summary>
        /// Copy some fields from etalon receiver
        /// </summary>
        /// <param name="mainReceiver"></param> this is our object for inicialize all ampty fields
        /// <param name="copyReceiver"></param> this is atalon receiver 
        /// <returns></returns>
        private Receiver CloneReceivcerData(Receiver mainReceiver, Receiver copyReceiver)
        {
            if ((mainReceiver != null) && (copyReceiver != null))
            {
                mainReceiver.Bcc = copyReceiver.Bcc;
                mainReceiver.CC = copyReceiver.CC;
                mainReceiver.CompanyName = copyReceiver.CompanyName;
                mainReceiver.FieldAddress = copyReceiver.FieldAddress;
                mainReceiver.FieldContractAmount = copyReceiver.FieldContractAmount;
                mainReceiver.FieldDate1 = copyReceiver.FieldDate1;
                mainReceiver.FieldDate2 = copyReceiver.FieldDate2;
                mainReceiver.FieldDate3 = copyReceiver.FieldDate3;
                mainReceiver.FieldInn = copyReceiver.FieldInn;
                mainReceiver.FieldOkvd = copyReceiver.FieldOkvd;
                mainReceiver.FieldPhone = copyReceiver.FieldPhone;
                mainReceiver.FieldRecord1 = copyReceiver.FieldRecord1;
                mainReceiver.FieldRecord2 = copyReceiver.FieldRecord2;
                mainReceiver.FieldRecord3 = copyReceiver.FieldRecord3;
                mainReceiver.Letter = copyReceiver.Letter;
                mainReceiver.PersonName = copyReceiver.PersonName;
                mainReceiver.Time = copyReceiver.Time;
            }
            return mainReceiver;
        }

        public void Load(BindableCollection<Receiver> startCollection, string dbPath)
        {
            var newReceivers = _dbWorker.LoadOurReceivers(dbPath);
            if (newReceivers.Count > 1)
            {
                UpdateCollection(startCollection, newReceivers);
            }
        }

        private void UpdateCollection(BindableCollection<Receiver> startCollection, BindableCollection<Receiver> collectionForCopy)
        {
            startCollection.Clear();
            foreach(var element in collectionForCopy)
            {
                startCollection.Add(element);
            }
            collectionForCopy.Clear();
        }

        #endregion
    }
}
