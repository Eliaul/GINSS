using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Wpf.Model;
using Ginss.Wpf.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        public object selectedCalProperty;

        public bool CanInsProcess
        {
            get
            {
                var tmp = (CalculateSettings)selectedCalProperty;
                return !string.IsNullOrEmpty(tmp.InitialPosition.Longitude)
                    && !string.IsNullOrEmpty(tmp.InitialPosition.Latitude)
                    && !string.IsNullOrEmpty(tmp.InitialPosition.Height)
                    && !string.IsNullOrEmpty(tmp.StaticTimeSpan)
                    && !string.IsNullOrEmpty(tmp.ImuSamplingRate)
                    && IsFilePathExist;
            }
        }

        public bool CanGinssProcess
        {
            get
            {
                return CanInsProcess && GinssService.gnssReader != null && GinssService.antennaLever != null;
            }
        }
    }
}
