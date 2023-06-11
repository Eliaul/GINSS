using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Filter.Kalman
{

    public interface IKalmanFilter
    {
        /// <summary>
        /// The covariance of the current state estimate.
        /// </summary>
        Matrix<double> Cov { get; }

        /// <summary>
        /// The current best estimate of the state of the system.
        /// </summary>
        Matrix<double> State { get; }

        /// <summary>
        /// Perform a prediction of the next state of the system.
        /// </summary>
        /// <param name="F">The state transition matrix.</param>
        void Predict(Matrix<double> F);

        /// <summary>
        /// Perform a prediction of the next state of the system.
        /// </summary>
        /// <param name="F">The state transition matrix.</param>
        /// <param name="G">The linear equations to describe the effect of the noise on the system.</param>
        /// <param name="Q">The covariance of the noise acting on the system.</param>
        void Predict(Matrix<double> F, Matrix<double> G, Matrix<double> Q);

        /// <summary>
        /// Update the state estimate and covariance of the system based on the
        /// given measurement.
        /// </summary>
        /// <param name="z">The measurements of the system.</param>
        /// <param name="H">Linear equations to describe relationship between measurements and state variables.</param>
        /// <param name="R">The covariance matrix of the measurements.</param>
        void Update(Matrix<double> z, Matrix<double> H, Matrix<double> R);
    }

    internal static class KalmanFilter
    {
        public static void CheckInitialParameters(Matrix<double> x0, Matrix<double> P0)
        {
            if (x0.ColumnCount != 1)
                throw new MatrixSizeException(nameof(x0), x0.RowCount, x0.ColumnCount);
            if (P0.ColumnCount != P0.RowCount)
                throw new MatrixSizeException($"{nameof(P0)} must be square matrix!");
            if (P0.ColumnCount != x0.RowCount)
                throw new MatrixSizeException();
        }

        public static void CheckPredictParameters(Matrix<double> F, Matrix<double> G, Matrix<double> Q, IKalmanFilter filter)
        {
            if ((F.ColumnCount != F.RowCount) || (F.ColumnCount != filter.State.RowCount))
                throw new MatrixSizeException();
            if ((G.RowCount != filter.State.RowCount) || (G.ColumnCount != Q.RowCount))
                throw new MatrixSizeException();
            if (Q.ColumnCount != Q.RowCount)
                throw new MatrixSizeException($"{nameof(Q)} must be square matrix!");
        }

        public static void CheckPredictParameters(Matrix<double> F, Matrix<double> Q, IKalmanFilter filter)
        {
            if ((F.ColumnCount != F.RowCount) || (F.ColumnCount != filter.State.RowCount))
                throw new MatrixSizeException();
            if ((Q.ColumnCount != Q.RowCount) || (Q.ColumnCount != filter.State.RowCount))
                throw new MatrixSizeException();
        }

        public static void CheckPredictParameters(Matrix<double> F, IKalmanFilter filter)
        {
            if ((F.ColumnCount != F.RowCount) || (F.ColumnCount != filter.State.RowCount))
                throw new MatrixSizeException();
        }

        public static void CheckUpdateParameters(Matrix<double> z, Matrix<double> H, Matrix<double> R, IKalmanFilter filter)
        {
            if (z.ColumnCount != 1)
                throw new MatrixSizeException(nameof(z), z.RowCount, z.ColumnCount);
            if ((H.RowCount != z.RowCount) || (H.ColumnCount != filter.State.RowCount))
                throw new MatrixSizeException();
            if ((R.ColumnCount != R.RowCount) || (R.ColumnCount != z.RowCount))
                throw new MatrixSizeException();
        }
    }
}
