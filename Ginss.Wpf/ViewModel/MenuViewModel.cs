using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Wpf.Service;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ginss.Wpf.ViewModel
{
    public partial class MenuViewModel : ObservableObject
    {
        private string filePath = "";

        public string FilePath
        {
            get => filePath;
            set
            {
                if (SetProperty(ref filePath, value))
                {
                    WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<string>(this, nameof(FilePath), "", value), nameof(FilePath));
                    CalculateService.filePath = value;
                }
            }
        }

        [ObservableProperty]
        private bool isEnabled = true;

        [RelayCommand]
        public void FileDialog()
        {
            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = "C:\\Desktop",
                Filter = "ASC文件|*.ASC",
                RestoreDirectory = true
            };
            openFileDialog.ShowDialog();
            FilePath = openFileDialog.FileName;
        }

        [ObservableProperty]
        private bool isProcessEnabled = true;

        [RelayCommand]
        public void InsProcess()
        {
            WeakReferenceMessenger.Default.Send("INS process", "Data process");
        }


        [RelayCommand]
        public void GinsProcess()
        {
            //WeakReferenceMessenger.Default.Send("GNSS/INS process", "Data process");
        }

        [RelayCommand]
        public void Draw()
        {
            WeakReferenceMessenger.Default.Send("Data", "Data visualize");
        }

        public MenuViewModel()
        {
            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<bool>, string>(this, "IsProcessClick", (r, m) =>
            {
                IsEnabled = m.NewValue;
            });
        }

        private void ReceiveProcessState(object recipient, PropertyChangedMessage<bool> msg)
        {
            //IsProcessEnabled = !msg.NewValue;
        }
    }
}
