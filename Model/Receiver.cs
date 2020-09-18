using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Model
{
    public class Receiver : ObservableObject
    {
        private string statusEmailExist;
        private string statusSend;
        public int IdReceiver { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string StatusSend
        {
            get { return statusSend; }
            set { Set(() => StatusSend, ref statusSend, value); }
        }
        public string CompanyName { get; set; }
        public string PersonName { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public int Count { get; set; }
        public string StatusEmailExist
        {
            get { return statusEmailExist; }
            set { Set(() => StatusEmailExist, ref statusEmailExist, value); }
        }
        public string FieldInn { get; set; }
        public string FieldOkvd { get; set; }
        public string FieldDate1 { get; set; }
        public string FieldDate2 { get; set; }
        public string FieldDate3 { get; set; }
        public string FieldPhone { get; set; }
        public string FieldAddress { get; set; }
        public string FieldContractAmount { get; set; }
        public string FieldRecord1 { get; set; }
        public string FieldRecord2 { get; set; }
        public string FieldRecord3 { get; set; }
    }
}
