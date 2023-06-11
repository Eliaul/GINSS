using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NaviTools
{
    public readonly record struct GpsTime : 
        IComparable<GpsTime>, IComparable, IEquatable<GpsTime>,
        IFormattable, ISpanFormattable
    {
        public readonly static UtcTime OriginalTimePoint = new(1980, 1, 6, 0, 0, 0, TimeSpan.Zero);

        public readonly TimeSpan GpsTimeSpan;

        public readonly UtcTime UtcTimePoint
        {
            get
            {
                var utc = OriginalTimePoint + GpsTimeSpan;
                utc -= LeapSeconds.FindLeapSeconds(utc);
                return utc;
            }
        }

        public readonly UtcTime LocalTimePoint
        {
            get => UtcTimePoint.ToLocalTime();
        }

        public static GpsTime Now => new(DateTimeOffset.Now);

        public static string gpsTimeRegexString => "[0-9]{4}\\s[0-9]+\\.[0-9]+";

        public readonly int Weeks
        {
            get => (int)(GpsTimeSpan.TotalDays / 7.0);
        }

        public readonly double SecOfWeeks
        {
            get => GpsTimeSpan.TotalSeconds - Weeks * 7 * 24 * 3600;
        }

        public void Deconstruct(out int weeks, out double secOfWeeks)
        {
            weeks = Weeks;
            secOfWeeks = SecOfWeeks;
        }

        public GpsTime(TimeSpan timeSpan)
        {
            GpsTimeSpan = timeSpan;
        }

        public GpsTime(int weeks, int seconds)
        {
            GpsTimeSpan = new(weeks * 7, 0, 0, seconds);
        }

        public GpsTime(int weeks, int seconds, int milliseconds)
        {
            GpsTimeSpan = new(weeks * 7, 0, 0, seconds, milliseconds);
        }

        public GpsTime(int weeks, int seconds, int milliseconds, int microseconds)
        {
            GpsTimeSpan = new(weeks * 7, 0, 0, seconds, milliseconds, microseconds);
        }

        public GpsTime(int weeks, double seconds)
        {
            GpsTimeSpan = TimeSpan.FromSeconds(seconds) + new TimeSpan(weeks * 7, 0, 0, 0);
        }

        public GpsTime(UtcTime localTime)
        {
            UtcTime utc = localTime.UtcDateTime;
            utc += LeapSeconds.FindLeapSeconds(utc);
            GpsTimeSpan = utc - OriginalTimePoint;
        }

        public static GpsTime FromString(string gpsTime)
        {
            if (Check(gpsTime))
            {
                var times = gpsTime.Split(' ');
                return new(Convert.ToInt32(times[0]), Convert.ToDouble(times[1]));
            }
            throw new Exception();
        }

        public static bool Check(string gpsTime)
        {
            return Regex.IsMatch(gpsTime, gpsTimeRegexString);
        }

        public static TimeSpan operator -(GpsTime t1, GpsTime t2)
            => t1.GpsTimeSpan - t2.GpsTimeSpan;

        public int CompareTo(GpsTime other) => GpsTimeSpan.CompareTo(other.GpsTimeSpan);

        public int CompareTo(object? value) => GpsTimeSpan.CompareTo(value);

        public override int GetHashCode() => (GpsTimeSpan, OriginalTimePoint).GetHashCode();

        public override string ToString()
        {
            return string.Format("{0} {1:F4}", Weeks, SecOfWeeks);
        }

        public string ToString(char delimiter)
        {
            return $"{Weeks}{delimiter}{SecOfWeeks:F4}";
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return ((IFormattable)GpsTimeSpan).ToString(format, formatProvider);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            return ((ISpanFormattable)GpsTimeSpan).TryFormat(destination, out charsWritten, format, provider);
        }

        public static bool operator <(GpsTime left, GpsTime right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(GpsTime left, GpsTime right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(GpsTime left, GpsTime right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(GpsTime left, GpsTime right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static GpsTime operator +(GpsTime left, TimeSpan right)
        {
            return new(left.GpsTimeSpan + right);
        }

        public static GpsTime operator -(GpsTime left, TimeSpan right)
        {
            return new(left.GpsTimeSpan - right);
        }
    }
}
