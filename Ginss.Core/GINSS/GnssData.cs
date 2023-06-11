using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Geodesy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.GINSS
{
    public record class GnssData
    {
        public GpsTime Time;

        public GeodeticCoordinate GeodeticPosition;

        public Vector<double> Velocity;

        public Vector<double> PosStd;

        public Vector<double> VelStd;

        public GnssData(GpsTime time, GeodeticCoordinate geodeticPosition, Vector<double> velocity, Vector<double> posStd, Vector<double> velStd)
        {
            Time = time;
            GeodeticPosition = geodeticPosition;
            Velocity = velocity;
            PosStd = posStd;
            VelStd = velStd;
        }


    }
}
