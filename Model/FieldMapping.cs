using GalaSoft.MvvmLight;

namespace EmailSender.Model
{
    public class FieldMapping : ObservableObject
    {
        string fieldEmail;
        string fieldOrganizationName;
        string fieldPersonName;
        string fieldSendingStatus;
        string fieldValidateStatus;
        string fieldInn;
        string fieldOkvd;
        string fieldDate1;
        string fieldDate2;
        string fieldDate3;
        string fieldPhone;
        string fieldAddress;
        string fieldContractAmount;
        string fieldRecord1;
        string fieldRecord2;
        string fieldRecord3;

        public string FieldEmail 
        {
            get
            {
                return fieldEmail;
            }
            set
            {
                Set(()=>FieldEmail,ref fieldEmail,value);
            }
        }
        public string FieldOrganizationName
        {
            get
            {
                return fieldOrganizationName;
            }
            set
            {
                Set(() => FieldOrganizationName, ref fieldOrganizationName, value);
            }
        }
        public string FieldPersonName
        {
            get
            {
                return fieldPersonName;
            }
            set
            {
                Set(() => FieldPersonName, ref fieldPersonName, value);
            }
        }
        public string FieldSendingStatus
        {
            get
            {
                return fieldSendingStatus;
            }
            set
            {
                Set(() => FieldSendingStatus, ref fieldSendingStatus, value);
            }
        }
        public string FieldValidateStatus
        {
            get
            {
                return fieldValidateStatus;
            }
            set
            {
                Set(() => FieldValidateStatus, ref fieldValidateStatus, value);
            }
        }
        public string FieldInn
        {
            get
            {
                return fieldInn;
            }
            set
            {
                Set(() => FieldInn, ref fieldInn, value);
            }
        }
        public string FieldOkvd
        {
            get
            {
                return fieldOkvd;
            }
            set
            {
                Set(() => FieldOkvd, ref fieldOkvd, value);
            }
        }
        public string FieldDate1
        {
            get
            {
                return fieldDate1;
            }
            set
            {
                Set(() => FieldDate1, ref fieldDate1, value);
            }
        }
        public string FieldDate2
        {
            get
            {
                return fieldDate2;
            }
            set
            {
                Set(() => FieldDate2, ref fieldDate2, value);
            }
        }
        public string FieldDate3
        {
            get
            {
                return fieldDate3;
            }
            set
            {
                Set(() => FieldDate3, ref fieldDate3, value);
            }
        }
        public string FieldPhone
        {
            get
            {
                return fieldPhone;
            }
            set
            {
                Set(() => FieldPhone, ref fieldPhone, value);
            }
        }
        public string FieldAddress
        {
            get
            {
                return fieldAddress;
            }
            set
            {
                Set(() => FieldAddress, ref fieldAddress, value);
            }
        }
        public string FieldContractAmount
        {
            get
            {
                return fieldContractAmount;
            }
            set
            {
                Set(() => FieldContractAmount, ref fieldContractAmount, value);
            }
        }
        public string FieldRecord1
        {
            get
            {
                return fieldRecord1;
            }
            set
            {
                Set(() => FieldRecord1, ref fieldRecord1, value);
            }
        }
        public string FieldRecord2
        {
            get
            {
                return fieldRecord2;
            }
            set
            {
                Set(() => FieldRecord2, ref fieldRecord2, value);
            }
        }
        public string FieldRecord3
        {
            get
            {
                return fieldRecord3;
            }
            set
            {
                Set(() => FieldRecord3, ref fieldRecord3, value);
            }
        }

    }
}
