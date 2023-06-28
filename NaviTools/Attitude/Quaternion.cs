using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Attitude
{
    public record struct Quaternion :
        System.Numerics.IAdditionOperators<Quaternion, Quaternion, Quaternion>,
        System.Numerics.IMultiplyOperators<Quaternion, Quaternion, Quaternion>,
        System.Numerics.IDivisionOperators<Quaternion, double, Quaternion>,
        System.Numerics.ISubtractionOperators<Quaternion, Quaternion, Quaternion>,
        IEquatable<Quaternion>, IFormattable
    {
        public double W { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public Vector<double> Imaginary
        {
            get => Vector<double>.Build.DenseOfArray(new double[] { X, Y, Z});
        }

        public Vector<double> Vector => Vector<double>.Build.DenseOfArray(new double[] {W, X, Y, Z });

        public readonly static Quaternion One = new(1, 0, 0, 0);

        public readonly static Quaternion Zero = new(0, 0, 0, 0);

        public Quaternion(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public Quaternion(Vector<double> vector)
        {
            if (vector.Count != 4)
                throw new ArgumentOutOfRangeException(nameof(vector), "The length of vector must be 4.");
            W = vector[0];
            X = vector[1];
            Y = vector[2];
            Z = vector[3];
        }

        public static Quaternion FromAxisAndAngle(Vector<double> axis, double angle, AngleUnit unit = AngleUnit.rad)
        {
            if (axis.Count != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(axis));
            }
            double ang = new Angle(angle, unit).Radian;
            double halfAng = ang / 2;
            double sinAng = Math.Sin(halfAng);
            double cosAng = Math.Cos(halfAng);
            Vector<double> unitAxis = axis.Normalize(2);
            return new(cosAng, unitAxis[0] * sinAng, unitAxis[1] * sinAng, unitAxis[2] * sinAng);
        }

        public static Quaternion FromRotationVector(Vector<double> rotationVector)
            => FromAxisAndAngle(rotationVector, rotationVector.Norm(2));

        public static Quaternion FromRotationMatrix(Matrix<double> rotationMatrix)
        {
            if (rotationMatrix.RowCount != 3 || rotationMatrix.ColumnCount != 3)
            {
                throw new ArgumentException("The matrix's size must be 3×3", nameof(rotationMatrix));
            }
            return FromEulerAngle(EulerAngle.FromRotationMatrix(rotationMatrix));
        }

        public static Quaternion FromEulerAngle(EulerAngle eulerAngle)
        {
            double cosHalfRoll = (eulerAngle.Roll / 2.0).Cos;
            double sinHalfRoll = (eulerAngle.Roll / 2.0).Sin;
            double cosHalfPitch = (eulerAngle.Pitch / 2.0).Cos;
            double sinHalfPitch = (eulerAngle.Pitch / 2.0).Sin;
            double cosHalfYaw = (eulerAngle.Yaw / 2.0).Cos;
            double sinHalfYaw = (eulerAngle.Yaw / 2.0).Sin;
            return new Quaternion(
                cosHalfRoll * cosHalfPitch * cosHalfYaw + sinHalfRoll * sinHalfPitch * sinHalfYaw,
                sinHalfRoll * cosHalfPitch * cosHalfYaw - cosHalfRoll * sinHalfPitch * sinHalfYaw,
                cosHalfRoll * sinHalfPitch * cosHalfYaw + sinHalfRoll * cosHalfPitch * sinHalfYaw,
                cosHalfRoll * cosHalfPitch * sinHalfYaw - sinHalfRoll * sinHalfPitch * cosHalfYaw
                );
        }

        public double this[int i]
        {
            get
            {
                return i switch
                {
                    0 => W,
                    1 => X,
                    2 => Y,
                    3 => Z,
                    _ => throw new ArgumentOutOfRangeException(nameof(i), $"Index {i} is invalid.")
                };

            }
            set
            {
                switch (i)
                {
                    case 0:
                        W = value;
                        break;
                    case 1:
                        X = value;
                        break;
                    case 2:
                        Y = value;
                        break;
                    case 3:
                        Z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(i), $"Index {i} is invalid.");
                }
            }
        }

        public Matrix<double> ToRotationMatrix()
        {
            double q0 = W;
            double q1 = X;
            double q2 = Y;
            double q3 = Z;
            return Matrix<double>.Build.DenseOfArray(new double[,]
            {
                {q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3, 2 * (q1 * q2 - q0 * q3), 2 * (q1 * q3 + q0 * q2) },
                {2 * (q1 * q2 + q0 * q3), q0 * q0 - q1 * q1 + q2 * q2 - q3 * q3, 2 * (q2 * q3 - q0 * q1) },
                {2 * (q1 * q3 - q0 * q2), 2 * (q2 * q3 + q0 * q1), q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3 }
            });
        }

        public override string ToString()
        => ToString("F10", null, ',');

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return W.ToString(format, formatProvider) + " " + X.ToString(format, formatProvider) + " " +
                 Y.ToString(format, formatProvider) + " " + Z.ToString(format, formatProvider);
        }

        public string ToString(string? format, IFormatProvider? formatProvider, char delimiter)
        {
            return W.ToString(format, formatProvider) + delimiter + X.ToString(format, formatProvider) + delimiter +
                Y.ToString(format, formatProvider) + delimiter + Z.ToString(format, formatProvider);
        }

        public double Norm() => Math.Sqrt(W * W + X * X + Y * Y + Z * Z);

        public Quaternion Normalize() => this / Norm();

        public Quaternion Conjugate() => new(W, -X, -Y, -Z);

        public Quaternion Inverse() => Conjugate() / (Norm() * Norm());


        #region "operator overload"

        public static Quaternion operator +(Quaternion left, Quaternion right)
            => new(left.W + right.W, left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        public static Quaternion operator -(Quaternion left, Quaternion right)
            => new(left.W - right.W, left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        public static Quaternion operator -(Quaternion quaternion)
            => new(-quaternion.W, -quaternion.X, -quaternion.Y, -quaternion.Z);

        public static Quaternion operator *(Quaternion left, double right)
            => new(left.W * right, left.X * right, left.Y * right, left.Z * right);

        public static Quaternion operator *(double left, Quaternion right)
            => right * left;

        public static Quaternion operator /(Quaternion left, double right)
            => left * (1.0 / right);

        public static Quaternion operator *(Quaternion p, Quaternion q)
        {
            return new(
                p[0] * q[0] - p[1] * q[1] - p[2] * q[2] - p[3] * q[3],
                p[0] * q[1] + p[1] * q[0] + p[2] * q[3] - p[3] * q[2],
                p[0] * q[2] + p[2] * q[0] + p[3] * q[1] - p[1] * q[3],
                p[0] * q[3] + p[3] * q[0] + p[1] * q[2] - p[2] * q[1]
            );
        }

        #endregion




    }
}
