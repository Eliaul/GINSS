using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ginss.Wpf.View
{
    /// <summary>
    /// ScatterChartView.xaml 的交互逻辑
    /// </summary>
    public partial class ScatterChartView : UserControl
    {
        public ScatterChartView()
        {
            InitializeComponent();
        }

        private void chart_SelectionChanged(object sender, Syncfusion.UI.Xaml.Charts.ChartSelectionChangedEventArgs e)
        {
            if (e.SelectedIndex != -1)
            {
                WeakReferenceMessenger.Default.Send<ValueChangedMessage<int>, string>(new ValueChangedMessage<int>(e.SelectedIndex), "Chart selectedIndex");
            }
        }
    }
}
