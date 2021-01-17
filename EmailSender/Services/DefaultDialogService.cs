using System;
using System.Collections.Generic;
using System.Text;
using EmailSender.Interfaces;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace EmailSender.Services
{
    public class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "files (.*xlsx;*.txt;*.sqlite;*.db)|*.xlsx;*.txt;*.sqlite;*.db|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "exel files(.*xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public string OpenFolder()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Выберете папку для сохранения отчетов.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if ((bool)dialog.ShowDialog())
            {
                return dialog.SelectedPath;
            }
            return "";
        }
    }
}
