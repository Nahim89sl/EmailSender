using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface IStatuses
    {
        string mailSended { get; }
        string mailAnswer { get; }
        string mailNotSend { get; }
        string mailWrongFormat { get; }
        string mailNotExist { get; }
        
        string defaultValue { get; }

        string serverOk { get; }
        string serverAuthErr { get; }
        string serverTimeOutErr { get; }
        
        string accOk { get; }
        string accBlock { get; }
        string accAuthErr { get; }
    }
}
