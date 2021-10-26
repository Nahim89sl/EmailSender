using EmailSender.Interfaces;
using EmailSender.Models;
using EmailSender.Settings.Models;
using OfficeOpenXml;
using StyletIoC;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;


namespace EmailSender.Services
{
    public class ExcelWorker : IExcelWorker
    {
        [Inject] private IStatuses statuses;

        public void  LoadReceivers(ObservableCollection<Receiver> _receivers, FieldMappingSettingsModel fm)
        {
            _receivers.Clear();
            //create a fileinfo object of an excel file on the disk
            FileInfo file = new FileInfo(fm.receiverListFilePath);
            //Set licence  Non Commercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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

                    recaiver.Email = worksheet.Cells[fm.fieldEmail + i.ToString()].Value?.ToString() ?? statuses.defaultValue;

                    if (fm.fieldValidateStatus != null)
                    {
                        recaiver.StatusEmailExist = GetValue(
                                                worksheet.Cells[fm.fieldValidateStatus + i.ToString()],
                                                statuses.defaultValue);
                    }
                    if (fm.fieldSendingStatus != null)
                    {
                        recaiver.StatusSend = GetValue(
                                                worksheet.Cells[fm.fieldSendingStatus + i.ToString()],
                                                statuses.defaultValue);
                    }
                    if (fm.fieldOrganizationName != null)
                    {
                        recaiver.CompanyName = GetValue(
                                                worksheet.Cells[fm.fieldOrganizationName + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldPersonName != null)
                    {
                        recaiver.PersonName = GetValue(
                                                worksheet.Cells[fm.fieldPersonName + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldInn != null)
                    {
                        recaiver.FieldInn = GetValue(
                                                worksheet.Cells[fm.fieldInn + i.ToString()],
                                                statuses.emptyField);                        
                    }
                    if (fm.fieldOkvd != null)
                    {
                        recaiver.FieldOkvd = GetValue(
                                                worksheet.Cells[fm.fieldOkvd + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldPhone != null)
                    {
                        recaiver.FieldPhone = GetValue(
                                                worksheet.Cells[fm.fieldPhone + i.ToString()],
                                                statuses.emptyField);                       
                    }
                    if (fm.fieldAddress != null)
                    {
                        recaiver.FieldAddress = GetValue(
                                                worksheet.Cells[fm.fieldAddress + i.ToString()],
                                                statuses.emptyField);                        
                    }
                    if (fm.fieldContractAmount != null)
                    {
                        recaiver.FieldContractAmount = GetValue(
                                                worksheet.Cells[fm.fieldContractAmount + i.ToString()],
                                                statuses.emptyField);                        
                    }
                    if (fm.fieldDate1 != null)
                    {
                        recaiver.FieldDate1 = GetValue(
                                                worksheet.Cells[fm.fieldDate1 + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldDate2 != null)
                    {

                        recaiver.FieldDate2 = GetValue(
                                                worksheet.Cells[fm.fieldDate2 + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldDate3 != null)
                    {
                        recaiver.FieldDate3 = GetValue(
                                                worksheet.Cells[fm.fieldDate3 + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldRecord1 != null)
                    {
                        recaiver.FieldRecord1 = GetValue(
                                                worksheet.Cells[fm.fieldRecord1 + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldRecord2 != null)
                    {
                        recaiver.FieldRecord2 = GetValue(
                                                worksheet.Cells[fm.fieldRecord2 + i.ToString()],
                                                statuses.emptyField);
                    }
                    if (fm.fieldRecord3 != null)
                    {
                        recaiver.FieldRecord3 = GetValue(
                                                worksheet.Cells[fm.fieldRecord3 + i.ToString()],
                                                statuses.emptyField);
                    }


                    //validate email address
                    var resEmail = EmailValidator.Validate(recaiver.Email);
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
        }

        private string GetValue(ExcelRange cell, string defVal)
        {
            if(cell.Value == null)
            {
                return defVal;
            }

            var t = cell.Value.GetType();
            if(t == typeof(DateTime))
            {              
                return GetFromValDateStr(cell.Value.ToString());
            }
            return cell.Value.ToString();
        }

        private string GetFromValDateStr(string date)
        {
            var parsedDate = DateTime.Parse(date);
            return parsedDate.ToString("dd.MM.yyyy");
        }



    }
}
