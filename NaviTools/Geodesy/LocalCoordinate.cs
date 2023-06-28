using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Geodesy
{
    public record struct LocalCoordinate : IEquatable<LocalCoordinate>, IFormattable
    {
        public double East;

        public double North;

        public double Up;

        public LocalCoordinate(double east, double north, double up)
        {
            East = east;
            North = north;
            Up = up;
        }

        public static LocalCoordinate FromNED(double north, double east, double down)
        {
            return new(east, north, -down);
        }

        public void Deconstruct(out double east, out double north, out double up)
        {
            east = East;
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
            => East.ToString(format, formatProvider) + " " +
                North.ToString(format, formatProvider) + " " +
                Up.ToString(format, formatProvider);

        public override string ToString()
            => string.Format("{0:F10}", this);

    }
}
