using CommunityToolkit.Mvvm.ComponentModel;
using Ginss.Wpf.View;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ginss.Wpf.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {

        public List<ScatterChartView> Charts { get; set; } = new();

        private void DrawResult(object recipient, string msg)
        {
            if (msg == "Data")
            {
                Charts.Add(new ScatterChartView());
            }
        }

    }
}
