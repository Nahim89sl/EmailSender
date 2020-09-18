using EmailSender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmailSender.Interfaces
{
    public interface ILoadRanges
    {
        void Load(string filePath, Pauses pauses);
    }
}
