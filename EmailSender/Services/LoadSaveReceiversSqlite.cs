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
using System.Data.SQLite;
using AppCommon.Interfaces;

namespace EmailSender.Services
{
    public class LoadSaveReceiversSqlite : ILoadReceivers
    {
        private ILogger logger;
        private FieldMappingSettingsModel FieldMapping;
        private ObservableCollection<Receiver> _receivers;
        private const string dbName = "db.sqlite";
        private readonly IStatuses _statuses;
        private IExcelWorker _excel;

        #region Constructor

        public LoadSaveReceiversSqlite(IContainer ioc)
        {
            logger = ioc.Get<ILogger>();
            FieldMapping = ioc.Get<AppSettingsModel>()?.FielMappingSettings ?? null;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _receivers = ioc.Get<BindableCollection<Receiver>>();
            _statuses = ioc.Get<IStatuses>();
            _excel = ioc.Get<IExcelWorker>();
        }

        #endregion

        //convertation from exel to database
        public void OpenAndLoad() 
        {
            //LoadReceiversFromExel(FieldMapping);

            _excel.LoadReceivers(_receivers, FieldMapping);

            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }
            
            CreateReceiversBb();

            ImportDataToDb();
        }

        public ObservableCollection<Receiver> Load()
        {
            if (File.Exists(dbName))
            {
                using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbName};Version=3;"))
                {
                    m_dbConnection.Open();
                    var sqlCommand = new SQLiteCommand(m_dbConnection);
                    sqlCommand.CommandText = "SELECT * FROM receivers";
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var reseiver = new Receiver()
                        {
                            IdReceiver = int.Parse(reader["IdReceiver"].ToString()),
                            StatusSend = reader["StatusSend"].ToString(),
                            StatusEmailExist = reader["StatusEmailExist"].ToString(),
                            Email = reader["Email"].ToString(),
                            Hidden = reader["Hidden"].ToString(),
                            Time = reader["Time"].ToString(),
                            Address = reader["Address"].ToString(),
                            CompanyName = reader["CompanyName"].ToString(),
                            PersonName = reader["PersonName"].ToString(),
                            CC = reader["CC"].ToString(),
                            Bcc = reader["Bcc"].ToString(),
                            Count = int.Parse(reader["Count"].ToString()),
                            FieldInn = reader["FieldInn"].ToString(),
                            FieldOkvd = reader["FieldOkvd"].ToString(),
                            FieldDate1 = reader["FieldDate1"].ToString(),
                            FieldDate2 = reader["FieldDate2"].ToString(),
                            FieldDate3 = reader["FieldDate3"].ToString(),
                            FieldPhone = reader["FieldPhone"].ToString(),
                            FieldAddress = reader["FieldAddress"].ToString(),
                            FieldContractAmount = reader["FieldContractAmount"].ToString(),
                            FieldRecord1 = reader["FieldRecord1"].ToString(),
                            FieldRecord2 = reader["FieldRecord2"].ToString(),
                            FieldRecord3 = reader["FieldRecord3"].ToString()
                        };
                        _receivers.Add(reseiver);
                    }
                }              
            }
            return _receivers;
        }

        public BindableCollection<Receiver> LoadOurReceivers(string dbPath)
        {
            BindableCollection<Receiver> receivers = new BindableCollection<Receiver>();
            if (File.Exists(dbPath))
            {
                using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    m_dbConnection.Open();
                    var sqlCommand = new SQLiteCommand(m_dbConnection);
                    sqlCommand.CommandText = "SELECT Id,Email FROM Akkaunts WHERE AkkState = 'ok'";
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var reseiver = new Receiver()
                        {
                            IdReceiver = int.Parse(reader["Id"].ToString()),
                            Email = reader["Email"].ToString(),
                        };
                        receivers.Add(reseiver);
                    }
                }
            }
            return receivers;
        }

        public bool CheckStatusOfOurReceiver(Receiver receiver, string dbPath)
        {
            bool res = false;
            if (File.Exists(dbPath))
            {
                
                using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {                   
                    m_dbConnection.Open();
                    var sqlCommand = new SQLiteCommand(m_dbConnection);
                    sqlCommand.CommandText = $"SELECT AkkState FROM Akkaunts WHERE Id = {receiver.IdReceiver.ToString()}";
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        if(reader["AkkState"].ToString() == "ok")
                        {
                            res = true;
                        }
                    }
                }
            }
            return res;
        }


        #region Private methods

        private  void CreateReceiversBb()
        {
            //Create datebase file
            SQLiteConnection.CreateFile(dbName);
            using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbName};Version=3;"))
            {
                m_dbConnection.Open();
                var sqlCommand = new SQLiteCommand(m_dbConnection);
                //create main table receivers
                sqlCommand.CommandText = "CREATE TABLE receivers (" +
                            "IdReceiver INT PRIMARY KEY," +
                            "StatusSend TEXT," +
                            "StatusEmailExist TEXT," +
                            "Email TEXT," +
                            "Hidden TEXT," +
                            "Time TEXT," +
                            "Address TEXT," +
                            "CompanyName TEXT," +
                            "PersonName TEXT," +
                            "CC TEXT," +
                            "Bcc TEXT," +
                            "Count INT," +
                            "FieldInn TEXT," +
                            "FieldOkvd TEXT," +
                            "FieldDate1 TEXT," +
                            "FieldDate2 TEXT," +
                            "FieldDate3 TEXT," +
                            "FieldPhone TEXT," +
                            "FieldAddress TEXT," +
                            "FieldContractAmount TEXT," +
                            "FieldRecord1 TEXT," +
                            "FieldRecord2 TEXT," +
                            "FieldRecord3 TEXT)";
                
                sqlCommand.ExecuteNonQuery();

                m_dbConnection.Close();
            }
                

            
        }

        private void ImportDataToDb()
        {
            using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbName};Version=3;"))
            {
                m_dbConnection.Open();

                var sqlCommand = new SQLiteCommand(m_dbConnection);
                sqlCommand.CommandText = "INSERT INTO receivers (" +
                    "IdReceiver, StatusSend, StatusEmailExist, Email, Hidden, Time, Address, CompanyName, PersonName, CC, Bcc, Count, FieldInn, FieldOkvd, FieldDate1, FieldDate2, FieldDate3, FieldPhone, FieldAddress, FieldContractAmount, FieldRecord1, FieldRecord2, FieldRecord3) " +
                    "VALUES(" +
                    "@IdReceiver, @StatusSend, @StatusEmailExist, @Email, @Hidden, @Time, @Address, @CompanyName, @PersonName, @CC, @Bcc, @Count,  @FieldInn, @FieldOkvd, @FieldDate1, @FieldDate2, @FieldDate3, @FieldPhone, @FieldAddress, @FieldContractAmount, @FieldRecord1, @FieldRecord2, @FieldRecord3)";

                #region add params to sql command

                foreach (var receiver in _receivers)
                {
                    sqlCommand.Parameters.Clear();

                    sqlCommand.Parameters.AddWithValue("@IdReceiver", receiver.IdReceiver);
                    sqlCommand.Parameters.AddWithValue("@StatusSend", receiver.StatusSend);
                    sqlCommand.Parameters.AddWithValue("@StatusEmailExist", receiver.statusEmailExist);
                    sqlCommand.Parameters.AddWithValue("@Email", receiver.Email);
                    sqlCommand.Parameters.AddWithValue("@Hidden", receiver.Hidden);
                    sqlCommand.Parameters.AddWithValue("@Time", receiver.Time);
                    sqlCommand.Parameters.AddWithValue("@Address", receiver.Address);
                    sqlCommand.Parameters.AddWithValue("@CompanyName", receiver.CompanyName);
                    sqlCommand.Parameters.AddWithValue("@PersonName", receiver.PersonName);
                    sqlCommand.Parameters.AddWithValue("@CC", receiver.CC);
                    sqlCommand.Parameters.AddWithValue("@Bcc", receiver.Bcc);
                    sqlCommand.Parameters.AddWithValue("@Count", receiver.Count);
                    sqlCommand.Parameters.AddWithValue("@FieldInn", receiver.FieldInn);
                    sqlCommand.Parameters.AddWithValue("@FieldOkvd", receiver.FieldOkvd);
                    sqlCommand.Parameters.AddWithValue("@FieldDate1", receiver.FieldDate1);
                    sqlCommand.Parameters.AddWithValue("@FieldDate2", receiver.FieldDate2);
                    sqlCommand.Parameters.AddWithValue("@FieldDate3", receiver.FieldDate3);
                    sqlCommand.Parameters.AddWithValue("@FieldPhone", receiver.FieldPhone);
                    sqlCommand.Parameters.AddWithValue("@FieldAddress", receiver.FieldAddress);
                    sqlCommand.Parameters.AddWithValue("@FieldContractAmount", receiver.FieldContractAmount);
                    sqlCommand.Parameters.AddWithValue("@FieldRecord1", receiver.FieldRecord1);
                    sqlCommand.Parameters.AddWithValue("@FieldRecord2", receiver.FieldRecord2);
                    sqlCommand.Parameters.AddWithValue("@FieldRecord3", receiver.FieldRecord3);

                    sqlCommand.ExecuteNonQuery();
                }
                #endregion

                m_dbConnection.Close();
            }

        }


        private void CreateFile(string filename)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Report");

                //Save your file
                FileInfo fi = new FileInfo(filename);
                excelPackage.SaveAs(fi);
            }
        }
        #endregion

        //add changes of receiver to db
        public void SaveReceiver(Receiver receiver)
        {
            using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbName};Version=3;"))
            {
                m_dbConnection.Open();
                var sqlCommand = new SQLiteCommand(m_dbConnection);
                sqlCommand.CommandText = "UPDATE receivers SET " +
                    "StatusSend = @StatusSend, " +
                    "StatusEmailExist = @StatusEmailExist, " +
                    "Time = @Time, " +
                    "Count = @Count " +
                    "WHERE IdReceiver = @IdReceiver";
                sqlCommand.Parameters.Clear();

                sqlCommand.Parameters.AddWithValue("@IdReceiver", receiver.IdReceiver);
                sqlCommand.Parameters.AddWithValue("@StatusSend", receiver.StatusSend);
                sqlCommand.Parameters.AddWithValue("@StatusEmailExist", receiver.StatusEmailExist);
                sqlCommand.Parameters.AddWithValue("@Time", receiver.Time);
                sqlCommand.Parameters.AddWithValue("@Count", receiver.Count);

                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    logger.ErrorSender(ex.Message);
                }
                
                m_dbConnection.Close();
            }
        }

        public void AddListToReport(string filename, ObservableCollection<Answer> letters, Receiver receiver, string serverName)
        {
            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
            {
                logger.InfoSender("Report file not exist " + filename);
                CreateFile(filename);
                logger.InfoSender("Report created " + filename);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                int rowCount = worksheet.Dimension.Rows + 1;
                foreach (var letter in letters)
                {
                    worksheet.Cells[rowCount, 1].Value = receiver?.IdReceiver.ToString() ?? "";
                    worksheet.Cells[rowCount, 2].Value = receiver?.CompanyName ?? "";

                    worksheet.Cells[rowCount, 3].Value = letter.Email;
                    worksheet.Cells[rowCount, 4].Value = letter.Subject;
                    worksheet.Cells[rowCount, 5].Value = letter.Text;
                    worksheet.Cells[rowCount, 6].Value = serverName;

                    rowCount++;
                }
                excelPackage.Save();
            }
        }

        public void AddToReport(string filename, IMailAnswer letter, Receiver receiver, string serverName)
        {
            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
            {
                logger.ErrorSender("Report file not exist " + filename);
                CreateFile(filename);
                logger.InfoSender("Report created " + filename);
            }
            try
            {
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                    //when the file empty Dimension has value null
                    int rowCount = worksheet?.Dimension?.Rows ?? 0;
                    rowCount++;

                    worksheet.Cells[rowCount, 1].Value = receiver?.IdReceiver.ToString() ?? "";
                    worksheet.Cells[rowCount, 2].Value = receiver?.CompanyName ?? "";

                    worksheet.Cells[rowCount, 3].Value = letter.From;
                    worksheet.Cells[rowCount, 4].Value = letter.EmailAddress;
                    worksheet.Cells[rowCount, 5].Value = letter.EmailSubject;
                    worksheet.Cells[rowCount, 6].Value = letter.EmailText;
                    worksheet.Cells[rowCount, 7].Value = serverName;

                    //save result
                    Byte[] bin = excelPackage.GetAsByteArray();
                    File.WriteAllBytes(filename, bin);
                    //excelPackage.Save();
                }
            }
            catch(Exception ex)
            {
                logger.ErrorReader($"AddToReport {ex.Message}");
            }
            
        }

        public void SaveChanges(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping)
        {
            using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbName};Version=3;"))
            {
                m_dbConnection.Open();
                var sqlCommand = new SQLiteCommand(m_dbConnection);
                sqlCommand.CommandText = "UPDATE receivers SET " +
                    "StatusSend = @defValue " +
                    "WHERE StatusSend = @StatusSend";
                
                sqlCommand.Parameters.Clear();
                sqlCommand.Parameters.AddWithValue("@defValue", _statuses.mailNotSend);
                sqlCommand.Parameters.AddWithValue("@StatusSend", _statuses.mailSended);

                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    logger.ErrorSender(ex.Message);
                }

                m_dbConnection.Close();
            }
        }

        public Task SaveChangesAsync(ObservableCollection<Receiver> receivers, FieldMappingSettingsModel FieldMapping)
        {
            return Task.Run(() => {
                SaveChanges(receivers, FieldMapping);
            });
        }

    }
}
