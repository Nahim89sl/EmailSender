using EmailSender.Interfaces;
using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender.Services
{
    class OurMailsTxtService : IOurMails
    {
        public  ObservableCollection<Receiver> LoadAsync(string path)
        {
            var receivers = new ObservableCollection<Receiver>();
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    receivers.Add(new Receiver() { Email = line, Count = 0 });
                }
            }
            return receivers;
        }
    }
}
