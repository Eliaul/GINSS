using Ginss.Core;
using Ginss.Wpf.Model;
using Ginss.Wpf.ViewModel;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ginss.Wpf.Service
{
    public static class ExportService
    {
        public static Dictionary<string, Tuple<string, string>> propertyDictionary
            = new()
            {
                {"GPS周", new("GPS周", "GpsWeeks") },
                {"GPS周内秒", new("GPS周内秒", "GpsSeconds") },
                {"大地坐标", new("经度,纬度,高度", "GeodeticPosition") },
                {"空间直角坐标", new("ECEF-X,ECEF-Y,ECEF-Z", "CartesianPosition") },
                {"速度（N-E-D坐标系）", new("Vel-N,Vel-E,Vel-D", "Velocity") },
                {"欧拉角", new("Roll,Pitch,Yaw", "EulerAngleAtt") },
                {"UTC时（本地）", new("UTC时(+8)", "LocalTime")},
                {"姿态四元数", new("Att-w,Att-x,Att-y,Att-z", "Attitude") }
            };

        public static bool Export(List<StateOfEpoch> states, IEnumerable objects)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "导出",
                Filter = "CSV文件|*.csv"
            };
            saveFileDialog.ShowDialog();
            string path = saveFileDialog.FileName;
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            Type type = typeof(StateOfEpoch);
            List<PropertyInfo> infos = new(10);
            StringBuilder stringBuilder = new();
            var strings = objects.Cast<string>().ToList();
            for (int i = 0; i < strings.Count; i++)
            {
                stringBuilder.Append(propertyDictionary[strings[i]].Item1);
                if (i != strings.Count - 1)
                    stringBuilder.Append(',');
            }
            stringBuilder.Append("\r\n");
            foreach (var item in objects)
            {
                infos.Add(type.GetProperty(propertyDictionary[(string)item].Item2)!);
            }
            foreach (var item in states)
            {
                for (int i = 0; i < infos.Count; i++)
                {
                    var property = infos[i];
                    var value = property.GetValue(item);
                    if (property.PropertyType == typeof(Vector<double>))
                    {
                        var vec = (Vector<double>)value!;
                        stringBuilder.Append($"{vec[0]:F4},{vec[1]:F4},{vec[2]:F4}");
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        stringBuilder.Append(((DateTime)value!).ToString("yyyy/MM/dd HH:mm:ss.ffff"));
                    }
                    else
                    {
                        stringBuilder.Append(value!.ToString());
                    }
                    if (i != infos.Count - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }
                stringBuilder.Append("\r\n");
            }
            streamWriter.Write(stringBuilder.ToString());
            return true;
        }
    }
}
