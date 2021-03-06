﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.Interfaces
{
    interface IDialogService
    {
        string FilePath { get; set; }   // путь к выбранному файлу
        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog();  // сохранение файла
        string OpenFolder();
    }
}
