using NaviTools;
using Syncfusion.Windows.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.Model
{
    [Editor(typeof(DateTime), typeof(CustomDateTimeEditor))]
    [Editor("StartGpsTime", typeof(GpsTimeEditor))]
    [Editor("EndGpsTime", typeof(GpsTimeEditor))]
    public class TimeInfo
    {
        [ReadOnly(true)]
        [Display(Name = "起始时间")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.None)]
        public DateTime StartTime { get; set; } = DateTime.MinValue;

        [ReadOnly(true)]
        [Display(Name = "结束时间")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.None)]
        public DateTime EndTime { get; set;} = DateTime.MinValue;

        [ReadOnly(true)]
        [Display(Name = "起始时间（GPS周秒）")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.None)]
        public string StartGpsTime { get; set; }

        [ReadOnly(true)]
        [Display(Name = "结束时间（GPS周秒）")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.None)]
        public string EndGpsTime { get; set; }

        public override string ToString()
        {
            return StartTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + ", " + EndTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }
    }

    [Editor(typeof(DateTime), typeof(CustomDateTimeEditor))]
    public class FileProperty
    {
        [ReadOnly(true)]
        [DisplayName("文件名")]
        public string FileName { get; set; }

        [ReadOnly(true)]
        [DisplayName("文件路径")]
        public string FilePath { get; set; }

        [ReadOnly(true)]
        [DisplayName("文件类型")]
        public string FileExtension { get; set; }

        [ReadOnly(true)]
        [DisplayName("打开时间")]
        [PropertyGrid(NestedPropertyDisplayMode = NestedPropertyDisplayMode.None)]
        public DateTime OpenTime { get; set; } = DateTime.MinValue;

        [ReadOnly(true)]
        [DisplayName("文件时间信息")]
        public TimeInfo FileTimeInfo { get; set; }
    }
}
