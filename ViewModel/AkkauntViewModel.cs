using EmailSender.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender.ViewModel
{
    public class AkkauntViewModel : ObservableObject
    {

        public RelayCommand CheckAkkCommand
        {
            get;
            private set;
        }

        private Akkaunt activeAkk;
        public Akkaunt ActiveAkk
        {
            get 
            { 
                return activeAkk; 
            }
            set 
            { 
                Set(()=>ActiveAkk, ref activeAkk,value);
            }
        }
        private ObservableCollection<Akkaunt> akkaunts;

        public ObservableCollection<Akkaunt> Akkaunts
        {
            get { return akkaunts; }
            set { Set(()=>Akkaunts, ref akkaunts, value); }
        }

        public AkkauntViewModel()
        {
            CheckAkkCommand = new RelayCommand(CheckAkk_execute);
        }

        private void CheckAkk_execute()
        {
            try
            {
                Task.Run(async () => {
                    //await _sendService.Authentification(ActiveAkk);
                    DispatcherHelper.CheckBeginInvokeOnUI(() => {
                        RaisePropertyChanged(() => ActiveAkk);
                    });

                });
            }
            catch (Exception ex)
            {
               // logger.Error("ExecudeCheckAkkaunt " + ex.Message);
            }
        }




    }
}
