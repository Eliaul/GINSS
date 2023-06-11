using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Geodesy
{
    public record struct LocalCoordinate : IEquatable<LocalCoordinate>, IFormattable
    {
        public double Earth;

        public double North;

        public double Up;

        public LocalCoordinate(double earth, double north, double up)
        {
            Earth = earth;
            North = north;
            Up = up;
        }

        public static LocalCoordinate FromNED(double north, double earth, double down)
        {
            return new(earth, north, -down);
        }

        public void Deconstruct(out double earth, out double north, out double up)
        {
            earth = Earth;
            north = North;
            up = Up;
        }

        public static LocalCoordinate FromCartesianCoordinate(CartesianCoordinate roverPos, CartesianCoordinate referencePos, Ellipsoid ellipsoid)
        {
            var (x, y, z) = roverPos;
            var (x0, y0, z0) = referencePos;
            var (b, l, _) = GeodeticCoordinate.FromCartesianCoordinate(referencePos, ellipsoid);
            double sinB = Math.Sin(b);
            double cosB = Math.Cos(b);
            double sinL = Math.Sin(l);
            double cosL = Math.Cos(l);
            return new
            (
                -(x - x0) * sinL + (y - y0) * cosL,
                -(x - x0) * sinB * cosL - (y - y0) * sinB * sinL + (z - z0) * cosB,
                (x - x0) * cosB * cosL + (y - y0) * cosB * sinL + (z - z0) * sinB
            );
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
            => Earth.ToString(format, formatProvider) + " " +
                North.ToString(format, formatProvider) + " " +
                Up.ToString(format, formatProvider);

        public override string ToString()
            => string.Format("{0:F10}", this);

    }
}
