using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Wpf.Model;
using Ginss.Wpf.Service;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Ginss.Wpf.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {

        [ObservableProperty]
        public double progress;

        public bool IsFilePathExist
        {
            get
            {
                FileInfo info = new(filePath);
                return info.Exists;
            }
        }

        public bool isEnabled = true;

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                SetProperty(ref isEnabled, value);
                OnPropertyChanged();
                WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<bool>(this, "IsEnabled", false, value), "IsProcessClick");
            }
        }

        private string filePath = "";


        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<string>, string>(this, "FilePath", ReceiveFilePath);
            WeakReferenceMessenger.Default.Register<string, string>(this, "Data process", Process);
            WeakReferenceMessenger.Default.Register<RequestMessage<bool>, string>(this, "CanProcess", (recipient, msg) =>
            {
                msg.Reply(CanProcess);
            });
            WeakReferenceMessenger.Default.Register<string, string>(this, "Data visualize", DrawResult);

            SelectedCalProperty = new CalculateSettings();
            SelectedFileProperty = new FileProperty();

            Logs = new List<LogContent>();
        }

        private async void Process(object recipient, string msg)
        {
            var canProcess = WeakReferenceMessenger.Default.Send<RequestMessage<bool>, string>("CanProcess");
            if (!canProcess || !IsFilePathExist)
            {
                MessageBox.Show("error");
                return;
            }
            IsEnabled = false;
            if (msg == "INS process")
            {
                Logs.Add(new LogContent()
                {
                    ImgName = "info.ico",
                    Description = "开始机械编排.",
                    LogTime = DateTime.Now.ToLocalTime().ToString()
                });
                Stopwatch sw = new();
                sw.Start();
                await Task.Run(() =>
                {
                    foreach (var it in InsService.Process())
                    {
                        Progress = InsService.progress;
                    }
                });
                sw.Stop();
                Logs.Add(new LogContent()
                {
                    ImgName = "info.ico",
                    Description = $"机械编排完成（用时{sw.Elapsed.TotalSeconds}秒）.",
                    LogTime = DateTime.Now.ToLocalTime().ToString()
                });
            }
            IsEnabled = true;
        }
    }
}
