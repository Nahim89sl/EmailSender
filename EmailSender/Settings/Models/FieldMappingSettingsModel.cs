using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Settings.Models
{
    public class FieldMappingSettingsModel
    {
        public string receiverListFilePath { set; get; }

        public string fieldEmail { get; set; }
        public string fieldOrganizationName { get; set; }
        public string fieldPersonName { get; set; }
        public string fieldSendingStatus { get; set; }
        public string fieldValidateStatus { get; set; }
        public string fieldInn { get; set; }
        public string fieldOkvd { get; set; }
        public string fieldDate1 { get; set; }
        public string fieldDate2 { get; set; }
        public string fieldDate3 { get; set; }
        public string fieldPhone { get; set; }
        public string fieldAddress { get; set; }
        public string fieldContractAmount { get; set; }
        public string fieldRecord1 { get; set; }
        public string fieldRecord2 { get; set; }
        public string fieldRecord3 { get; set; }
    }
}
