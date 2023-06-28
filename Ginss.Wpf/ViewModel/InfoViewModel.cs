using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Core;
using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Attitude;
using NaviTools.Geodesy;
using Syncfusion.Windows.PropertyGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.ViewModel
{
    
    [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.Show, PropertyType = typeof(Angle))]
    [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.Show, PropertyType = typeof(GeodeticCoordinate))]
    [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.Show, PropertyType = typeof(CartesianCoordinate))]
    public class StatesOfEpochInfo
    {
        [Display(Name = "GPS时", Description = "周+周内秒")]
        public string GpsTime { get; }

        [Display(Name = "本地时", Description = "年/月/日 时:分:秒（UTC偏移量）")]
        public string LocalTime { get; }

        [ReadOnly(true)]
        [Display(Name = "大地坐标", Description = "经度（°），纬度（°），高程（m）")]
        public ArrayList GeodeticCoordinate { get; }

        [ReadOnly(true)]
        [Display(Name = "直角坐标", Description = "单位（m）")]
        public CartesianCoordinate CartesianCoordinate { get; }

        [ReadOnly(true)]
        [Display(Name = "N-E-D速度", Description = "单位（m）")]
        public Vector<double> Velocity { get; }

        [Display(Name = "姿态欧拉角", Description = "单位（°）")]
        public EulerAngle Attitude { get; }

        public StatesOfEpochInfo(StateOfEpoch states)
        {
            GpsTime = states.Time.ToString();
            LocalTime = states.Time.LocalTimePoint.ToString("yyyy:MM:dd HH:mm:ss.ffff zz");
            GeodeticCoordinate = new ArrayList()
            {
                states.GeodeticPosition.Longitude, states.GeodeticPosition.Latitude, states.GeodeticPosition.Height
            };
            CartesianCoordinate = states.CartesianPosition;
            Velocity = states.Velocity;
            Attitude = states.EulerAngleAtt;
        }
    }

    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        public object selectedInfoProperty;

        public void ShowInfo(object recipient, ValueChangedMessage<int> msg)
        {
            var item = States[msg.Value];
            SelectedInfoProperty = new StatesOfEpochInfo(item);
        }
    }
}
