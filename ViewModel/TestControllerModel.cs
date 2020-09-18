using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EmailSender.ViewModel
{
    public class TestControllerModel : ObservableObject
    {
        public RelayCommand TestCommand
        {
            get;
            private set;
        }


        public TestControllerModel()
        {
            Field1 = "Stok value";
            TestCommand = new RelayCommand(HelloShowMessage);
        }
        private string _field1;

        public string Field1
        {
            get { return _field1; }
            set { Set(()=>Field1,ref _field1, value); }
        }

        private void HelloShowMessage()
        {
            MessageBox.Show("Настройки сохранены");
        }

    }
}
