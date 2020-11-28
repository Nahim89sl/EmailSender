using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender.Interfaces
{
    interface IReporter
    {
        Task ReportToExel();
        Task ReportToGoogleSheets();
        Task ReportToTelegram();
    }
}
