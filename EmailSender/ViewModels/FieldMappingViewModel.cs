using Stylet;
using System;
using System.Collections.Generic;
using System.Text;
using EmailSender.Interfaces;
using EmailSender.Models;
using StyletIoC;
using EmailSender.Settings;
using EmailSender.Settings.Models;
using System.Collections.ObjectModel;
using System.IO;
using EmailSender.Logger;

namespace EmailSender.ViewModels
{
    public class FieldMappingViewModel : PropertyChangedBase
    {
        private FieldMappingSettingsModel fieldSettings;
        private ILoadReceivers _loader;
        private IDialogService _dialogService;
        //private ILogger _logger;

        public FieldMappingViewModel(IContainer ioc)
        {
            var settings = ioc.Get<AppSettingsModel>();
            _dialogService = ioc.Get<IDialogService>();
            Receivers = ioc.Get<BindableCollection<Receiver>>();
            _loader = ioc.Get<ILoadReceivers>();
            //_logger = ioc.Get<ILogger>();
            //check if the settings not set
            if (settings.FielMappingSettings == null)
            {
                settings.FielMappingSettings = new FieldMappingSettingsModel();
            }
            fieldSettings = settings.FielMappingSettings;
            
            if(File.Exists(ReceiverListFilePath))
            {
                _loader.Load();
                //_logger.InfoSender($"Loaded {_receivers.Count} receivers");
                NotifyOfPropertyChange(nameof(this.Receivers));                
            }
        }

        private BindableCollection<Receiver> _receivers;
        public BindableCollection<Receiver> Receivers
        {
            get
            {
                
                return _receivers;
            }
            set
            {
                SetAndNotify(ref _receivers, value);
            }
        }

        private string receiverListFilePath;
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

        public string ReceiverListFilePath
        {
            get
            {
                receiverListFilePath = fieldSettings?.receiverListFilePath ?? "";
                return receiverListFilePath;
            }
            set
            {
                SetAndNotify(ref receiverListFilePath, value);
                fieldSettings.receiverListFilePath = value;
            }
        }

        public string FieldEmail
        {
            get
            {
                fieldEmail = fieldSettings?.fieldEmail ?? "";
                return fieldEmail;
            }
            set
            {
                SetAndNotify(ref fieldEmail, value);
                fieldSettings.fieldEmail = value;
            }
        }
        public string FieldOrganizationName
        {
                get
                {
                    fieldOrganizationName = fieldSettings?.fieldOrganizationName ?? "";
                    return fieldOrganizationName;
                }
                set
                {
                    SetAndNotify(ref fieldOrganizationName, value);
                    fieldSettings.fieldOrganizationName = value;
                }
        }
        public string FieldPersonName
        {
            get
            {
                fieldPersonName = fieldSettings?.fieldPersonName ?? "";
                return fieldPersonName;
            }
            set
            {
                SetAndNotify(ref fieldPersonName, value);
                fieldSettings.fieldPersonName = value;
            }
        }
        public string FieldSendingStatus
        {
            get
            {
                fieldSendingStatus = fieldSettings?.fieldSendingStatus ?? "";
                return fieldSendingStatus;
            }
            set
            {
                SetAndNotify(ref fieldSendingStatus, value);
                fieldSettings.fieldSendingStatus = value;
            }
        }
        public string FieldValidateStatus
        {
            get
            {
                fieldValidateStatus = fieldSettings?.fieldValidateStatus ?? "";
                return fieldValidateStatus;
            }
            set
            {
                SetAndNotify(ref fieldValidateStatus, value);
                fieldSettings.fieldValidateStatus = value;
            }
        }
        public string FieldInn
        {
            get
            {
                fieldInn = fieldSettings?.fieldInn ?? "";
                return fieldInn;
            }
            set
            {
                SetAndNotify(ref fieldInn, value);
                fieldSettings.fieldInn = value;
            }
        }
        public string FieldOkvd
        {
            get
            {
                fieldOkvd = fieldSettings?.fieldOkvd ?? "";
                return fieldOkvd;
            }
            set
            {
                SetAndNotify(ref fieldOkvd, value);
                fieldSettings.fieldOkvd = value;
            }
        }
        public string FieldDate1
        {
            get
            {
                fieldDate1 = fieldSettings?.fieldDate1 ?? "";
                return fieldDate1;
            }
            set
            {
                SetAndNotify(ref fieldDate1, value);
                fieldSettings.fieldDate1 = value;
            }
        }
        public string FieldDate2
        {
            get
            {
                fieldDate2 = fieldSettings?.fieldDate2 ?? "";
                return fieldDate2;
            }
            set 
            {
                SetAndNotify(ref fieldDate2, value);
                fieldSettings.fieldDate2 = value;
            }
            }
        public string FieldDate3
        {
            get
            {
                fieldDate3 = fieldSettings?.fieldDate3 ?? "";
                return fieldDate3;
            }
            set
            {
                SetAndNotify(ref fieldDate3, value);
                fieldSettings.fieldDate3 = value;
            }
        }
        public string FieldPhone
        {
            get
            {
                fieldPhone = fieldSettings?.fieldPhone ?? "";
                return fieldPhone;
            }
            set
            {
                SetAndNotify(ref fieldPhone, value);
                fieldSettings.fieldPhone = value;
            }
        }
        public string FieldAddress
        {
            get
            {
                fieldAddress = fieldSettings?.fieldAddress ?? "";
                return fieldAddress;
            }
            set
            {
                SetAndNotify(ref fieldAddress, value);
                fieldSettings.fieldAddress = value;
            }
        }
        public string FieldContractAmount
        {
            get
            {
                fieldContractAmount = fieldSettings?.fieldContractAmount ?? "";
                return fieldContractAmount;
            }
            set
            {
                SetAndNotify(ref fieldContractAmount, value);
                fieldSettings.fieldContractAmount = value;
            }
        }
        public string FieldRecord1
        {
            get
            {
                fieldRecord1 = fieldSettings?.fieldRecord1 ?? "";
                return fieldRecord1;
            }
            set
            {
                SetAndNotify(ref fieldRecord1, value);
                fieldSettings.fieldRecord1 = value;
            }
        }
        public string FieldRecord2
        {
            get
            {
                fieldRecord2 = fieldSettings?.fieldRecord2 ?? "";
                return fieldRecord2;
            }
            set
            {
                SetAndNotify(ref fieldRecord2, value);
                fieldSettings.fieldRecord2 = value;
            }
        }
        public string FieldRecord3
        {
            get
            {
                fieldRecord3 = fieldSettings?.fieldRecord3 ?? "";
                return fieldRecord3;
            }
            set
            {
                SetAndNotify(ref fieldRecord3, value);
                fieldSettings.fieldRecord3 = value;
            }
        }

        public void LoadReseiversCommand()
        {
            if (_dialogService.OpenFileDialog() == true)
            {
                ReceiverListFilePath = _dialogService.FilePath;
                _loader.Load();
                NotifyOfPropertyChange(nameof(this.ReceiverListFilePath));                
                //_logger.InfoSender($"Loaded {_receivers.Count} receivers");
            }
        }
    }
}
