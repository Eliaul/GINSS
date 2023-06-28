using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Core;
using Ginss.Wpf.Model;
using Ginss.Wpf.Service;
using Ginss.Wpf.View;
using MaterialDesignThemes.Wpf;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panuon.WPF.UI;
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
                return CalculateService.ascFilePath != null;
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

        public List<StateOfEpoch> States { get; set; }

        public bool IsShown => throw new NotImplementedException();

        public int Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<string>, string>(this, "FilePath", ReceiveFilePath);
            WeakReferenceMessenger.Default.Register<string, string>(this, "Data process", Process);
            WeakReferenceMessenger.Default.Register<string, string>(this, "Data export", Export);
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<int>, string>(this, "Chart selectedIndex", ShowInfo);
            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<ObservableCollection<string>>, string>(this, "Data visualize", DrawResult);

            SelectedCalProperty = new CalculateSettings();
            SelectedFileProperty = new FileProperty();
        }

        private async void Process(object recipient, string msg)
        {
            if (msg == "INS process")
            {
                if (!CanInsProcess)
                {
                    Logs.Add(new()
                    {
                        Description = "不能开始机械编排！（可能是由于缺少文件或者初值）",
                        LogTime = DateTime.Now.ToLocalTime().ToString(),
                        IconKind = PackIconKind.AlertCircle,
                        Brush = Brushes.OrangeRed
                    });
                    Dialog dialog = new()
                    {
                        DataContext = new DialogViewModel()
                        {
                            IconKind = PackIconKind.AlertCircleOutline,
                            Brush = Brushes.DarkRed,
                            Title = "Error",
                            Message = "不能开始机械编排！"
                        }
                    };
                    dialog.ShowDialog();
                    return; 
                }
                IsEnabled = false;
                ClearChartData();
                Logs.Add(new LogContent()
                {
                    Description = "开始机械编排.",
                    LogTime = DateTime.Now.ToLocalTime().ToString()
                });
                States?.Clear();
                Stopwatch sw = new();
                sw.Start();
                await Task.Run(() =>
                {
                    var res = InsService.Process();
                    States ??= new();
                    foreach (var it in res)
                    {
                        Progress = InsService.progress;
                        States.Add(it);
                    }
                });
                sw.Stop();
                Logs.Add(new LogContent()
                {
                    Description = $"机械编排完成（用时{sw.Elapsed.TotalSeconds}秒）.",
                    LogTime = DateTime.Now.ToLocalTime().ToString()
                });
            }
            else if (msg == "GNSS/INS process")
            {
                if (!CanGinssProcess)
                {
                    Logs.Add(new()
                    {
                        Description = "不能开始松组合！（可能是由于缺少文件或者初值）",
                        LogTime = DateTime.Now.ToLocalTime().ToString(),
                        IconKind = PackIconKind.AlertCircle,
                        Brush = Brushes.OrangeRed
                    });
                    Dialog dialog = new()
                    {
                        DataContext = new DialogViewModel()
                        {
                            IconKind = PackIconKind.AlertCircleOutline,
                            Brush = Brushes.DarkRed,
                            Title = "Warning",
                            Message = "不能开始松组合！"
                        }
                    };
                    dialog.ShowDialog();
                    return;
                }
                IsEnabled = false;
                ClearChartData();
                Logs.Add(new LogContent()
                {
                    Description = "开始松组合.",
                    LogTime = DateTime.Now.ToLocalTime().ToString()
                });
                States?.Clear();
                Stopwatch sw = new();
                sw.Start();
                await Task.Run(() =>
                {
                    var res = GinssService.Process();
                    States ??= new();
                    foreach (var it in res)
                    {
                        Progress = GinssService.progress;
                        States.Add(it);
                    }
                });
                sw.Stop();
                Logs.Add(new LogContent()
                {
                    Description = $"松组合完成（用时{sw.Elapsed.TotalSeconds}秒）.",
                    LogTime = DateTime.Now.ToLocalTime().ToString()
                });
            }
            IsEnabled = true;
        }
    }
}
