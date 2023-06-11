using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.INS
{
    /// <summary>
    /// The body relative directions is forward-right-down.
    /// </summary>
    public record class ImuData : IEquatable<ImuData>
    {
        public const double AccelerationScaleFactor = 0.05 / 32768;

        public const double GyroscopeScaleFactor = 0.1 / (3600 * 256);

        public static double SamplingRate { get; set; }

        public GpsTime Time { get; init; }

        public Vector<double> DeltaVelocity { get; private set; }

        public Vector<double> DeltaAngle { get; private set; }

        public Vector<double> SpecificForce => DeltaVelocity / SamplingRate;

        public Vector<double> AngularVelocity => DeltaAngle / SamplingRate;

        public ImuData(GpsTime time, Vector<double> accData, Vector<double> gyroData)
        {
            if (accData.Count != 3 || gyroData.Count != 3)
                throw new VectorLengthException();
            Time = time;
            DeltaVelocity = accData;
            DeltaAngle = gyroData;
        }

        /// <summary>
        /// Correct the output based on the known amount of IMU error.
        /// </summary>
        /// <param name="gyroBias">The bias of gyroscope.</param>
        /// <param name="gyroScaleFactor">The scale factor of gyroscope.</param>
        /// <param name="acceBias">The bias of accelerometer.</param>
        /// <param name="acceScaleFactor">The scale factor of accelerometer.</param>
        public void Compensate(Vector<double> gyroBias, Vector<double> gyroScaleFactor, Vector<double> acceBias, Vector<double> acceScaleFactor)
        {
            DeltaAngle = (DeltaAngle - gyroBias * SamplingRate) / (Vector<double>.Build.Dense(3, 1) + gyroScaleFactor);
            DeltaVelocity = (DeltaVelocity - acceBias * SamplingRate) / (Vector<double>.Build.Dense(3, 1) + acceScaleFactor);
        }

        public static ImuData FromRawData(
            uint gpsWeek, double secOfWeek, 
            int accelZ, int accelY, int accelX,
            int gyroZ, int gyroY, int gyroX)
        {
            GpsTime time = new((int)gpsWeek, secOfWeek);
            Vector<double> accData = Vector<double>.Build.DenseOfArray(new double[] 
            {
                - accelY * AccelerationScaleFactor,
                accelX * AccelerationScaleFactor,
                - accelZ * AccelerationScaleFactor
            });
            Vector<double> gyroData = Vector<double>.Build.DenseOfArray(new double[]
            {
                - gyroY * GyroscopeScaleFactor,
                gyroX * GyroscopeScaleFactor,
                - gyroZ * GyroscopeScaleFactor
            });
            return new ImuData(time, accData, gyroData);
        }

        /// <summary>
        /// Use linear interpolation to obtain a virtual output.
        /// </summary>
        /// <param name="imuPre">The output of previous moment.</param>
        /// <param name="imuCur">The output of current moment.</param>
        /// <param name="gpsTime">The time required for interpolation.</param>
        /// <returns></returns>
        public static ImuData VirtualOutput(ImuData imuPre, ImuData imuCur, GpsTime gpsTime)
        {
            var factor = ((gpsTime - imuPre.Time).TotalSeconds / (imuCur.Time - imuPre.Time).TotalSeconds);
            var deltaVel = imuCur.DeltaVelocity * factor;
            var deltaAng = imuCur.DeltaAngle * factor;
            imuCur.DeltaVelocity -= deltaVel;
            imuCur.DeltaAngle -= deltaAng;
            return new(gpsTime, deltaVel, deltaAng);
        }
    }
}
