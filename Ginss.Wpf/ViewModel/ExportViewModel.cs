using CommunityToolkit.Mvvm.ComponentModel;
using Ginss.Wpf.Model;
using Ginss.Wpf.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {


        public ObservableCollection<string> CanExportItems { get; set; }
        = new() {
            "UTC时（本地）",
            "姿态四元数"
        };

        public ObservableCollection<string> ExprotedItems { get; set; }
        = new() {
            "GPS周", "GPS周内秒", "大地坐标",
            "空间直角坐标", "速度（N-E-D坐标系）", "欧拉角"
        };

        public void Export(object recipient, string msg)
        {
            ExportWindow exportWindow = new()
            {
                DataContext = this
            };
            exportWindow.ShowDialog();
            var items = exportWindow.exportedBox.ItemsSource;
            if (States != null)
            {
                if (ExportService.Export(States, items))
                {
                    Logs.Add(new LogContent()
                    {
                        Description = "导出文件成功.",
                        LogTime = DateTime.Now.ToLocalTime().ToString()
                    });
                    Dialog dialog = new()
                    {
                        DataContext = new DialogViewModel()
                        {
                            Title = "Info",
                            Message = "导出成功！"
                        }
                    };
                    dialog.ShowDialog();
                }

            }
        }
    }
}
