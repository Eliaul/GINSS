using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Wpf.Model;
using Ginss.Wpf.View;
using NaviTools.Geodesy;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ginss.Wpf.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {

        public ObservableCollection<object> Charts { get; set; } = new();

        public ObservableCollection<ScatterChartModel> LocalCoordinates { get; set; } = new();

        public ObservableCollection<LineChartModel> LatitudeData { get; set; } = new();

        public ObservableCollection<LineChartModel> LongtitudeData { get; set; } = new();

        public ObservableCollection<LineChartModel> HeightData { get; set; } = new();

        public ObservableCollection<LineChartModel> Roll { get; set; } = new();

        public ObservableCollection<LineChartModel> Pitch { get; set; } = new();

        public ObservableCollection<LineChartModel> Yaw { get; set; } = new();

        public ObservableCollection<LineChartModel> VelN { get; set; } = new();

        public ObservableCollection<LineChartModel> VelE { get; set; } = new();

        public ObservableCollection<LineChartModel> VelD { get; set; } = new();

        public ObservableCollection<LineChartModel> X { get; set; } = new();

        public ObservableCollection<LineChartModel> Y { get; set; } = new();

        public ObservableCollection<LineChartModel> Z { get; set; } = new();

        public void ClearChartData()
        {
            Charts.Clear();
            LocalCoordinates.Clear();
            LatitudeData.Clear();
            LongtitudeData.Clear();
            HeightData.Clear();
            Roll.Clear();
            Pitch.Clear();
            Yaw.Clear();
            VelN.Clear();
            VelE.Clear();
            VelD.Clear();
            X.Clear();
            Y.Clear();
            Z.Clear();
        }

        public void SetWindow(DependencyObject dependencyObject, string header)
        {
            DocumentContainer.SetCanClose(dependencyObject, false);
            DocumentContainer.SetMDIWindowState(dependencyObject, MDIWindowState.Minimized);
            DocumentContainer.SetHeader(dependencyObject, header);
        }

        private void DrawResult(object recipient, PropertyChangedMessage<ObservableCollection<string>> msg)
        {
            if (msg.NewValue.Contains("轨迹图") && States != null)
            {
                if (LocalCoordinates.Count == 0)
                {
                    foreach (var item in States)
                    {
                        var localCoordinate = LocalCoordinate.FromCartesianCoordinate(item.CartesianPosition, States[0].CartesianPosition, Ellipsoid.WGS84);
                        LocalCoordinates.Add(new()
                        {
                            EastCoordinate = localCoordinate.East,
                            NorthCoordinate = localCoordinate.North
                        });
                    }
                    var chartView = new ScatterChartView();
                    SetWindow(chartView, "轨迹图");
                    Charts.Add(chartView);
                }

            }
            if (msg.NewValue.Contains("大地坐标时序图") && States != null)
            {
                if (LatitudeData.Count == 0)
                {
                    foreach (var item in States)
                    {
                        LatitudeData.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.GeodeticPosition.Latitude.Degree
                        });
                        LongtitudeData.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.GeodeticPosition.Longitude.Degree
                        });
                        HeightData.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.GeodeticPosition.Height
                        });
                    }
                    var latitudeChartView = new LineChartView();
                    var longitudeChartView = new LineChartView();
                    var heightChartView = new LineChartView();
                    latitudeChartView.lineSeries.ItemsSource = LatitudeData;
                    longitudeChartView.lineSeries.ItemsSource = LongtitudeData;
                    heightChartView.lineSeries.ItemsSource = HeightData;
                    latitudeChartView.xAxis.Header = "GPS秒（s）";
                    longitudeChartView.xAxis.Header = "GPS秒（s）";
                    heightChartView.xAxis.Header = "GPS秒（s）";
                    latitudeChartView.yAxis.Header = "纬度（°）";
                    longitudeChartView.yAxis.Header = "经度（°）";
                    heightChartView.yAxis.Header = "高程（m）";
                    SetWindow(latitudeChartView, "纬度");
                    SetWindow(longitudeChartView, "经度");
                    SetWindow(heightChartView, "高程");
                    Charts.Add(latitudeChartView);
                    Charts.Add(longitudeChartView);
                    Charts.Add(heightChartView);
                }
            }
            if (msg.NewValue.Contains("直角坐标系时序图") && States != null)
            {
                if (X.Count == 0)
                {
                    foreach (var item in States)
                    {
                        X.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.GeodeticPosition.Latitude.Degree
                        });
                        Y.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.GeodeticPosition.Longitude.Degree
                        });
                        Z.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.GeodeticPosition.Height
                        });
                    }
                    var xChartView = new LineChartView();
                    var yChartView = new LineChartView();
                    var zChartView = new LineChartView();
                    xChartView.lineSeries.ItemsSource = X;
                    yChartView.lineSeries.ItemsSource = Y;
                    zChartView.lineSeries.ItemsSource = Z;
                    xChartView.xAxis.Header = "GPS秒（s）";
                    yChartView.xAxis.Header = "GPS秒（s）";
                    zChartView.xAxis.Header = "GPS秒（s）";
                    xChartView.yAxis.Header = "ECEF-X（m）";
                    yChartView.yAxis.Header = "ECEF-Y（m）";
                    zChartView.yAxis.Header = "ECEF-Z（m）";
                    SetWindow(xChartView, "ECEF-X");
                    SetWindow(yChartView, "ECEF-Y");
                    SetWindow(zChartView, "ECEF-Z");
                    Charts.Add(xChartView);
                    Charts.Add(yChartView);
                    Charts.Add(zChartView);
                }

            }
            if (msg.NewValue.Contains("欧拉角时序图") && States != null)
            {
                if (Roll.Count == 0)
                {
                    foreach (var item in States)
                    {
                        Roll.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.EulerAngleAtt.Roll.Degree
                        });
                        Pitch.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.EulerAngleAtt.Pitch.Degree
                        });
                        Yaw.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.EulerAngleAtt.Yaw.Degree
                        });
                    }
                    var rollChartView = new LineChartView();
                    var pitchChartView = new LineChartView();
                    var yawChartView = new LineChartView();
                    rollChartView.lineSeries.ItemsSource = Roll;
                    pitchChartView.lineSeries.ItemsSource = Pitch;
                    yawChartView.lineSeries.ItemsSource = Yaw;
                    rollChartView.xAxis.Header = "GPS秒（s）";
                    pitchChartView.xAxis.Header = "GPS秒（s）";
                    yawChartView.xAxis.Header = "GPS秒（s）";
                    rollChartView.yAxis.Header = "横滚（°）";
                    pitchChartView.yAxis.Header = "俯仰（°）";
                    yawChartView.yAxis.Header = "航向（°）";
                    SetWindow(rollChartView, "横滚角");
                    SetWindow(pitchChartView, "俯仰角");
                    SetWindow(yawChartView, "航向角");
                    Charts.Add(rollChartView);
                    Charts.Add(pitchChartView);
                    Charts.Add(yawChartView);
                }


            }
            if (msg.NewValue.Contains("速度时序图") && States != null)
            {
                if (VelN.Count == 0)
                {
                    foreach (var item in States)
                    {
                        VelN.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.Velocity[0]
                        });
                        VelE.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.Velocity[1]
                        });
                        VelD.Add(new()
                        {
                            X = item.Time.SecOfWeeks,
                            Y = item.Velocity[2]
                        });
                    }
                    var velNChartView = new LineChartView();
                    var velEChartView = new LineChartView();
                    var velDChartView = new LineChartView();
                    velNChartView.lineSeries.ItemsSource = VelN;
                    velEChartView.lineSeries.ItemsSource = VelE;
                    velDChartView.lineSeries.ItemsSource = VelD;
                    velNChartView.xAxis.Header = "GPS秒（s）";
                    velEChartView.xAxis.Header = "GPS秒（s）";
                    velDChartView.xAxis.Header = "GPS秒（s）";
                    velNChartView.yAxis.Header = "北向速度（m/s）";
                    velEChartView.yAxis.Header = "东向速度（m/s）";
                    velDChartView.yAxis.Header = "地向速度（m/s）";
                    SetWindow(velNChartView, "北向速度");
                    SetWindow(velEChartView, "东向速度");
                    SetWindow(velDChartView, "地向速度");
                    Charts.Add(velNChartView);
                    Charts.Add(velEChartView);
                    Charts.Add(velDChartView);
                }


            }
        }

    }
}
