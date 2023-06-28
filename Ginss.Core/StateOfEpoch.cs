using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Attitude;
using NaviTools.Geodesy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core
{
    public record class StateOfEpoch : IFormattable
    {
        public GpsTime Time { get; set; }

        public Quaternion Attitude { get; set; }

        public GeodeticCoordinate geodeticPosition;

        public GeodeticCoordinate GeodeticPosition
        {
            get => geodeticPosition;
            set => geodeticPosition = value;
        }

        public Vector<double> Velocity { get; set; }

        public Vector<double> Omegaenn { get; set; }

        public Vector<double> Omegaien { get; set; }

        public Vector<double> Gravity { get; set; }

        public EulerAngle EulerAngleAtt => EulerAngle.FromQuaternion(Attitude);

        public CartesianCoordinate CartesianPosition => CartesianCoordinate.FromGeodeticCoordinate(GeodeticPosition);

        public double GpsSeconds => Time.SecOfWeeks;

        public double GpsWeeks => Time.Weeks;

        public DateTime LocalTime => Time.LocalTimePoint.LocalDateTime;

        public StateOfEpoch(GpsTime time, Quaternion attitude, GeodeticCoordinate geodeticCoordinate, Vector<double> velocity)
        {
            if (velocity.Count != 3)
                throw new ArgumentException("The velocity vector's length must be 3.", nameof(velocity));
            Time = time;
            Attitude = attitude;
            GeodeticPosition = geodeticCoordinate;
            Velocity = velocity;
            double RN = geodeticCoordinate.RadiusOfPrimeVertical;
            double RM = geodeticCoordinate.RadiusOfMeridian;
            double H = geodeticCoordinate.Height;
            double B = geodeticCoordinate.Latitude.Radian;
            double sinB = geodeticCoordinate.Latitude.Sin;
            double omegae = geodeticCoordinate.EllipsoidType.AngularVelocity;
            Omegaenn = Vector<double>.Build.DenseOfArray(new double[]
            {
                Velocity[1] / (RN + H), -Velocity[0] / (RM + H), -Velocity[1] * Math.Tan(B) / (RN + H)
            });
            Omegaien = Vector<double>.Build.DenseOfArray(new double[]
            {
                omegae * Math.Cos(B), 0, -omegae * sinB
            });
            double g0 = 9.7803267715 * (1 + 0.0052790414 * sinB * sinB + 0.0000232718 * Math.Pow(sinB, 4));
            Gravity = Vector<double>.Build.DenseOfArray(new double[] {
                0, 0, g0 - (3.087691089e-6 - 4.397731e-9 * sinB * sinB) * H + 0.721e-12 * H * H
            });
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format)) format = "deg";

            return Time.ToString() + " " + GeodeticPosition.ToString(format, formatProvider) + " "
                + CartesianPosition.ToString("F4", formatProvider) + " " + Velocity.ToVectorString(12, 80, "..", "", " ", x => x.ToString("F4", formatProvider)) + " " +
                EulerAngleAtt.ToString(format, formatProvider);
        }

        public string ToString(string? format, IFormatProvider? formatProvider, char delimiter)
        {
            if (string.IsNullOrEmpty(format)) format = "deg";

            return Time.ToString(delimiter) + delimiter + GeodeticPosition.ToString(format, formatProvider, delimiter) + delimiter
                + CartesianPosition.ToString("F4", formatProvider, delimiter) + delimiter + Velocity[0].ToString("F4", formatProvider) + delimiter +
                Velocity[1].ToString("F4", formatProvider) + delimiter + Velocity[2].ToString("F4", formatProvider) + delimiter +
                EulerAngleAtt.ToString(format, formatProvider, delimiter);
        }

        public override string ToString()
            => string.Format("{0:deg}", this);
    }
}
