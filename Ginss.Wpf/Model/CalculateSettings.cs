using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Ginss.Wpf.Service;
using MaterialDesignThemes.Wpf;
using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

#nullable disable

namespace Ginss.Wpf.Model
{
    public static class ParseDoubleArray
    {
        public static bool TryParse(string? s, out Vector<double> vector)
        {
            vector = Vector<double>.Build.Dense(3, 0);
            if (s == null)
            {
                return false;
            }
            var nums = s.Split(' ', StringSplitOptions.TrimEntries);
            if (nums.Length != 3)
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                if (double.TryParse(nums[i], out double test))
                {
                    vector[i] = test;
                }
                else
                    return false;
            }
            return true;
        }
    }

    public class GyroErrorModel
    {
        private string initialBias;

        [Display(Name = "初值零偏", Description = "单位：deg/s")]
        public string InitialBias
        {
            get => initialBias;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    initialBias = value;
                    GinssService.gyroErrorModel.Bias = vector;
                }
            }
        }

        private string initialScaleFactor;

        [Display(Name = "初值比例因子")]
        public string InitialScaleFactor
        {
            get => initialScaleFactor;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    initialScaleFactor = value;
                    GinssService.gyroErrorModel.ScaleFactor = vector;
                }
            }
        }

        private string angularRandomWalk;

        [Display(Name = "角度随机游走", Description = "单位：rad/s/sqrt(s)")]
        public string AngularRandomWalk
        {
            get => angularRandomWalk;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    angularRandomWalk = value;
                    GinssService.gyroErrorModel.ARW = vector;
                }
            }
        }

        private string biasProcessNoise;

        [Display(Name = "零偏过程噪声", Description = "单位：rad/s")]
        public string BiasProcessNoise
        {
            get => biasProcessNoise;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    biasProcessNoise = value;
                    GinssService.gyroErrorModel.BiasProcessNoise = vector;
                }
            }
        }

        private string scaleFactorProcessNoise;

        [Display(Name = "比例因子过程噪声")]
        public string ScaleFactorProcessNoise
        {
            get => scaleFactorProcessNoise;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    scaleFactorProcessNoise = value;
                    GinssService.gyroErrorModel.ScaleFactorProcessNoise = vector;
                }
            }
        }

        private string biasRelevantTime;

        [Display(Name = "零偏相关时间", Description = "单位：s")]
        public string BiasRelevantTime
        {
            get => biasRelevantTime;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    biasRelevantTime = value;
                    GinssService.gyroErrorModel.BiasRelevantTime = vector;
                }
            }
        }

        private string scaleFactorRelevantTime;

        [Display(Name = "比例因子相关时间", Description = "单位：s")]
        public string ScaleFactorRelevantTime
        {
            get => scaleFactorRelevantTime;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    scaleFactorRelevantTime = value;
                    GinssService.gyroErrorModel.ScaleFactorRelevantTime = vector;
                }
            }
        }
    }

    public class AcceErrorModel
    {
        private string initialBias;

        [Display(Name = "初值零偏", Description = "单位：m/s^2")]
        public string InitialBias
        {
            get => initialBias;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    initialBias = value;
                    GinssService.acceErrorModel.Bias = vector;
                }
            }
        }

        private string initialScaleFactor;

        [Display(Name = "初值比例因子")]
        public string InitialScaleFactor
        {
            get => initialScaleFactor;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    initialScaleFactor = value;
                    GinssService.acceErrorModel.ScaleFactor = vector;
                }
            }
        }

        private string velocityRandomWalk;

        [Display(Name = "速度随机游走", Description = "单位：m/s^2/sqrt(s)")]
        public string VelocityRandomWalk
        {
            get => velocityRandomWalk;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    velocityRandomWalk = value;
                    GinssService.acceErrorModel.VRW = vector;
                }
            }
        }

        private string biasProcessNoise;

        [Display(Name = "零偏过程噪声", Description = "单位：m/s")]
        public string BiasProcessNoise
        {
            get => biasProcessNoise;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    biasProcessNoise = value;
                    GinssService.acceErrorModel.BiasProcessNoise = vector;
                }
            }
        }

        private string scaleFactorProcessNoise;

        [Display(Name = "比例因子过程噪声")]
        public string ScaleFactorProcessNoise
        {
            get => scaleFactorProcessNoise;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    scaleFactorProcessNoise = value;
                    GinssService.acceErrorModel.ScaleFactorProcessNoise = vector;
                }
            }
        }

        private string biasRelevantTime;

        [Display(Name = "零偏相关时间", Description = "单位：s")]
        public string BiasRelevantTime
        {
            get => biasRelevantTime;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    biasRelevantTime = value;
                    GinssService.acceErrorModel.BiasRelevantTime = vector;
                }
            }
        }

        private string scaleFactorRelevantTime;

        [Display(Name = "比例因子相关时间", Description = "单位：s")]
        public string ScaleFactorRelevantTime
        {
            get => scaleFactorRelevantTime;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    scaleFactorRelevantTime = value;
                    GinssService.acceErrorModel.ScaleFactorRelevantTime = vector;
                }
            }
        }
    }

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

        [Display(Name = "IMU采样率", Description = "单位为秒.")]
        [Category("IMU参数及误差模型")]
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

        [ReadOnly(true)]
        [Display(Name = "陀螺仪误差模型", Description = "设置陀螺仪相关的误差")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.Show)]
        [Category("IMU参数及误差模型")]
        public GyroErrorModel GyroErrorModel { get; set; }

        [ReadOnly(true)]
        [Display(Name = "加速度计误差模型", Description = "设置加速度计相关的误差")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.Show)]
        [Category("IMU参数及误差模型")]
        public AcceErrorModel AcceErrorModel { get; set; }

        private string antennaLever;

        [Display(Name = "杆臂", Description = "设置杆臂向量（单位m）")]
        public string AntennaLever
        {
            get => antennaLever;
            set
            {
                if (ParseDoubleArray.TryParse(value, out var vector))
                {
                    antennaLever = value;
                    GinssService.antennaLever = vector;
                }
            }
        }

        public CalculateSettings()
        {
            InitialPosition = new()
            {
                Latitude = "30.5278108948",
                Longitude = "114.3557126173",
                Height = "22.321"
            };
            GyroErrorModel = new()
            {
                InitialBias = "0 0 0",
                InitialScaleFactor = "0 0 0",
                AngularRandomWalk = "5.82e-5 5.82e-5 5.82e-5",
                BiasProcessNoise = "1.16e-5 1.16e-5 1.16e-5",
                ScaleFactorProcessNoise = "1e-3 1e-3 1e-3",
                BiasRelevantTime = "3600 3600 3600",
                ScaleFactorRelevantTime = "3600 3600 3600"
            };
            AcceErrorModel = new()
            {
                InitialBias = "0 0 0",
                InitialScaleFactor = "0 0 0",
                VelocityRandomWalk = "6.67e-3 6.67e-3 6.67e-3",
                BiasProcessNoise = "4e-3 4e-3 4e-3",
                ScaleFactorProcessNoise = "1e-3 1e-3 1e-3",
                BiasRelevantTime = "3600 3600 3600",
                ScaleFactorRelevantTime = "3600 3600 3600"
            };
            AntennaLever = new("-0.1000 0.2350 -0.8900");
            ImuSamplingRate = "0.01";
            StaticTimeSpan = "0:5";
        }
    }
}
