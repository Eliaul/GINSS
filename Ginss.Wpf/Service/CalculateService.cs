using NaviTools;
using NaviTools.Geodesy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.Service
{
    public static class CalculateService
    {
        public static string ascFilePath;

        public static GpsTime? startTime;

        public static GpsTime? endTime;

        public static GeodeticCoordinate initialPosition;

        public static TimeSpan staticTimeSpan;

        public static double imuSamplingRate;

        static CalculateService()
        {
            initialPosition.EllipsoidType = Ellipsoid.WGS84;
        }
    }
}
