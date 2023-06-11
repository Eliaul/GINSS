using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Wpf.Service;
using MaterialDesignThemes.Wpf;
using NaviTools;
using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ginss.Wpf.Model
{
    public class InitialPosition
    {
        private string latitude;

        [Display(Name = "纬度", Description = "初始时刻的纬度（角度）")]
        public string Latitude
        {
            get => latitude;
            set
            {
                if (double.TryParse(value, out double test))
                {
                    latitude = value;
                    CalculateService.initialPosition.Latitude = new(test, AngleUnit.deg);
                }
            }
        }

        private string longitude;

        [Display(Name = "经度", Description = "初始时刻的经度（角度）")]
        public string Longitude
        {
            get => longitude;
            set
            {
                if (double.TryParse(value, out double test))
                {
                    longitude = value;
                    CalculateService.initialPosition.Longitude = new(test, AngleUnit.deg);
                }
            }
        }

        private string height;

        [Display(Name = "大地高", Description = "初始时刻的大地高（米）")]
        public string Height
        {
            get => height;
            set
            {
                if (double.TryParse(value, out double test))
                {
                    height = value;
                    CalculateService.initialPosition.Height = test;
                }
            }
        }

        public override string ToString()
        {
            return $"{Latitude} {Longitude} {Height}";
        }
    }

    public class CalculateSettings
    {
        private string startTime;

        [Display(Name = "起始时间", Description = "开始解算的时刻，留空则不做限制。输入格式为：\r\nGPS周+空格+GPS秒\r\n示例：2222 12345.678")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.None)]
        [Category("时间设置")]
        public string StartTime
        {
            get => startTime;
            set
            {
                if (GpsTime.Check(value))
                {
                    //WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<GpsTime?>(this, nameof(StartTime), null, GpsTime.FromString(value)), "StartCalGpsTime");
                    CalculateService.startTime = GpsTime.FromString(value);
                    startTime = value;
                }
            }
        }

        private string endTime;

        [Display(Name = "结束时间", Description = "结束解算的时刻，留空则不做限制。输入格式为：\r\nGPS周+空格+GPS秒\r\n示例：2222 12345.678")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.None)]
        [Category("时间设置")]
        public string EndTime
        {
            get => endTime;
            set
            {
                if (GpsTime.Check(value))
                {
                    //WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<GpsTime?>(this, nameof(EndTime), null, GpsTime.FromString(value)), "EndCalGpsTime");
                    CalculateService.endTime = GpsTime.FromString(value);
                    endTime = value;
                }
            }
        }


        private string staticTimeSpan;

        [Display(Name = "静止时长", Description = "设置静止时长，格式如下：\r\n6 --> 6小时\r\n6:12 --> 6小时12分\r\n6:12:14 --> 6小时12分14秒")]
        [Category("初始状态参数设置")]
        public string StaticTimeSpan
        {
            get => staticTimeSpan;
            set
            {
                if (TimeSpan.TryParse(value, out CalculateService.staticTimeSpan))
                {
                    staticTimeSpan = value;
                }
            }
        }

        [Display(Name = "初始位置", Description = "设置初始时刻的位置")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.Show)]
        [Category("初始状态参数设置")]
        [ReadOnly(true)]
        public InitialPosition InitialPosition { get; set; }

        public string imuSamplingRate;

        [Display(Name = "IMU采样率")]
        [Category("IMU参数设置")]
        public string ImuSamplingRate
        {
            get => imuSamplingRate;
            set
            {
                if (double.TryParse(value, out double test))
                {
                    imuSamplingRate = value;
                    CalculateService.imuSamplingRate = test;
                }
            }
        }

        public CalculateSettings()
        {
            InitialPosition = new()
            {
                Latitude = "",
                Longitude = "",
                Height = ""
            };
        }
    }
}
