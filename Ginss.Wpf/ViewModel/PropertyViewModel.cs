using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ginss.Wpf.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows.Shapes;
using NaviTools;
using Syncfusion.Windows.PropertyGrid;
using Ginss.Wpf.Service;

namespace Ginss.Wpf.ViewModel
{


    public partial class MainWindowViewModel : ObservableObject
    {

        [ObservableProperty]
        public object selectedFileProperty;

        private void ReceiveFilePath(object recipient, PropertyChangedMessage<string> message)
        {
            filePath = message.NewValue;
            FileInfo info = new(message.NewValue);
            if (info.Extension.ToUpperInvariant() == ".ASC")
            {
                var it = File.ReadLines(message.NewValue);
                var firstLine = it.First();
                var lastLine = it.Last();
                string[] firstLineData = firstLine.Split(',', '*', ';');
                string[] lastLineData = lastLine.Split(',', '*', ';');
                GpsTime firstGpsTime = new(Convert.ToInt32(firstLineData[1]), Convert.ToDouble(firstLineData[2]));
                GpsTime lastGpsTime = new(Convert.ToInt32(lastLineData[1]), Convert.ToDouble(lastLineData[2]));
                SelectedFileProperty = new FileProperty()
                {
                    FilePath = info.FullName,
                    FileName = info.Name,
                    FileExtension = info.Extension,
                    OpenTime = info.LastAccessTime.ToLocalTime(),
                    FileTimeInfo = new()
                    {
                        StartTime = firstGpsTime.LocalTimePoint.DateTime,
                        StartGpsTime = firstGpsTime.ToString(),
                        EndTime = lastGpsTime.LocalTimePoint.DateTime,
                        EndGpsTime = lastGpsTime.ToString()
                    }
                };
            }
            Logs.Add(new LogContent()
            {
                ImgName = "info.ico",
                Description = $"文件\"{filePath}\"读取成功.",
                LogTime = DateTime.Now.ToLocalTime().ToString()
            });
        }
    }


}
