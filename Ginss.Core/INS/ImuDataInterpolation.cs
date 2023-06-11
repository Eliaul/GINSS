using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.MathTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.INS
{
    public static class ImuDataInterpolation
    {
        public static List<ImuData> Process(ImuData? imuPre, ImuData imuCur, double samplingRate)
        {
            double tolerance = 1e-4;
            if (imuPre == null)
                return new(0);
            int count = 1;
            double x1 = imuPre.Time.SecOfWeeks;
            double x2 = imuCur.Time.SecOfWeeks;
            if (Math.Abs((imuCur.Time - imuPre.Time).TotalSeconds - samplingRate) < tolerance)
                return new(0);
            List<ImuData> list = new((int)((imuCur.Time - imuPre.Time).TotalSeconds / samplingRate) + 1);
            while (x1 + count * samplingRate < x2)
            {
                Vector<double> deltaVelocityMid = Vector<double>.Build.Dense(3);
                Vector<double> deltaAngleMid = Vector<double>.Build.Dense(3);
                double secMid = x1 + count * samplingRate;
                for (int i = 0; i < 3; i++)
                {
                    deltaVelocityMid[i] = Interpolation.LinerInterpolation((x1, imuPre.DeltaVelocity[i]), (x2, imuCur.DeltaVelocity[i]), secMid);
                    deltaAngleMid[i] = Interpolation.LinerInterpolation((x1, imuPre.DeltaAngle[i]), (x2, imuCur.DeltaAngle[i]), secMid);
                }
                GpsTime timeMid = new(imuPre.Time.Weeks, secMid);
                ImuData imuMid = new(timeMid, deltaVelocityMid, deltaAngleMid);
                list.Add(imuMid);
                count++;
            }
            return list;
        }

    }
}
