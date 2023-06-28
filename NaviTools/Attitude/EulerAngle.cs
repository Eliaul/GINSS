using MathNet.Numerics.LinearAlgebra;

namespace NaviTools.Attitude
{
    public record struct EulerAngle : IFormattable, IEquatable<EulerAngle>
    {
        public Angle Roll { get; set; }

        public Angle Pitch { get; set; }

        public Angle Yaw { get; set; }

        public readonly static EulerAngle Zero = new(0, 0, 0);

        public EulerAngle(double roll, double pitch, double yaw, AngleUnit unit = AngleUnit.rad)
            :this(new(roll, unit), new(pitch, unit), new(yaw, unit))
        {

        }

        public EulerAngle(Angle roll, Angle pitch, Angle yaw)
        {
            Angle rollAng = roll.MapToRange(-180, AngleUnit.deg);
            Angle pitchAng = pitch.MapToRange(-180, AngleUnit.deg);
            Angle yawAng = yaw.MapToRange(-180, AngleUnit.deg);
            if (pitchAng.Radian < -Math.PI / 2) pitchAng += new Angle(Math.PI);
            else if (pitchAng.Radian > Math.PI / 2) pitchAng -= new Angle(Math.PI);
            Roll = rollAng;
            Pitch = pitchAng;
            Yaw = yawAng;
        }

        public static EulerAngle FromRotationMatrix(Matrix<double> rotationMatrix)
        {
            if (rotationMatrix.RowCount != 3 || rotationMatrix.ColumnCount != 3)
                throw new ArgumentOutOfRangeException(nameof(rotationMatrix), "Rotation matrix's size must be 3×3.");
            double pitch = -Math.Asin(rotationMatrix[2, 0]);
            if (!rotationMatrix[2, 0].Equals(1))
            {
                double yaw = Math.Atan2(rotationMatrix[1, 0], rotationMatrix[0, 0]);
                double roll = Math.Atan2(rotationMatrix[2, 1], rotationMatrix[2, 2]);
                return new(roll, pitch, yaw);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Pitch), "The pitch must not equal 90°.");
            }
        }

        public static EulerAngle FromQuaternion(Quaternion quaternion)
        {
            double q0 = quaternion.W;
            double q1 = quaternion.X;
            double q2 = quaternion.Y;
            double q3 = quaternion.Z;
            double pitch = -Math.Asin(2 * (q1 * q3 - q0 * q2));
            if (!pitch.Equals(Math.PI / 2))
            {
                double yaw = Math.Atan2(2 * (q1 * q2 + q0 * q3), q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3);
                double roll = Math.Atan2(2 * (q2 * q3 + q0 * q1), q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3);
                return new(roll, pitch, yaw);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Pitch), "The pitch must not equal 90°.");
            }
        }

        public Matrix<double> ToRotationMatrix()
        {
            double cosRoll = Roll.Cos;
            double sinRoll = Roll.Sin;
            double cosPitch = Pitch.Cos;
            double sinPitch = Pitch.Sin;
            double cosYaw = Yaw.Cos;
            double sinYaw = Yaw.Sin;

            return Matrix<double>.Build.DenseOfArray(
                new double[,] {
                    { cosPitch * cosYaw, -cosRoll * sinYaw + sinRoll * sinPitch * cosYaw, sinRoll * sinYaw + cosRoll * sinPitch * cosYaw},
                    {cosPitch * sinYaw, cosRoll * cosYaw + sinRoll * sinPitch * sinYaw, -sinRoll * cosYaw + cosRoll * sinPitch * sinYaw },
                    {-sinPitch, sinRoll * cosPitch, cosRoll * cosPitch }
                }
            );
        }

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            Roll.ToString(format, formatProvider) + " "
                + Pitch.ToString(format, formatProvider) + " "
                + Yaw.ToString(format, formatProvider);

        public string ToString(string? format, IFormatProvider? formatProvider, char delimiter)
            => Roll.ToString(format, formatProvider) + delimiter
                + Pitch.ToString(format, formatProvider) + delimiter
                + Yaw.ToString(format, formatProvider);

        public override string ToString()
            => ToString("degval", null, ',');

        public override int GetHashCode()
        {
            return HashCode.Combine(Roll, Pitch, Yaw);
        }

    }
}
