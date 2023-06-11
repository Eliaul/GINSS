using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools
{
    public enum AngleUnit : byte
    {
        rad,
        deg
    }

    public readonly record struct Angle : 
        IFormattable, IEquatable<Angle>, IComparable<Angle>,
        IComparable, IAdditionOperators<Angle, Angle, Angle>,
        ISubtractionOperators<Angle, Angle, Angle>
    {
        public readonly double Radian { get; init; }

        public readonly double Degree { get => Radian.ToDeg(); }

        public readonly double Sin => Math.Sin(Radian);

        public readonly double Cos => Math.Cos(Radian);  

        public static readonly Angle Zero = new(0);

        public static readonly Angle HalfPi = new(Math.PI / 2);

        public static readonly Angle Pi = new(Math.PI);

        public static readonly Angle TwoPi = new(2 * Math.PI);

        public readonly (int, int, double) DMS
        {
            get
            {
                int deg = (int)Degree;
                int min = (int)((Degree - deg) * 60);
                double sec = Degree * 3600 - deg * 3600 - min * 60;
                return (deg, min, sec);
            }
        }

        public Angle(double angle, AngleUnit unit = AngleUnit.rad)
        {
            if (unit == AngleUnit.rad)
                Radian = angle;
            else if (unit == AngleUnit.deg)
                Radian = angle.ToRad();
        }

        public Angle(int degree, int minute, double second)
        {
            Radian = (degree + minute / 60.0 + second / 3600.0).ToRad();
        }

        public Angle MapToRange(double startAngleOfRange, AngleUnit unit)
        {
            var startAng = new Angle(startAngleOfRange, unit).Radian;
            var ang = Radian;

            while (ang < startAng)
                ang += 2 * Math.PI;
            while (ang >= startAng + 2 * Math.PI)
                ang -= 2 * Math.PI;

            return new Angle(ang);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format)) format = "deg";

            switch(format.ToUpperInvariant())
            {
                case "DEG":
                    return Degree.ToString("F8", formatProvider) + "°";
                case "RAD":
                    return Radian.ToString("F10", formatProvider) + "rad";
                case "DMS":
                    (int deg, int min, double sec) = DMS;
                    return deg.ToString("F0", formatProvider) + "°" +
                        min.ToString("F0", formatProvider) + "\"" +
                        sec.ToString("F5", formatProvider) + "'";
                case "DEGVAL":
                    return Degree.ToString("F8", formatProvider);
                case "RADVAL":
                    return Radian.ToString("F10", formatProvider);
                default:
                    if (format[0] == 'D')
                    {
                        return Degree.ToString(format[1..], formatProvider);
                    }
                    if (format[0] == 'R')
                    {
                        return Radian.ToString(format[1..], formatProvider);
                    }
                    throw new FormatException($"The {format} format string is not supported.");
            }
        }

        public int CompareTo(Angle other) => Radian.CompareTo(other.Radian);

        public int CompareTo(object? obj) => Radian.CompareTo(obj);

        public static Angle operator +(Angle left, Angle right)
        {
            return new(left.Radian + right.Radian);
        }

        public static Angle operator -(Angle left, Angle right)
        {
            return new(left.Radian - right.Radian);
        }

        public static Angle operator -(Angle ang)
            => new(-ang.Radian);

        public static bool operator <(Angle left, Angle right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Angle left, Angle right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Angle left, Angle right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Angle left, Angle right)
            => left.CompareTo(right) >= 0;

        public static Angle operator *(double left, Angle right)
            => new(left * right.Radian);

        public static Angle operator *(Angle left, double right)
            => new(left.Radian * right);

        public static Angle operator /(Angle left, double right)
            => new(left.Radian / right);



    }
}
