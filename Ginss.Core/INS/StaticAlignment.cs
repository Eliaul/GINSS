using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Attitude;
using NaviTools.Geodesy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.INS
{
    public class StaticAlignment
    {

        public TimeSpan StaticTimeSpan { get; init; }

        public GeodeticCoordinate OriginalPos { get; init; }

        public GpsTime OriginalEpoch { get; private set; }

        private Vector<double> DeltaAngleAve;

        private Vector<double> DeltaVelocityAve;

        private GpsTime LastTime;

        private int EpochCount;

        public bool Complete { get; private set; }

        public StaticAlignment(TimeSpan staticTimeSpan, GeodeticCoordinate originalPos)
        {
            StaticTimeSpan = staticTimeSpan;
            OriginalPos = originalPos;
            DeltaAngleAve = Vector<double>.Build.Dense(3, 0);
            DeltaVelocityAve = Vector<double>.Build.Dense(3, 0);
            EpochCount = 0;
            Complete = false;
        }

        public EulerAngle? RoughAlignmentProcessor(ImuData imuDataCur)
        {
            if (Complete)
                return null;
            EpochCount++;
            if (EpochCount == 1)
            {
                OriginalEpoch = imuDataCur.Time;
            }
            DeltaAngleAve += imuDataCur.DeltaAngle;
            DeltaVelocityAve += imuDataCur.DeltaVelocity;
            if (imuDataCur.Time < OriginalEpoch + StaticTimeSpan)
            {
                return null;
            }
            else
            {
                DeltaAngleAve /= EpochCount;
                DeltaVelocityAve /= EpochCount;
                LastTime = imuDataCur.Time;
                ImuData imuDataAve = new(LastTime, DeltaVelocityAve, DeltaAngleAve);
                Complete = true;
                return RoughAlignment(OriginalPos, imuDataAve);
            }

        }

        public static EulerAngle RoughAlignment(GeodeticCoordinate blh, ImuData imuData)
        {
            double sinLat = blh.Latitude.Sin;
            double cosLat = blh.Latitude.Cos;
            double gravity = 9.7803267715 * (1 + 0.0052790414 * sinLat * sinLat
                + 0.0000232718 * Math.Pow(sinLat, 4)) -
                (3.087691089e-6 - 4.397731e-9 * sinLat * sinLat) * blh.Height
                + 0.721e-12 * blh.Height * blh.Height;
            Vector<double> gn = Vector<double>.Build.DenseOfArray(new double[]
            {
                0, 0, gravity
            });
            Vector<double> omegaien = Vector<double>.Build.DenseOfArray(new double[]
            {
                blh.EllipsoidType.AngularVelocity * cosLat,
                0,
                -blh.EllipsoidType.AngularVelocity * sinLat
            });
            Vector<double> omegaieb = imuData.AngularVelocity;
            Vector<double> gb = -imuData.SpecificForce;
            Vector<double> vg = gn.Normalize(2);
            Vector<double> vwtmp = Extensions.CrossProduct(gn, omegaien);
            Vector<double> vw = vwtmp.Normalize(2);
            Vector<double> vgwtmp = Extensions.CrossProduct(vwtmp, gn);
            Vector<double> vgw = vgwtmp.Normalize(2);
            Vector<double> omegag = gb.Normalize(2);
            Vector<double> omegawtmp = Extensions.CrossProduct(gb, omegaieb);
            Vector<double> omegaw = omegawtmp.Normalize(2);
            Vector<double> omegagwtmp = Extensions.CrossProduct(omegawtmp, gb);
            Vector<double> omegagw = omegagwtmp.Normalize(2);
            Matrix<double> m = Matrix<double>.Build.DenseOfColumnVectors(vg, vw, vgw);
            Matrix<double> n = Matrix<double>.Build.DenseOfRowVectors(omegag, omegaw, omegagw);
            Matrix<double> rotationMatrix = m * n;
            return EulerAngle.FromRotationMatrix(rotationMatrix);
        }

    }
}
