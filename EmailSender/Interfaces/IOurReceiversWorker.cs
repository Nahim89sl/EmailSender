using EmailSender.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface IOurReceiversWorker
    {
        /// <summary>
        /// This method return Receiver from our list wich has status ok in db from mailImitator
        /// </summary>
        /// <param name="receivers"></param>
        /// <param name="receiver"></param>
        /// <param name="dbPath"></param>
        /// <returns></returns>
        Receiver GetReadyReceiverForSend(BindableCollection<Receiver> receivers, Receiver receiver, string dbPath);
        /// <summary>
        /// update collection that we need reload
        /// </summary>
        /// <param name="startCollection"></param>
        /// <param name="collectionForCopy"></param>
        void Load(BindableCollection<Receiver> startCollection, string dbPath);
    }
}
