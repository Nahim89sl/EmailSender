using EmailSender.Interfaces;
using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace EmailSender.Services
{
    class OurMailsTxtService : IOurMails
    {
        public async void LoadAsync(string path, ObservableCollection<Receiver> receivers)
        {
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    receivers.Add(new Receiver() { Email = line, Count = 0 });
                }
            }
        }
    }
}
