using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Filter.Kalman
{
    /// <summary>This implementation uses the most common form of the discrete time
    /// Kalman Filter:
    /// <code>
    /// Prediction: x(k|k-1) = F(k-1) * x(k-1|k-1)
    ///             P(k|k-1) = F(k-1)*P(k-1|k-1)*F(k-1) + G(k-1)*Q(k-1)*G'(k-1)
    /// Update:     S(k) = H(k)*P(k|k-1)*H'(k) + R(k)
    ///             K(k) = P(k|k-1)*H'(k)*S^(-1)(k)
    ///             P(k|k) = (I-K(k)*H(k))*P(k|k-1)
    ///             x(k|k) = x(k|k-1) + K(k)*(z(k)-H(k)*x(k|k-1))
    /// </code>
    /// </summary>
    public class DiscreteKalmanFilter : IKalmanFilter
    {
        /// <summary>
        /// The current covariance of the estimated state of the system.
        /// <code>P(k|k-1) or P(k|k)</code>
        /// </summary>
        protected Matrix<double> P;

        /// <summary>
        /// The current state of the system.
        /// <code>x(k|k-1) or x(k|k)</code>
        /// </summary>
        protected Matrix<double> x;

        public Matrix<double> Cov => P;

        /// <value>The current state of the system.</value>
        public Matrix<double> State
        {
            get => x;
            set
            {
                x = value;
            }
        }

        /// <summary>
        /// Creates a new Discrete Time Kalman Filter with the given values for
        /// the initial state and the covariance of that state.
        /// </summary>
        /// <param name="x0">The best estimate of the initial state of the estimate.</param>
        /// <param name="P0">The covariance of the initial state estimate.</param>
        public DiscreteKalmanFilter(Matrix<double> x0, Matrix<double> P0)
        {
            KalmanFilter.CheckInitialParameters(x0, P0);
            P = P0;
            x = x0;
        }

        /// <summary>
        /// Perform a discrete time prediction of the system state.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        public void Predict(Matrix<double> F)
        {
            KalmanFilter.CheckPredictParameters(F, this);
            x = F * x;
            P = F * P * F.Transpose();
        }

        /// <summary>
        /// Perform a discrete time prediction of the system state.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <param name="Q">Plant noise covariance.</param>
        public void Predict(Matrix<double> F, Matrix<double> Q) 
        {
            KalmanFilter.CheckPredictParameters(F, Q, this);
            x = F * x;
            P = F * P * F.Transpose() + Q;
        }

        /// <summary>
        /// Perform a discrete time prediction of the system state.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <param name="G">Noise coupling matrix.</param>
        /// <param name="Q">Plant noise covariance.</param>
        public void Predict(Matrix<double> F, Matrix<double> G, Matrix<double> Q)
        {
            KalmanFilter.CheckPredictParameters(F, G, Q, this);
            x = F * x;
            P = F * P * F.Transpose() + G * Q * G.Transpose();
        }

        /// <summary>
        /// Updates the state of the system based on the given noisy measurements,
        /// a description of how those measurements relate to the system, and a
        /// covariance Matrix to describe the noise of the system.
        /// </summary>
        /// <param name="z">The measurements of the system.</param>
        /// <param name="H">Measurement model.</param>
        /// <param name="R">Covariance of measurements.</param>
        public void Update(Matrix<double> z, Matrix<double> H, Matrix<double> R)
        {
            KalmanFilter.CheckUpdateParameters(z, H, R, this);
            var Ht = H.Transpose();
            var I = Matrix<double>.Build.DenseIdentity(x.RowCount, x.RowCount);
            var S = H * P * Ht + R;
            var K = P * Ht * S.Inverse();
            P = (I - K * H) * P;
            x += K * (z - H * x);
        }
    }
}
