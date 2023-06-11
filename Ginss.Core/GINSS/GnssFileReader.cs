using Ginss.Core.GINSS;
using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Geodesy;
using NaviTools.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ginss.Core.GINSS
{
    public partial class GnssFileReader : FileStreamLineReader<GnssData>
    {
        public GnssFileReader(string filePath) : base(filePath, 2)
        {

        }

        public override GnssData? LineParse(string line)
        {
            string[] lineData = MyRegex().Split(line.Trim());
            int week = int.Parse(lineData[0]);
            double secOfWeek = double.Parse(lineData[1]);
            GpsTime gpsTime = new(week, secOfWeek);
            double lat = double.Parse(lineData[2]);
            double lon = double.Parse(lineData[3]);
            double hei = double.Parse(lineData[4]);
            GeodeticCoordinate geodeticCoordinate = new(lat, lon, hei, Ellipsoid.WGS84, AngleUnit.deg);
            Vector<double> posStd = Vector<double>.Build.DenseOfArray(new double[]
            {
                double.Parse(lineData[6]),
                double.Parse(lineData[5]),
                double.Parse(lineData[7]),
            });
            Vector<double> velocity = Vector<double>.Build.DenseOfArray(new double[]
            {
                double.Parse(lineData[9]),
                double.Parse(lineData[8]),
                -double.Parse(lineData[10])
            });
            Vector<double> velStd = Vector<double>.Build.DenseOfArray(new double[]
            {
                double.Parse(lineData[12]),
                double.Parse(lineData[11]),
                double.Parse(lineData[13])
            });
            return new GnssData(gpsTime, geodeticCoordinate, velocity, posStd, velStd);
        }

        [GeneratedRegex("\\s+")]
        private static partial Regex MyRegex();
    }
}
