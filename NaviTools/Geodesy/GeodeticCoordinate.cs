using MathNet.Numerics.LinearAlgebra.Complex;
using NaviTools.MathTools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Geodesy
{
    public record struct GeodeticCoordinate :
        IEquatable<GeodeticCoordinate>, IFormattable
    {

        public Angle Latitude;

        public Angle Longitude;

        public double Height;

        public Ellipsoid EllipsoidType;

        /// <summary>
        /// Radius of curvature in prime vertical(R_N).
        /// </summary>
        public double RadiusOfPrimeVertical
        {
            get
            {
                double a = EllipsoidType.SemiMajorAxis;
                double e = EllipsoidType.FirstEccentricity;
                double sinB = Latitude.Sin;
                return a / Math.Sqrt(1 - e * e * sinB * sinB);
            }
        }

        /// <summary>
        /// Radius of curvature in meridian(R_M).
        /// </summary>
        public double RadiusOfMeridian
        {
            get
            {
                double a = EllipsoidType.SemiMajorAxis;
                double e = EllipsoidType.FirstEccentricity;
                double sinB = Latitude.Sin;
                return a * (1 - e * e) / Math.Pow(1 - e * e * sinB * sinB, 1.5);
            }
        }

        public GeodeticCoordinate(Angle latitude, Angle longitude, double height, Ellipsoid ellipsoid)
        {
            if (latitude < -Angle.HalfPi || latitude > Angle.HalfPi)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be within the range of -90 to 90 degrees.");
            if (longitude < -Angle.Pi || longitude > Angle.Pi)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be within the range of -180 to 180 degrees.");
            Latitude = latitude;
            Longitude = longitude;
            Height = height;
            EllipsoidType = ellipsoid;
        }

        public GeodeticCoordinate(double latitude, double longtitude, double height, Ellipsoid ellipsoid, AngleUnit unit = AngleUnit.rad)
            :this(new(latitude, unit), new(longtitude, unit), height, ellipsoid)
        {

        }

        public void Deconstruct(out double latitude, out double longtitude, out double height)
        {
            latitude = Latitude.Radian;
            longtitude = Longitude.Radian;
            height = Height;
        }

        public static GeodeticCoordinate FromCartesianCoordinate(CartesianCoordinate cartesianCoordinate, Ellipsoid ellipsoid)
        {
            if (cartesianCoordinate != new CartesianCoordinate(0, 0, 0))
            {
                var (x, y, z) = cartesianCoordinate;
                double L = Math.Atan2(y, x);
                double R = Math.Sqrt(x * x + y * y + z * z);
                double phi = Math.Atan(z / Math.Sqrt(x * x + y * y));
                double e = ellipsoid.FirstEccentricity;
                double a = ellipsoid.SemiMajorAxis;
                var function = (double x) =>
                {
                    double W = Math.Sqrt(1 - e * e * Math.Sin(x) * Math.Sin(x));
                    double equation = x - Math.Atan(Math.Tan(phi) * (1 + a * e * e * Math.Sin(x) / (z * W)));
                    double dEquation = 1 - (a * e * e * z * Math.Cos(x) * Math.Tan(phi)) / (W * (z * z / Math.Pow(Math.Cos(phi), 2)
                        + e * e * Math.Sin(x) * (2 * a * z * W * Math.Pow(Math.Tan(phi), 2) - Math.Sin(x) * (z * z + (-e * e * a * a + z * z) * Math.Pow(Math.Tan(phi), 2)))));
                    return equation / dEquation;
                };
                double B = NewtonIteration.Solve(phi, 1e-12, 12, function);
                double W = Math.Sqrt(1 - e * e * Math.Sin(B) * Math.Sin(B));
                double H = R * Math.Cos(phi) / Math.Cos(B) - a / W;
                return new(B, L, H, ellipsoid);
            }
            return new GeodeticCoordinate(0, 0, -ellipsoid.SemiMajorAxis, ellipsoid);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format)) format = "deg";
            
            return format.ToUpperInvariant() switch
            {
                "DEG" => Longitude.ToString("DEG", formatProvider) + " " +
                                        Latitude.ToString("DEG", formatProvider) + " " +
                                        Height.ToString("F4", formatProvider),
                "RAD" => Longitude.ToString("RAD", formatProvider) + " " +
                                        Latitude.ToString("RAD", formatProvider) + " " +
                                        Height.ToString("F4", formatProvider),
                "DMS" => Longitude.ToString("DMS", formatProvider) + " " +
                                        Latitude.ToString("DMS", formatProvider) + " " +
                                        Height.ToString("F4", formatProvider),
                "DEGVAL" => Longitude.ToString("DEGVAL", formatProvider) + " " +
                                        Latitude.ToString("DEGVAL", formatProvider) + " " +
                                        Height.ToString("F4", formatProvider),
                "RADVAL" => Longitude.ToString("RADVAL", formatProvider) + " " +
                                        Latitude.ToString("RADVAL", formatProvider) + " " +
                                        Height.ToString("F4", formatProvider),
                _ => throw new FormatException(string.Format("The {0} format string is not supported.", format)),
            };
        }

        public string ToString(string? format, IFormatProvider? formatProvider, char delimiter)
        {
            if (string.IsNullOrEmpty(format)) format = "deg";

            return format.ToUpperInvariant() switch
            {
                "DEG" => Longitude.ToString("DEG", formatProvider) + delimiter +
                                        Latitude.ToString("DEG", formatProvider) + delimiter +
                                        Height.ToString("F4", formatProvider),
                "RAD" => Longitude.ToString("RAD", formatProvider) + delimiter +
                                        Latitude.ToString("RAD", formatProvider) + delimiter +
                                        Height.ToString("F4", formatProvider),
                "DMS" => Longitude.ToString("DMS", formatProvider) + delimiter +
                                        Latitude.ToString("DMS", formatProvider) + delimiter +
                                        Height.ToString("F4", formatProvider),
                "DEGVAL" => Longitude.ToString("DEGVAL", formatProvider) + delimiter +
                                        Latitude.ToString("DEGVAL", formatProvider) + delimiter +
                                        Height.ToString("F4", formatProvider),
                "RADVAL" => Longitude.ToString("RADVAL", formatProvider) + delimiter +
                                        Latitude.ToString("RADVAL", formatProvider) + delimiter +
                                        Height.ToString("F4", formatProvider),
                _ => throw new FormatException(string.Format("The {0} format string is not supported.", format)),
            };
        }

        public override string ToString()
            => string.Format("{0:deg}", this);

    }
}
