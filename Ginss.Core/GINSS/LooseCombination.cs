using Ginss.Core.INS;
using MathNet.Numerics;
using NaviTools;
using NaviTools.Attitude;
using NaviTools.Filter.Kalman;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.GINSS
{
    using VecD = MathNet.Numerics.LinearAlgebra.Vector<double>;
    using MatD = MathNet.Numerics.LinearAlgebra.Matrix<double>;

    enum UpdateTimeStatus
    {
        /// <summary>
        /// Indicates that there is no GNSS between the two IMU moments befor and after.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that the GNSS moment is closer to the IMU output of the previous moment.
        /// </summary>
        NearPre,
        /// <summary>
        /// Indicates that the GNSS moment is closer to the IMU output of the current moment.
        /// </summary>
        NearCur,
        /// <summary>
        /// Indicates that the GNSS moment is not close to any IMU output.
        /// </summary>
        Middle
    }

    public partial class LooseCombination
    {
        public InsMechanization InsMech { get; set; }

        public DiscreteKalmanFilter KalmanFilter { get; set; }

        public ImuError ImuErrorVec
        {
            get; set;
        }

        public StateOfEpoch? StateCur { get; set; }

        /// <summary>
        /// The power spectral density matrix of the system.
        /// </summary>
        public MatD Psd { get; set; }

        public ZuptWindow? Zupt { get; set; }

        public readonly VecD Tgb;

        public readonly VecD Tgs;

        public readonly VecD Tab;

        public readonly VecD Tas;

        #region "state vector"
        public VecD PositionError => VecD.Build.Dense(3, i => KalmanFilter.State[i, 0]);

        public VecD VelocityError => VecD.Build.Dense(3, i => KalmanFilter.State[i + 3, 0]);

        public VecD AttitudeError => VecD.Build.Dense(3, i => KalmanFilter.State[i + 6, 0]);

        public VecD GyroBiasError => VecD.Build.Dense(3, i => KalmanFilter.State[i + 9, 0]);

        public VecD GyroScaleError => VecD.Build.Dense(3, i => KalmanFilter.State[i + 12, 0]);

        public VecD AcceBiasError => VecD.Build.Dense(3, i => KalmanFilter.State[i + 15, 0]);

        public VecD AcceScaleError => VecD.Build.Dense(3, i => KalmanFilter.State[i + 18, 0]);
        #endregion

        /// <summary>
        /// Constructs power spectral density matrix based on IMU error model.
        /// </summary>
        /// <param name="errorModel"></param>
        /// <returns></returns>
        private static MatD ConstructPsd(ImuErrorModel errorModel)
        {
            var acceError = errorModel.AccelerometerError;
            var gyroError = errorModel.GyroscopeError;
            var res = MatD.Build.Dense(18, 18, 0);
            for (int i = 0; i < 3; i++)
            {
                res[i, i] = acceError.VRW[i] * acceError.VRW[i];
                res[i + 3, i + 3] = gyroError.ARW[i] * gyroError.ARW[i];
                res[i + 6, i + 6] = 2 * gyroError.BiasProcessNoise[i] * gyroError.BiasProcessNoise[i] / gyroError.BiasRelevantTime[i];
                res[i + 9, i + 9] = 2 * gyroError.ScaleFactorProcessNoise[i] * gyroError.ScaleFactorProcessNoise[i] / gyroError.ScaleFactorRelevantTime[i];
                res[i + 12, i + 12] = 2 * acceError.BiasProcessNoise[i] * acceError.BiasProcessNoise[i] / acceError.BiasRelevantTime[i];
                res[i + 15, i + 15] = 2 * acceError.ScaleFactorProcessNoise[i] * acceError.ScaleFactorProcessNoise[i] / acceError.ScaleFactorRelevantTime[i];
            }
            return res;
        }

        public LooseCombination(InsMechanization insMech, ImuError initialImuError, ImuErrorModel errorModel, DiscreteKalmanFilter kalmanFilter, int? windowLength = null)
        {
            InsMech = insMech;
            KalmanFilter = kalmanFilter;
            ImuErrorVec = initialImuError;
            Tgb = errorModel.GyroscopeError.BiasRelevantTime;
            Tgs = errorModel.GyroscopeError.ScaleFactorRelevantTime;
            Tab = errorModel.AccelerometerError.BiasRelevantTime;
            Tas = errorModel.AccelerometerError.ScaleFactorRelevantTime;
            Psd = ConstructPsd(errorModel);
            Zupt = windowLength == null ? null : new((int)windowLength);
        }

        public LooseCombination(InsMechanization insMech, ImuError initialImuError, ImuErrorModel errorModel, MatD initialStateVec, MatD initialCov, int? windowLength = null)
        {
            InsMech = insMech;
            ImuErrorVec = initialImuError;
            Tgb = errorModel.GyroscopeError.BiasRelevantTime;
            Tgs = errorModel.GyroscopeError.ScaleFactorRelevantTime;
            Tab = errorModel.AccelerometerError.BiasRelevantTime;
            Tas = errorModel.AccelerometerError.ScaleFactorRelevantTime;
            Psd = ConstructPsd(errorModel);
            KalmanFilter = new(initialStateVec, initialCov);
            Zupt = windowLength == null ? null : new((int)windowLength);
        }

        public LooseCombination(InsMechanization insMech, ImuError initialImuError, ImuErrorModel errorModel, MatD initialStateVec, VecD initialPosStd, VecD initialVelStd, VecD initialAttStd, int? windowLength = null)
        {
            InsMech = insMech;
            ImuErrorVec = initialImuError;
            Tgb = errorModel.GyroscopeError.BiasRelevantTime;
            Tgs = errorModel.GyroscopeError.ScaleFactorRelevantTime;
            Tab = errorModel.AccelerometerError.BiasRelevantTime;
            Tas = errorModel.AccelerometerError.ScaleFactorRelevantTime;
            Psd = ConstructPsd(errorModel);
            var initialCov = MatD.Build.Dense(21, 21, 0);
            for (int i = 0; i < 3; i++)
            {
                initialCov[i, i] = initialPosStd[i] * initialPosStd[i];
                initialCov[i + 3, i + 3] = initialVelStd[i] * initialVelStd[i];
                initialCov[i + 6, i + 6] = initialAttStd[i] * initialAttStd[i];
                initialCov[i + 9, i + 9] = errorModel.GyroscopeError.BiasProcessNoise[i] * errorModel.GyroscopeError.BiasProcessNoise[i];
                initialCov[i + 12, i + 12] = errorModel.AccelerometerError.BiasProcessNoise[i] * errorModel.AccelerometerError.BiasProcessNoise[i];
                initialCov[i + 15, i + 15] = errorModel.GyroscopeError.ScaleFactorProcessNoise[i] * errorModel.GyroscopeError.ScaleFactorProcessNoise[i];
                initialCov[i + 18, i + 18] = errorModel.AccelerometerError.ScaleFactorProcessNoise[i] * errorModel.AccelerometerError.ScaleFactorProcessNoise[i];
            }
            KalmanFilter = new(initialStateVec, initialCov);
            Zupt = windowLength == null ? null : new((int)windowLength);
        }

        private static UpdateTimeStatus IsUpdatedEpoch(GpsTime preTime, GpsTime curTime, GpsTime gnssTime)
        {
            double tolerance = 1e-5;
            if ((gnssTime - preTime).TotalSeconds > tolerance && (curTime - gnssTime).TotalSeconds > tolerance)
            {
                return UpdateTimeStatus.Middle;
            }
            else if ((gnssTime - preTime).TotalSeconds < tolerance)
            {
                return UpdateTimeStatus.NearPre;
            }
            else if ((curTime - gnssTime).TotalSeconds > tolerance)
            {
                return UpdateTimeStatus.NearCur;
            }
            else
            {
                return UpdateTimeStatus.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imuCur"></param>
        /// <param name="gnssIterator"></param>
        /// <param name="antennaLever"></param>
        public void ForwardProcess(ImuData imuCur, IEnumerator<GnssData> gnssIterator, VecD antennaLever)
        {
            ImuData imuPre = InsMech.ImuPre;
            GnssData gnssData = gnssIterator.Current;

            Zupt?.Add(Math.Abs(imuCur.DeltaVelocity.Norm(2) - 9.8 * ImuData.SamplingRate));

            // GNSS update or INS Mechanization.
            var status = IsUpdatedEpoch(imuPre.Time, imuCur.Time, gnssData.Time);

            if (status == UpdateTimeStatus.Middle)
            {
                ImuData imuMid = ImuData.VirtualOutput(imuPre, imuCur, gnssData.Time);

                StateCur = StatePredict(imuMid, (imuMid.Time - imuPre.Time).TotalSeconds);

                GnssUpdate(gnssData, imuMid, antennaLever);

                StateCur = StatePredict(imuCur, (imuCur.Time - imuMid.Time).TotalSeconds);

                gnssIterator.MoveNext();
            }
            else if (status == UpdateTimeStatus.NearPre)
            {
                GnssUpdate(gnssData, imuPre, antennaLever);

                StateCur = StatePredict(imuCur, ImuData.SamplingRate);

                gnssIterator.MoveNext();
            }
            else if (status == UpdateTimeStatus.NearCur)
            {
                StateCur = StatePredict(imuCur, ImuData.SamplingRate);

                GnssUpdate(gnssData, imuCur, antennaLever);

                gnssIterator.MoveNext();
            }
            else {
                StateCur = StatePredict(imuCur, ImuData.SamplingRate);

                ZuptUpdate();
            }


        }

        public void BackwardProcess()
        {

        }

        /// <summary>
        /// Predict the state of the next moment.
        /// </summary>
        /// <param name="imuCur">The output at the current moment.</param>
        /// <param name="deltaTime">The time difference.</param>
        /// <param name="tgb">Relevant time of the first-order Gaussmarkov process on gyroscope bias.</param>
        /// <param name="tgs">Relevant time of the first-order Gaussmarkov process on gyroscope scale factor.</param>
        /// <param name="tab">Relevant time of the first-order Gaussmarkov process on accelerometer bias.</param>
        /// <param name="tas">Relevant time of the first-order Gaussmarkov process on accelerometer scale factor.</param>
        /// <returns></returns>
        public StateOfEpoch StatePredict(ImuData imuCur, double deltaTime)
        {
            // Compensate imu error.
            var (gyroBias, gyroScale, acceBias, acceScale) = ImuErrorVec;
            imuCur.Compensate(gyroBias, gyroScale, acceBias, acceScale);

            // INS Mechanization.
            var stateCur = InsMech.Process(imuCur);

            var transitionMat = TransitionMatrix(imuCur.SpecificForce, imuCur.AngularVelocity, InsMech.StatePre, Tgb, Tgs, Tab, Tas) * deltaTime + MatD.Build.DenseIdentity(21);
            var G_k = CofficientMatrixG(stateCur);
            var G_ksub1 = CofficientMatrixG(InsMech.StatePre);
            var Qk = (transitionMat * G_ksub1 * Psd * G_ksub1.Transpose() * transitionMat.Transpose() + G_k * Psd * G_k.Transpose()) * (deltaTime / 2);
            
            // State prediction.
            KalmanFilter.Predict(transitionMat, Qk);
            return stateCur;
        }

        /// <summary>
        /// Use GNSS data to update the state of the system.
        /// </summary>
        /// <param name="gnssData">The GNSS data.</param>
        /// <param name="imuMid">IMU data at GNSS's time.</param>
        /// <param name="antennaLever">The antenna lever vector.</param>
        public void GnssUpdate(GnssData gnssData, ImuData imuMid, VecD antennaLever)
        {
            if (StateCur == null)
                return;
            var Cbn = StateCur.Attitude.ToRotationMatrix();
            var DRInv = MatD.Build.DenseOfDiagonalArray(new double[]
            {
                1 / (StateCur.GeodeticPosition.RadiusOfMeridian + StateCur.GeodeticPosition.Height),
                1 / ((StateCur.GeodeticPosition.RadiusOfPrimeVertical + StateCur.GeodeticPosition.Height) * StateCur.GeodeticPosition.Latitude.Cos),
                -1
            });
            var DR = MatD.Build.DenseOfDiagonalArray(new double[]
            {
                StateCur.GeodeticPosition.RadiusOfMeridian + StateCur.GeodeticPosition.Height,
                (StateCur.GeodeticPosition.RadiusOfPrimeVertical + StateCur.GeodeticPosition.Height) * StateCur.GeodeticPosition.Latitude.Cos,
                -1
            });
            var tmp = DRInv * Cbn * antennaLever;
            var (insLat, insLon, insHei) = StateCur.GeodeticPosition;
            var (gnssLat, gnssLon, gnssHei) = gnssData.GeodeticPosition;
            tmp[0] += insLat - gnssLat;
            tmp[1] += insLon - gnssLon;
            tmp[2] += insHei - gnssHei;
            var deltaZr = DR * tmp;

            var deltaZ = MatD.Build.Dense(6, 1);
            for (int i = 0; i < 3; i++)
            {
                deltaZ[i, 0] = deltaZr[i];
                deltaZ[i + 3, 0] = StateCur.Velocity[i] - gnssData.Velocity[i];
            }

            // Construct observation matrix.
            var Hk = MatD.Build.DenseOfMatrixArray(new MatD[,]
            {
                { LocationObservationMatrix() }, { VelocityObservationMatrix() }
            });

            var R = MatD.Build.Dense(6, 6, 0);
            for (int i = 0; i < 3; i++)
            {
                R[i, i] = gnssData.PosStd[i] * gnssData.PosStd[i];
                R[i + 3, i + 3] = gnssData.VelStd[i] * gnssData.VelStd[i];
            }

            KalmanFilter.Update(deltaZ, Hk, R);

            Feedback();

            MatD LocationObservationMatrix()
            {
                var Hr = MatD.Build.Dense(3, 21, 0);
                Hr.SetSubMatrix(0, 0, MatD.Build.DenseIdentity(3));
                Hr.SetSubMatrix(0, 6, Extensions.SkewSymmetricMatrix(Cbn * antennaLever));
                return Hr;
            }
            MatD VelocityObservationMatrix()
            {
                var Hv = MatD.Build.Dense(3, 21, 0);
                Hv.SetSubMatrix(0, 3, MatD.Build.DenseIdentity(3));
                var omegaien = StateCur.Omegaien;
                var omegaenn = StateCur.Omegaenn;
                var tmp = Extensions.SkewSymmetricMatrix(Cbn * antennaLever);
                var Hv3 = -Extensions.SkewSymmetricMatrix(omegaien + omegaenn) * tmp - Extensions.SkewSymmetricMatrix(Cbn * Extensions.CrossProduct(antennaLever, imuMid.DeltaAngle));
                var Hv6 = -tmp * MatD.Build.DenseOfDiagonalVector(imuMid.DeltaAngle);
                Hv.SetSubMatrix(0, 6, Hv3);
                Hv.SetSubMatrix(0, 9, -tmp);
                Hv.SetSubMatrix(0, 15, Hv6);
                return Hv;
            }
        }

        private void Feedback()
        {
            if (StateCur == null)
                return;
            var DRInv = MatD.Build.DenseOfDiagonalArray(new double[]
            {
                1 / (StateCur.GeodeticPosition.RadiusOfMeridian + StateCur.GeodeticPosition.Height),
                1 / ((StateCur.GeodeticPosition.RadiusOfPrimeVertical + StateCur.GeodeticPosition.Height) * StateCur.GeodeticPosition.Latitude.Cos),
                -1
            });
            var tmp = DRInv * PositionError;
            StateCur.geodeticPosition.Latitude -= new Angle(tmp[0]);
            StateCur.geodeticPosition.Longitude -= new Angle(tmp[1]);
            StateCur.geodeticPosition.Height -= tmp[2];
            StateCur.Attitude = Quaternion.FromRotationVector(AttitudeError) * StateCur.Attitude;
            StateCur.Velocity -= VelocityError;
            ImuErrorVec += new ImuError(GyroBiasError, GyroScaleError, AcceBiasError, AcceScaleError);
            KalmanFilter.State = MatD.Build.Dense(21, 1, 0);
        }

        /// <summary>
        /// Construct the state transition matrix.
        /// </summary>
        /// <param name="fb">The specific force at the current moment.</param>
        /// <param name="omega_ibb">The angular velocity at the current moment.</param>
        /// <param name="statePre">The state of the previous moment.</param>
        /// <param name="tgb">Relevant time of the first-order Gaussmarkov process on gyroscope bias.</param>
        /// <param name="tgs">Relevant time of the first-order Gaussmarkov process on gyroscope scale factor.</param>
        /// <param name="tab">Relevant time of the first-order Gaussmarkov process on accelerometer bias.</param>
        /// <param name="tas">Relevant time of the first-order Gaussmarkov process on accelerometer scale factor.</param>
        /// <returns></returns>
        private static MatD TransitionMatrix(VecD fb, VecD omega_ibb, StateOfEpoch statePre, VecD tgb, VecD tgs, VecD tab, VecD tas)
        {
            double vn = statePre.Velocity[0];
            double ve = statePre.Velocity[1];
            double vd = statePre.Velocity[2];
            var (phi, _, h) = statePre.GeodeticPosition;
            double RM = statePre.GeodeticPosition.RadiusOfMeridian;
            double RN = statePre.GeodeticPosition.RadiusOfPrimeVertical;
            var F = MatD.Build.Dense(21, 21, 0);
            double RMh = RM + h;
            double RNh = RN + h;
            double omega_e = statePre.GeodeticPosition.EllipsoidType.AngularVelocity;
            double gp = statePre.Gravity[2];
            double tanphi = Math.Tan(phi);
            double cosphi = Math.Cos(phi);
            double sinphi = Math.Sin(phi);
            var Cbn = statePre.Attitude.ToRotationMatrix();

            var Frr = MatD.Build.DenseOfArray(new double[,]
            {
                { -vd / RMh, 0, vn / RMh},
                { ve * tanphi / RNh, -(vd + vn * tanphi) / RNh, ve / RNh},
                { 0, 0, 0 }
            });
            var I = MatD.Build.DenseIdentity(3);
            var Fvr = MatD.Build.DenseOfArray(new double[,]
            {
                { -2 * ve * omega_e * cosphi / RMh - ve * ve / (RMh * RNh * cosphi * cosphi), 0, vn * vd / (RMh * RMh) - ve * ve * tanphi / (RNh * RNh) },
                { 2 * omega_e * (vn * cosphi - vd * sinphi) / RMh + vn * ve / (RMh * RNh * cosphi * cosphi), 0, (ve * vd + vn * ve * tanphi) / (RNh * RNh) },
                { 2 * omega_e * ve * sinphi / RMh, 0, -ve * ve / (RNh * RNh) - vn * vn / (RMh * RMh) + 2 * gp / (Math.Sqrt(RM * RN) + h) }
            });
            var Fvv = MatD.Build.DenseOfArray(new double[,]
            {
                { vd / RMh, -2 * (omega_e * sinphi + ve * tanphi / RNh), vn / RMh },
                { 2 * omega_e * sinphi + ve * tanphi / RNh, (vd + vn * tanphi) / RNh, 2 * omega_e * cosphi + ve / RNh},
                { -2 * vn / RMh, -2 * (omega_e * cosphi + ve / RNh), 0}
            });
            var Fphir = MatD.Build.DenseOfArray(new double[,]
            {
                { -omega_e * sinphi / RMh, 0, ve / (RNh * RNh) },
                { 0, 0, -vn / (RMh * RMh) },
                { -omega_e * cosphi / RMh - ve / (RMh * RNh * cosphi * cosphi), 0, -ve * tanphi / (RNh * RNh) }
            });
            var Fphiv = MatD.Build.DenseOfArray(new double[,]
            {
                { 0, 1 / RNh, 0 },
                { -1 / RMh, 0, 0 },
                { 0, -tanphi / RNh, 0 }
            });
            var F12 = Extensions.SkewSymmetricMatrix(Cbn * fb);
            var F14 = Cbn;
            var F16 = Cbn * MatD.Build.DenseOfDiagonalVector(fb);
            var F22 = -Extensions.SkewSymmetricMatrix(statePre.Omegaien + statePre.Omegaenn);
            var F23 = -Cbn;
            var F25 = -Cbn * MatD.Build.DenseOfDiagonalVector(omega_ibb);
            var F33 = MatD.Build.DenseOfDiagonalVector((-1) / tgb);
            var F44 = MatD.Build.DenseOfDiagonalVector((-1) / tab);
            var F55 = MatD.Build.DenseOfDiagonalVector((-1) / tgs);
            var F66 = MatD.Build.DenseOfDiagonalVector((-1) / tas);
            F.SetSubMatrix(0, 0, Frr);
            F.SetSubMatrix(0, 3, I);
            F.SetSubMatrix(3, 0, Fvr);
            F.SetSubMatrix(3, 3, Fvv);
            F.SetSubMatrix(3, 6, F12);
            F.SetSubMatrix(3, 12, F14);
            F.SetSubMatrix(3, 18, F16);
            F.SetSubMatrix(6, 0, Fphir);
            F.SetSubMatrix(6, 3, Fphiv);
            F.SetSubMatrix(6, 6, F22);
            F.SetSubMatrix(6, 9, F23);
            F.SetSubMatrix(6, 15, F25);
            F.SetSubMatrix(9, 9, F33);
            F.SetSubMatrix(12, 12, F44);
            F.SetSubMatrix(15, 15, F55);
            F.SetSubMatrix(18, 18, F66);
            return F;
        }

        private static MatD CofficientMatrixG(StateOfEpoch state)
        {
            var G = MatD.Build.Dense(21, 18, 0);
            var Cbn = state.Attitude.ToRotationMatrix();
            G.SetSubMatrix(3, 0, Cbn);
            G.SetSubMatrix(6, 3, Cbn);
            for (int i = 9; i < 21; i++)
            {
                G[i, i - 3] = 1;
            }
            return G;
        }

        private void ZuptUpdate()
        {
            if (Zupt != null && Zupt.Average < 0.0002)
            {
                var H = MatD.Build.Dense(3, 21, 0);
                H[0, 3] = 1;
                H[1, 4] = 1;
                H[2, 5] = 1;
                var R = MatD.Build.DenseDiagonal(3, 0.01);
                KalmanFilter.Update(StateCur!.Velocity.ToColumnMatrix(), H, R);
                Feedback();
            }
        }

        public class ZuptWindow
        {
            public Queue<double> Window;

            public double Sum { get; private set; }

            public double Average { get; private set; }

            public double Length { get; }

            public ZuptWindow(int windowLength)
            {
                Length = windowLength;
                Window = new(windowLength + 1);
                Sum = 0;
                Average = 0;
            }

            public void Add(double element)
            {
                Window.Enqueue(element);
                Sum += element;
                if (Window.Count == Length + 1)
                {
                    Window.Dequeue();
                    Sum -= Window.First();
                }
                Average = Sum / Window.Count;
            }
        }

        public record class ImuError
        {
            public VecD GyroBias;

            public VecD GyroScale;

            public VecD AcceBias;

            public VecD AcceScale;

            public static ImuError Zero
            {
                get
                {
                    return new ImuError(VecD.Build.Dense(3, 0), VecD.Build.Dense(3, 0), VecD.Build.Dense(3, 0), VecD.Build.Dense(3, 0));
                }
            }

            public ImuError(VecD gyroBias, VecD gyroScale, VecD acceBias, VecD acceScale)
            {
                GyroBias = gyroBias;
                GyroScale = gyroScale;
                AcceBias = acceBias;
                AcceScale = acceScale;
            }

            public void Deconstruct(out VecD gyroBias, out VecD gyroScale, out VecD acceBias, out VecD acceScale)
            {
                gyroBias = GyroBias;
                gyroScale = GyroScale;
                acceBias = AcceBias;
                acceScale = AcceScale;
            }

            public static ImuError operator +(ImuError left, ImuError right)
            {
                var (lGyroBias, lGyroScale, lAcceBias, lAcceScale) = left;
                var (rGyroBias, rGyroScale, rAcceBias, rAcceScale) = right;
                return new(lGyroBias + rGyroBias, lGyroScale + rGyroScale, lAcceBias + rAcceBias, lAcceScale + rAcceScale);
            }

            public static ImuError operator -(ImuError left, ImuError right)
            {
                var (lGyroBias, lGyroScale, lAcceBias, lAcceScale) = left;
                var (rGyroBias, rGyroScale, rAcceBias, rAcceScale) = right;
                return new(lGyroBias - rGyroBias, lGyroScale - rGyroScale, lAcceBias - rAcceBias, lAcceScale - rAcceScale);
            }
        }
    }
}
