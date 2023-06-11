using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Filter.Kalman
{
    /// <summary>This implementation uses the Rauch-Tung-Striebel smooth.
    /// <code>
    /// Backward recursive:
    ///     C(k) = P(k|k) * F'(k+1) * P^(-1)(k+1|k)
    ///     x(k|n) = x(k|k) + C(k)*(x(k+1|n) - x(k+1|k))
    ///     P(k|n) = P(k|k) + C(k)*(P(k+1|n) - P(k+1|k))\
    ///     
    /// x(k|n) and P(k|n) is respectively the <c>smoothed</c> state estimates and 
    /// the <c>smoothed</c> covariances.
    /// 
    /// x(k|k) is the a-posteriori state estimate of timestep k and x(k+1|k) is
    /// the a-priori state estimate of timestep k+1. The same notation applies
    /// to the covariance.
    /// </code>
    /// </summary>
    public class RtsSmoother
    {
        private Matrix<double> x;

        private Matrix<double> P;

        public Matrix<double> State => x;

        public Matrix<double> Cov => P;

        public RtsSmoother(Matrix<double> xn, Matrix<double> Pn)
        {
            x = xn;
            P = Pn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="F"></param>
        /// <param name="xPredict"></param>
        /// <param name="xUpdate"></param>
        /// <param name="PPredict"></param>
        /// <param name="PUpdate"></param>
        public void Update(Matrix<double> F, Matrix<double> xPredict, Matrix<double> xUpdate, Matrix<double> PPredict, Matrix<double> PUpdate)
        {
            var C = PUpdate * F.Transpose() * PPredict.Inverse();
            x = xUpdate + C * (x - xPredict);
            P = PUpdate + C * (P - PPredict);
        }
            
    }
}
