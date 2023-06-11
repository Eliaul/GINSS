using NaviTools;
using NaviTools.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.INS
{
    public class AscFileReader : FileStreamLineReader<ImuData>
    {
        public GpsTime? StartTime { get; set; }

        public GpsTime? EndTime { get; set; }

        public AscFileReader(string filePath, GpsTime? startTime = null, GpsTime? endTime = null)
            : base(filePath, 0)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public override ImuData? LineParse(string line)
        {
            if (line[0] == '#')
                return null;
            string[] lineData = line.Split(',', '*', ';');
            uint week = Convert.ToUInt32(lineData[1]);
            double secOfWeek = Convert.ToDouble(lineData[2]);
            if (StartTime != null || EndTime != null)
            {
                GpsTime time = new((int)week, secOfWeek);
                if (StartTime != null && time < StartTime)
                    return null;
                if (EndTime != null && time > EndTime)
                    return null;
            }
            int accelZ = Convert.ToInt32(lineData[6]);
            int accelY = Convert.ToInt32(lineData[7]);
            int accelX = Convert.ToInt32(lineData[8]);
            int gyroZ = Convert.ToInt32(lineData[9]);
            int gyroY = Convert.ToInt32(lineData[10]);
            int gyroX = Convert.ToInt32(lineData[11]);
            return ImuData.FromRawData(week, secOfWeek, accelZ, accelY, accelX, gyroZ, gyroY, gyroX);
        }
    }
}
