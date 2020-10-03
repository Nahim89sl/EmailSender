using EmailSender.Interfaces;
using EmailSender.Model;
using NLog;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows;

namespace EmailSender.Services
{
    public class ExelFileService : IFileService
    {
        private Logger logger;
        private string _lib;
        public ExelFileService()
        {
            logger = LogManager.GetCurrentClassLogger();
            _lib = "ExelFileServece";
        }
        public ObservableCollection<Receiver> Open(string filename,  FieldMapping FieldMapping)
        {

            
            //create a fileinfo object of an excel file on the disk
            FileInfo file = new FileInfo(filename);

            if (!file.Exists)
            {
                logger.Error("Send list file not exist " + filename);
                MessageBox.Show("Файл рассылки");
                return null;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var receivers = new ObservableCollection<Receiver>();

            //create a new Excel package from the file
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                
                //create an instance of the the first sheet in the loaded file
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                string defValue = "no";
                //count of rows in file
                int rowCount = worksheet.Dimension.Rows;
                for(int i = 1; i < rowCount; i++)
                {
                    var recaiver = new Receiver();
                    recaiver.IdReceiver = i;
                    recaiver.Email = worksheet?.Cells[FieldMapping.FieldEmail + i.ToString()].Value?.ToString() ?? defValue; 
                    


                    recaiver.StatusEmailExist = worksheet?.Cells[FieldMapping.FieldValidateStatus + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.StatusSend = worksheet?.Cells[FieldMapping.FieldSendingStatus + i.ToString()].Value?.ToString() ?? defValue;                   
                    recaiver.CompanyName = worksheet?.Cells[FieldMapping.FieldOrganizationName + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.PersonName = worksheet?.Cells[FieldMapping.FieldPersonName + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldInn = worksheet?.Cells[FieldMapping.FieldInn + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldOkvd = worksheet?.Cells[FieldMapping.FieldOkvd + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldPhone = worksheet?.Cells[FieldMapping.FieldPhone + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldAddress = worksheet?.Cells[FieldMapping.FieldAddress + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldContractAmount = worksheet?.Cells[FieldMapping.FieldContractAmount + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldDate1 = worksheet?.Cells[FieldMapping.FieldDate1 + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldDate2 = worksheet?.Cells[FieldMapping.FieldDate2 + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldDate3 = worksheet?.Cells[FieldMapping.FieldDate3 + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldRecord1 = worksheet?.Cells[FieldMapping.FieldRecord1 + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldRecord2 = worksheet?.Cells[FieldMapping.FieldRecord2 + i.ToString()].Value?.ToString() ?? defValue;
                    recaiver.FieldRecord3 = worksheet?.Cells[FieldMapping.FieldRecord3 + i.ToString()].Value?.ToString() ?? defValue;
                    
                    
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

                    receivers.Add(recaiver);
                }
            }
            return receivers;

        }

        public void Save(string filename, ObservableCollection<Receiver> receivers, FieldMapping FieldMapping)
        {
            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
            {
                logger.Error("Send list file not exist " + filename);
                MessageBox.Show("Файл рассылки");
                return;
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                foreach(var receiver in receivers)
                {
                    worksheet.Cells[FieldMapping.FieldEmail + receiver.IdReceiver.ToString()].Value = receiver.Email;
                    worksheet.Cells[FieldMapping.FieldOrganizationName + receiver.IdReceiver.ToString()].Value = receiver.CompanyName;
                    worksheet.Cells[FieldMapping.FieldPersonName + receiver.IdReceiver.ToString()].Value = receiver.PersonName;
                    worksheet.Cells[FieldMapping.FieldValidateStatus + receiver.IdReceiver.ToString()].Value = receiver.StatusEmailExist;
                    worksheet.Cells[FieldMapping.FieldSendingStatus + receiver.IdReceiver.ToString()].Value = receiver.StatusSend;
                }
                Byte[] bin = excelPackage.GetAsByteArray();
                File.WriteAllBytes(filename, bin);
                excelPackage.Dispose();
            }

        }

        public void SaveChanges(string filename, Receiver receiver, FieldMapping FieldMapping)
        {
            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
            {
                logger.Error("Send list file not exist " + filename);
                MessageBox.Show("Файл рассылки");
                return;
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                
                 worksheet.Cells[FieldMapping.FieldEmail + receiver.IdReceiver.ToString()].Value = receiver.Email;
                 worksheet.Cells[FieldMapping.FieldOrganizationName + receiver.IdReceiver.ToString()].Value = receiver.CompanyName;
                 worksheet.Cells[FieldMapping.FieldPersonName + receiver.IdReceiver.ToString()].Value = receiver.PersonName;
                 worksheet.Cells[FieldMapping.FieldValidateStatus + receiver.IdReceiver.ToString()].Value = receiver.StatusEmailExist;
                 worksheet.Cells[FieldMapping.FieldSendingStatus + receiver.IdReceiver.ToString()].Value = receiver.StatusSend;

                //save file
                //Byte[] bin = excelPackage.GetAsByteArray();
                //File.WriteAllBytes(filename, bin);
                excelPackage.SaveAs(file);
                excelPackage.Dispose();
            }

        }

        public void AddToReport(string filename, ObservableCollection<Letter> letters)
        {
            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
            {
                logger.Error("Report file not exist " + filename);
                CreateFile(filename);
                logger.Error("Report created " + filename);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                int rowCount = worksheet.Dimension.Rows;
                foreach(var letter in letters)
                {
                    worksheet.Cells[rowCount, 1].Value = letter.Id.ToString();
                    worksheet.Cells[rowCount, 2].Value = DateTime.Now.ToString("yyyy.MM.dd hh.mm");
                    worksheet.Cells[rowCount, 3].Value = letter.EmailSender;
                    worksheet.Cells[rowCount, 4].Value = letter.Subject;
                    worksheet.Cells[rowCount, 5].Value = letter.Text;

                    rowCount++;
                }
                excelPackage.Save();
            }
        }

        private void CreateFile(string filename)
        {
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
                return addr;
            }
            catch (Exception ex)
            {
                logger.Error($"{_lib} ValidateEmailAddress {ex.Message}");
                return "";
            }
        }

    }
}
