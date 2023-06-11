using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.Model
{
    public partial class ScatterChartModel : ObservableObject
    {
        [ObservableProperty]
        private double eastCoordinate;

        [ObservableProperty]
        private double northCoordinate;
    }
}
