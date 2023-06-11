using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Geodesy
{
    public record struct CartesianCoordinate :
        IEquatable<CartesianCoordinate>, IFormattable
    {
        public double X;

        public double Y;

        public double Z;

        public readonly Vector<double> Vec => Vector<double>.Build.DenseOfArray(new double[] { X, Y, Z });

        public readonly static CartesianCoordinate Origin = new(0, 0, 0);

        public CartesianCoordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public CartesianCoordinate(double[] xyz)
        {
            if (xyz.Length != 3)
                throw new ArgumentException("The length of array must be 3.", nameof(xyz));
            X = xyz[0];
            Y = xyz[1];
            Z = xyz[2];
        }

        public CartesianCoordinate(Vector<double> xyz)
        {
            if (xyz.Count != 3)
                throw new ArgumentException("The length of array must be 3.", nameof(xyz));
            X = xyz[0];
            Y = xyz[1];
            Z = xyz[2];
        }

        public static CartesianCoordinate FromGeodeticCoordinate(GeodeticCoordinate geodeticCoordinate)
        {
            var N = geodeticCoordinate.RadiusOfPrimeVertical;
            var B = geodeticCoordinate.Latitude;
            var L = geodeticCoordinate.Longitude;
            var H = geodeticCoordinate.Height;
            var e = geodeticCoordinate.EllipsoidType.FirstEccentricity;
            return new(
                (N + H) * B.Cos * L.Cos,
                (N + H) * B.Cos * L.Sin,
                (N * (1 - e * e) + H) * B.Sin
            );
        }

        public static CartesianCoordinate FromLocalCoordinate(LocalCoordinate roverENU, CartesianCoordinate reference, Ellipsoid ellipsoid)
        {
            var (x0, y0, z0) = reference;
            var (b0, l0, _) = GeodeticCoordinate.FromCartesianCoordinate(reference, ellipsoid);
            var (e, n, u) = roverENU;
            double sinL = Math.Sin(l0);
            double cosL = Math.Cos(l0);
            double sinB = Math.Sin(b0);
            double cosB = Math.Cos(b0);
            return new
            (
                -e * sinL - n * sinB * cosL + u * cosB * cosL + x0,
                e * cosL - n * sinB * sinL + u * cosB * sinL + y0,
                n * cosB + u * sinB + z0
            );
        }

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public double this[int i]
        {
            get
            {
                return i switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new ArgumentOutOfRangeException(nameof(i), $"Index {i} is invalid."),
                };
            }
            set
            {
                switch (i)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(i), $"Index {i} is invalid.");
                }
            }
        }


        public static CartesianCoordinate operator +(CartesianCoordinate left, CartesianCoordinate right)
            => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        public static CartesianCoordinate operator -(CartesianCoordinate left, CartesianCoordinate right)
            => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        public static CartesianCoordinate operator -(CartesianCoordinate geo)
            => new(-geo.X, -geo.Y, -geo.Z);

        public static CartesianCoordinate operator *(CartesianCoordinate left, double right)
            => new(left.X * right, left.Y * right, left.Z * right);

        public static CartesianCoordinate operator *(double left, CartesianCoordinate right)
            => new(left * right.X, left * right.Y, left * right.Z);

        public static CartesianCoordinate operator /(CartesianCoordinate left, double right)
            => new(left.X / right, left.Y / right, left.Z / right);

        public string ToString(string? format, IFormatProvider? formatProvider)
            => X.ToString(format, formatProvider) + " " + Y.ToString(format, formatProvider) + " "
            + Z.ToString(format, formatProvider);

        public string ToString(string? format, IFormatProvider? formatProvider, char delimiter)
            => X.ToString(format, formatProvider) + delimiter + Y.ToString(format, formatProvider) + delimiter
            + Z.ToString(format, formatProvider);

        public override string ToString()
            => string.Format("{0:F10}", this);
    }
}
