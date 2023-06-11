using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools
{
    public static class Extensions
    {
        public static double ToRad(this double num)
            => num * Math.PI / 180;

        public static double ToDeg(this double num)
            => num * 180 / Math.PI;

        public static Vector<double> CrossProduct(Vector<double> left, Vector<double> right)
        {
            if ((left.Count != 3 || right.Count != 3))
            {
                string message = "Vectors must have a length of 3.";
                throw new Exception(message);
            }
            var result = Vector<double>.Build.Dense(3);
            result[0] = left[1] * right[2] - left[2] * right[1];
            result[1] = -left[0] * right[2] + left[2] * right[0];
            result[2] = left[0] * right[1] - left[1] * right[0];
            return result;
        }

        public static Matrix<double> SkewSymmetricMatrix(Vector<double> vector)
        {
            if (vector.Count != 3)
                throw new ArgumentException("The vector's length must be 3.", nameof(vector));
            return Matrix<double>.Build.DenseOfArray(new double[,]
            {
                {0, -vector[2], vector[1] },
                {vector[2], 0, -vector[0] },
                {-vector[1], vector[0], 0 }
            });
        }


    }
}
