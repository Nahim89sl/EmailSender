using EmailSender.Models;
using EmailSender.Settings.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface IExcelWorker
    {
        void LoadReceivers(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping);
    }
}
