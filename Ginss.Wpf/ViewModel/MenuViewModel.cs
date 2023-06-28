using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Wpf.Service;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                Filter = "ASC文件|*.ASC|POS文件|*.pos",
                RestoreDirectory = true
            };
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
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
            WeakReferenceMessenger.Default.Send("GNSS/INS process", "Data process");
        }

        [RelayCommand]
        public void Draw()
        {
            //WeakReferenceMessenger.Default.Send("Data", "Data visualize");
            WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<ObservableCollection<string>>(this, "", new(), SelectedItems), "Data visualize");
        }

        [RelayCommand]
        public void Export()
        {
            WeakReferenceMessenger.Default.Send("Export", "Data export");
        }

        public MenuViewModel()
        {
            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<bool>, string>(this, "IsProcessClick", (r, m) =>
            {
                IsEnabled = m.NewValue;
            });

            SelectedItems = new();
        }

        private ObservableCollection<string> chartType = new()
        {
            "轨迹图",
            "大地坐标时序图",
            "直角坐标系时序图",
            "欧拉角时序图",
            "速度时序图"
        };

        public ObservableCollection<string> ChartType
        {
            get => chartType;
            set
            {
                chartType = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> selectedItems;

        public ObservableCollection<string> SelectedItems
        {
            get => selectedItems;
            set
            {
                selectedItems = value;
                OnPropertyChanged();
            }
        }
    }
}
