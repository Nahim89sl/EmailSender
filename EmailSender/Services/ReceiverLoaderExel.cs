using EmailSender.Models;
using EmailSender.Settings.Models;
using StyletIoC;
using System;
using System.Collections.ObjectModel;
using EmailSender.Logger;
using System.IO;
using EmailSender.Settings;
using OfficeOpenXml;
using System.Linq;
using System.Net.Mail;
using EmailSender.Interfaces;
using System.Threading.Tasks;
using Stylet;

namespace EmailSender.Services
{
    public class ReceiverLoaderExel : ILoadReceivers
    {
        private ILogger logger;
        private FieldMappingSettingsModel FieldMapping;
        private string FilePath;
        private ObservableCollection<Receiver> _receivers;
        private const string defValue = "no";

        #region Constructor
        public ReceiverLoaderExel(IContainer ioc)
        {
            logger = ioc.Get<ILogger>();
            FieldMapping = ioc.Get<AppSettingsModel>()?.FielMappingSettings ?? null;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _receivers = ioc.Get<BindableCollection<Receiver>>();
        }
        #endregion

        public ObservableCollection<Receiver> Load() 
        {
            if (File.Exists(FieldMapping.receiverListFilePath))
            {
                return Open(FieldMapping);
            }
            return new ObservableCollection<Receiver>();
        }

        //load data to app
        public ObservableCollection<Receiver> Open( FieldMappingSettingsModel FieldMapping)
        {
            //create a fileinfo object of an excel file on the disk
            FileInfo file = new FileInfo(FieldMapping.receiverListFilePath);

            if (!file.Exists)
            {
                logger.ErrorSender("Send list file not exist " + FieldMapping.receiverListFilePath);
                return null;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _receivers.Clear();

            //create a new Excel package from the file
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                //create an instance of the the first sheet in the loaded file
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                
                //count of rows in file
                int rowCount = worksheet.Dimension.Rows;
                for (int i = 1; i < rowCount; i++)
                {
                    var recaiver = new Receiver();
                    recaiver.IdReceiver = i;
                    recaiver.Email = worksheet.Cells[FieldMapping.fieldEmail + i.ToString()].Value?.ToString() ?? defValue;
                    
                    if (FieldMapping.fieldValidateStatus != null)
                    {
                        recaiver.StatusEmailExist = worksheet.Cells[FieldMapping.fieldValidateStatus + i.ToString()].Value?.ToString() ?? defValue;
                    }
                    if (FieldMapping.fieldSendingStatus != null)
                    {
                        recaiver.StatusSend = worksheet.Cells[FieldMapping.fieldSendingStatus + i.ToString()].Value?.ToString() ?? defValue;
                    }
                    if (FieldMapping.fieldOrganizationName != null)
                    {
                        recaiver.CompanyName = worksheet.Cells[FieldMapping.fieldOrganizationName + i.ToString()].Value?.ToString() ?? defValue;
                    }      
                    if(FieldMapping.fieldPersonName != null)
                    {
                        recaiver.PersonName = worksheet.Cells[FieldMapping.fieldPersonName + i.ToString()].Value?.ToString() ?? defValue;
                    }
                    if(FieldMapping.fieldInn != null)
                    {
                        recaiver.FieldInn = worksheet.Cells[FieldMapping.fieldInn + i.ToString()].Value?.ToString() ?? defValue;
                    }
                    if (FieldMapping.fieldOkvd != null)
                    {
                        recaiver.FieldOkvd = worksheet.Cells[FieldMapping.fieldOkvd + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldPhone != null)
                    {
                        recaiver.FieldPhone = worksheet.Cells[FieldMapping.fieldPhone + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldAddress != null)
                    {
                        recaiver.FieldAddress = worksheet.Cells[FieldMapping.fieldAddress + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldContractAmount != null)
                    {
                        recaiver.FieldContractAmount = worksheet.Cells[FieldMapping.fieldContractAmount + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldDate1 != null)
                    {
                        recaiver.FieldDate1 = worksheet.Cells[FieldMapping.fieldDate1 + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldDate2 != null)
                    {
                        recaiver.FieldDate2 = worksheet.Cells[FieldMapping.fieldDate2 + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldDate3 != null)
                    {
                        recaiver.FieldDate3 = worksheet.Cells[FieldMapping.fieldDate3 + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldRecord1 != null)
                    {
                        recaiver.FieldRecord1 = worksheet.Cells[FieldMapping.fieldRecord1 + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldRecord2 != null)
                    {
                        recaiver.FieldRecord2 = worksheet.Cells[FieldMapping.fieldRecord2 + i.ToString()].Value?.ToString() ?? defValue;
                    }                    
                    if (FieldMapping.fieldRecord3 != null)
                    {
                        recaiver.FieldRecord3 = worksheet.Cells[FieldMapping.fieldRecord3 + i.ToString()].Value?.ToString() ?? defValue;
                    }
                    
                    //validate email address
                    var resEmail = ValidateEmailAddress(recaiver.Email);
                    if (resEmail == "")
                    {
                        recaiver.StatusEmailExist = "Wrong Email";
                        recaiver.StatusSend = "Wrong Email";
                    }
                    else
                    {
                        recaiver.Email = resEmail;
                    }
                    _receivers.Add(recaiver);
                }
            }

            //var count = _receivers.Where(a => a.StatusSend == defValue).ToList().Count;
            logger.InfoSender($"Loaded {_receivers.Count.ToString()} receivers");
            //logger.InfoSender($"Ready for sending {count.ToString()} receivers");
            return _receivers;
        }

        public void SaveChanges(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping)
        {
            try
            {
                logger.InfoSender("Save changes of list");
                FileInfo file = new FileInfo(FieldMapping.receiverListFilePath);
                if (!file.Exists)
                {
                    logger.ErrorSender("Send list file not exist " + FieldMapping.receiverListFilePath);
                    return;
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                int timeSaveStart = TimeUnixService.Timestamp();
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();

                    foreach (var receiver in receivers)
                    {
                        worksheet.Cells[FieldMapping.fieldValidateStatus + receiver.IdReceiver.ToString()].Value = receiver.StatusEmailExist;
                        worksheet.Cells[FieldMapping.fieldSendingStatus + receiver.IdReceiver.ToString()].Value = receiver.StatusSend;
                    }
                    //save file
                    Byte[] bin = excelPackage.GetAsByteArray();
                    File.WriteAllBytes(FieldMapping.receiverListFilePath, bin);
                    //excelPackage.SaveAs(file);
                }
                int res = TimeUnixService.Timestamp() - timeSaveStart;
                logger.InfoSender($"save process take {res.ToString()} sec");
            }
            catch (Exception ex)
            {
                logger.ErrorSender($"Save process get error {ex.Message}");
            }            
        }

        public async Task SaveChangesAsync(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping)
        {
            Task.Run(()=> {
                SaveChanges(receivers, FieldMapping);
            });            
        }

        public void AddListToReport(string filename, ObservableCollection<Answer> letters, Receiver receiver)
        {

            try { 
                FileInfo file = new FileInfo(filename);
                if (!file.Exists)
                {
                    logger.InfoSender("Report file not exist " + filename);
                    CreateFile(filename);
                    logger.InfoSender("Report created " + filename);
                    file = new FileInfo(filename);
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                    int rowCount = worksheet.Dimension.Rows+1;
                    foreach (var letter in letters)
                    {
                        worksheet.Cells[rowCount, 1].Value = receiver?.IdReceiver.ToString() ?? "";
                        worksheet.Cells[rowCount, 2].Value = receiver?.CompanyName ?? "";

                        worksheet.Cells[rowCount, 3].Value = letter.Email;
                        worksheet.Cells[rowCount, 4].Value = letter.Subject;
                        worksheet.Cells[rowCount, 5].Value = letter.Text;

                        rowCount++;
                    }
                    excelPackage.Save();
                }
            }
            catch(Exception ex)
            {
                logger.ErrorReader($"Error with write to report file AddListToReport: {ex.Message}");
            }
        }

        public void AddToReport(string filename, Answer letter, Receiver receiver)
        {
            try
            {
                FileInfo file = new FileInfo(filename);
                if (!file.Exists)
                {
                    logger.ErrorSender("Report file not exist " + filename);
                    CreateFile(filename);
                    logger.InfoSender("Report created " + filename);
                    file = new FileInfo(filename);
                }

                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                    int rowCount = worksheet.Dimension.Rows + 1;

                    worksheet.Cells[rowCount, 1].Value = receiver?.IdReceiver.ToString() ?? "";
                    worksheet.Cells[rowCount, 2].Value = receiver?.CompanyName ?? "";

                    worksheet.Cells[rowCount, 3].Value = letter.From;
                    worksheet.Cells[rowCount, 4].Value = letter.Email;
                    worksheet.Cells[rowCount, 5].Value = letter.Subject;
                    worksheet.Cells[rowCount, 6].Value = letter.Text;

                    //save result
                    Byte[] bin = excelPackage.GetAsByteArray();
                    File.WriteAllBytes(filename, bin);
                    //excelPackage.Save();
                }
            }catch(Exception ex)
            {
                logger.ErrorReader($"Error with write to report file AddToReport: {ex.Message}");
            }
            
        }

        private void CreateFile(string filename)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Report");

                //Add some text to cell A1
                worksheet.Cells["A1"].Value = "Reports of answers";

                //Save your file
                FileInfo fi = new FileInfo(filename);
                excelPackage.SaveAs(fi);
            }
        }

        public string ValidateEmailAddress(string emilAddress)
        {
            try
            {
                string addr = emilAddress.Replace(" ", "");
                //check the last symbol . / \
                var symb = addr.Substring(addr.Length - 1);
                if ((symb == ".") || (symb == "/") || (symb == "\\"))
                {
                    addr = addr.Substring(0, addr.Length - 1);
                }

                addr = new MailAddress(addr).Address;
                return addr.ToLower();
            }
            catch (Exception ex)
            {
                logger.ErrorSender($"ValidateEmailAddress {ex.Message}");
                return "";
            }
        }


        public void OpenAndLoad()
        {
            throw new NotImplementedException();
        }

        public void SaveReceiver(Receiver receivers)
        {
            throw new NotImplementedException();
        }

        public void LoadOurReceivers(ObservableCollection<Receiver> receivers, string dbPath)
        {
            throw new NotImplementedException();
        }

        public bool CheckStatusOfOurReceiver(Receiver receiver, string dbPath)
        {
            throw new NotImplementedException();
        }

        public BindableCollection<Receiver> LoadOurReceivers(string dbPath)
        {
            throw new NotImplementedException();
        }
    }
}
