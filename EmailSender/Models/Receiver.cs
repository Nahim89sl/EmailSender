using Stylet;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Models
{
    public class Receiver : PropertyChangedBase
    {
        public string Email { get; set; }
        public string Hidden { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public Letter Letter { get; set; }


        private string _statusEmailExist;
        public string statusEmailExist
        {
            get
            {
                return _statusEmailExist;
            }
            set
            {
                SetAndNotify(ref this._statusEmailExist, value);
            }
        }

        private string _statusSend;
        public string StatusSend
        {
            get
            {
                return _statusSend;
            }
            set
            {
                SetAndNotify(ref this._statusSend, value);
            }
        }


        public int IdReceiver { get; set; }
        public string Address { get; set; }
        //public string StatusSend { get; set; }
        public string CompanyName { get; set; }
        public string PersonName { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public int Count { get; set; }
        public string StatusEmailExist { get; set; }
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
