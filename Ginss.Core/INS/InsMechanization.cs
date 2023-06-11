using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Attitude;
using NaviTools.Geodesy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.INS
{
    using VectorD = MathNet.Numerics.LinearAlgebra.Vector<double>;

    public class InsMechanization
    {

        private static double deltaTime = ImuData.SamplingRate;

        public ImuData ImuPre { get; private set; }

        public StateOfEpoch StatePre { get; private set; }

        public StateOfEpoch StatePrePre { get; private set; }

        public InsMechanization(ImuData imuPre, StateOfEpoch statePre, StateOfEpoch statePrePre)
        {
            ImuPre = imuPre;
            StatePre = statePre;
            StatePrePre = statePrePre;
        }

        public StateOfEpoch Process(ImuData imuCur)
        {
            StateOfEpoch stateCur = Update(StatePre, StatePrePre, imuCur, ImuPre);
            StatePrePre = StatePre;
            StatePre = stateCur;
            ImuPre = imuCur;
            return stateCur;
        }

        public static StateOfEpoch Update(StateOfEpoch sPre, StateOfEpoch sPrePre, ImuData imuCur, ImuData imuPre)
        {
            deltaTime = (imuCur.Time - imuPre.Time).TotalSeconds;
            //deltaTime = 0.01;
            Quaternion attitude = AttitudeUpdate(sPre, imuPre, imuCur);
            VectorD velocity = VelocityUpdate(sPrePre, sPre, imuPre, imuCur);
            GeodeticCoordinate position = PositionUpdate(sPre, sPrePre, velocity);
            return new(imuCur.Time, attitude, position, velocity);
        }

        private static Quaternion AttitudeUpdate(StateOfEpoch sPre, ImuData imuPre, ImuData imuCur)
        {
            //更新b系
            VectorD phi_k = imuCur.DeltaAngle + Extensions.CrossProduct(imuPre.DeltaAngle, imuCur.DeltaAngle) / 12;
            Quaternion q1 = Quaternion.FromRotationVector(phi_k);

            //更新n系. 
            VectorD zeta_k = (sPre.Omegaien + sPre.Omegaenn) * deltaTime;
            Quaternion q2 = Quaternion.FromRotationVector(-zeta_k);

            //计算当前姿态的四元数
            Quaternion q = q2 * sPre.Attitude * q1;
            return q.Normalize();
        }

        private static VectorD VelocityUpdate(StateOfEpoch sPrePre, StateOfEpoch sPre, ImuData imuPre, ImuData imuCur)
        {
            VectorD v1 = imuCur.DeltaVelocity + Extensions.CrossProduct(imuCur.DeltaAngle, imuCur.DeltaVelocity) / 2
                + (Extensions.CrossProduct(imuPre.DeltaAngle, imuCur.DeltaVelocity) + Extensions.CrossProduct(imuPre.DeltaVelocity, imuCur.DeltaAngle)) / 12;
            VectorD Omega_ieMid = 1.5 * sPre.Omegaien - 0.5 * sPrePre.Omegaien;
            VectorD Omega_enMid = 1.5 * sPre.Omegaenn - 0.5 * sPrePre.Omegaenn;
            VectorD gMid = sPre.Gravity * 1.5 - sPrePre.Gravity * 0.5;
            VectorD zeta = (Omega_ieMid + Omega_enMid) * deltaTime;
            VectorD v2 = (Matrix<double>.Build.DenseIdentity(3) - (Extensions.SkewSymmetricMatrix(zeta) / 2)) * sPre.Attitude.ToRotationMatrix() * v1;
            VectorD tempBack = Extensions.CrossProduct(2 * sPre.Omegaien + sPre.Omegaenn, sPre.Velocity);
            VectorD tempBBack = Extensions.CrossProduct(2 * sPrePre.Omegaien + sPrePre.Omegaenn, sPrePre.Velocity);
            VectorD tempMid = 1.5 * tempBack - 0.5 * tempBBack;
            VectorD v3 = (gMid - tempMid) * deltaTime;
            return sPre.Velocity + v2 + v3;
        }

        private static GeodeticCoordinate PositionUpdate(StateOfEpoch sPre, StateOfEpoch sPrePre, VectorD vCur)
        {
            var (lat, lon, hei) = sPre.GeodeticPosition;
            double R_NMid = sPre.GeodeticPosition.RadiusOfPrimeVertical * 1.5 - sPrePre.GeodeticPosition.RadiusOfPrimeVertical * 0.5;
            double H = hei - (sPre.Velocity[2] + vCur[2]) * deltaTime * 0.5;
            double HAve = (H + hei) / 2;
            double B = lat + (sPre.Velocity[0] + vCur[0]) * deltaTime * 0.5 / (sPre.GeodeticPosition.RadiusOfMeridian + HAve);
            double BAve = (B + lat) / 2;
            double L = lon + (sPre.Velocity[1] + vCur[1]) * deltaTime * 0.5 / ((R_NMid + HAve) * Math.Cos(BAve));
            return new(B, L, H, Ellipsoid.WGS84);
        }


        //public static StateOfEpoch Update(StateOfEpoch sPre, ImuData imuCur, ImuData imuPre)
        //{
        //    deltaTime = (imuCur.Time - imuPre.Time).TotalSeconds;
        //    Quaternion attitude = AttitudeUpdate(sPre, imuPre, imuCur);
        //    VectorD velocity = VelocityUpdate(sPre, imuPre, imuCur);
        //    GeodeticCoordinate position = PositionUpdate(sPre, velocity);
        //    return new(imuCur.Time, attitude, position, velocity);
        //}

        //private static Quaternion AttitudeUpdate(StateOfEpoch sPre, ImuData imuPre, ImuData imuCur)
        //{
        //    VectorD phi_k = imuCur.DeltaAngle + Extensions.CrossProduct(imuPre.DeltaAngle, imuCur.DeltaAngle) / 12;
        //    Quaternion q1 = Quaternion.FromRotationVector(phi_k);

        //    VectorD zeta_k = (sPre.Omegaien + sPre.Omegaenn) * deltaTime;
        //    Quaternion q2 = Quaternion.FromRotationVector(-zeta_k);

        //    Quaternion q = q2 * sPre.Attitude * q1;
        //    return q.Normalize();
        //}

        //private static VectorD VelocityUpdate(StateOfEpoch sPre, ImuData imuPre, ImuData imuCur)
        //{
        //    var rotationMatrixPre = sPre.Attitude.ToRotationMatrix();
        //    VectorD d_vfb = imuCur.DeltaVelocity + Extensions.CrossProduct(imuCur.DeltaAngle, imuCur.DeltaVelocity) / 2
        //        + (Extensions.CrossProduct(imuPre.DeltaAngle, imuCur.DeltaVelocity) + Extensions.CrossProduct(imuPre.DeltaVelocity, imuCur.DeltaAngle)) / 12;

        //    VectorD temp1 = (sPre.Omegaien + sPre.Omegaenn) * (deltaTime / 2);
        //    VectorD d_vfn = (Matrix<double>.Build.DenseIdentity(3) - Extensions.SkewSymmetricMatrix(temp1)) * rotationMatrixPre * d_vfb;
        //    VectorD d_vgn = (sPre.Gravity - Extensions.CrossProduct((2 * sPre.Omegaien + sPre.Omegaenn), sPre.Velocity)) * deltaTime;
        //    VectorD midVel = sPre.Velocity + (d_vfn + d_vgn) / 2;

        //    Quaternion qnn = Quaternion.FromRotationVector(temp1);
        //    VectorD temp2 = VectorD.Build.DenseOfArray(new double[]
        //    {
        //        0, 0, sPre.GeodeticPosition.EllipsoidType.AngularVelocity * (-deltaTime / 2)
        //    });
        //    Quaternion qee = Quaternion.FromRotationVector(temp2);
        //    double B = sPre.GeodeticPosition.Latitude.Radian;
        //    double L = sPre.GeodeticPosition.Longitude.Radian;
        //    double H = sPre.GeodeticPosition.Height;
        //    Quaternion qne = new
        //    (
        //        Math.Cos(-Math.PI / 4 - B / 2) * Math.Cos(L / 2),
        //        -Math.Sin(-Math.PI / 4 - B / 2) * Math.Sin(L / 2),
        //        Math.Sin(-Math.PI / 4 - B / 2) * Math.Cos(L / 2),
        //        Math.Cos(-Math.PI / 4 - B / 2) * Math.Sin(L / 2)
        //    );
        //    Quaternion qmid = qee * qne * qnn;

        //    GeodeticCoordinate midPos = new
        //    (
        //        -2 * Math.Atan(qmid[2] / qmid[0]) - Math.PI / 2,
        //        2 * Math.Atan2(qmid[3], qmid[0]),
        //        H - midVel[2] * (deltaTime / 2),
        //        sPre.GeodeticPosition.EllipsoidType
        //    );

        //    StateMid = new(sPre.Time + TimeSpan.FromSeconds(deltaTime / 2), qmid, midPos, midVel);

        //    VectorD temp3 = (StateMid.Omegaenn + StateMid.Omegaien) * (deltaTime / 2);
        //    d_vfn = (Matrix<double>.Build.DenseIdentity(3) - Extensions.SkewSymmetricMatrix(temp3)) * rotationMatrixPre * d_vfb;

        //    d_vgn = (StateMid.Gravity - Extensions.CrossProduct((2 * StateMid.Omegaien + StateMid.Omegaenn), StateMid.Velocity)) * deltaTime;

        //    return sPre.Velocity + d_vfn + d_vgn;
        //}

        //private static GeodeticCoordinate PositionUpdate(StateOfEpoch sPre, VectorD vCur)
        //{
        //    double B = sPre.GeodeticPosition.Latitude.Radian;
        //    double L = sPre.GeodeticPosition.Longitude.Radian;
        //    double H = sPre.GeodeticPosition.Height;
        //    double R_NMid = StateMid!.GeodeticPosition.RadiusOfPrimeVertical;
        //    double HCur = H - (sPre.Velocity[2] + vCur[2]) * deltaTime * 0.5;
        //    double HAve = (HCur + H) / 2;
        //    double BCur = B + (sPre.Velocity[0] + vCur[0]) * deltaTime * 0.5 / (sPre.GeodeticPosition.RadiusOfMeridian + HAve);
        //    double BAve = (BCur + B) / 2;
        //    double LCur = L + (sPre.Velocity[1] + vCur[1]) * deltaTime * 0.5 / ((R_NMid + HAve) * Math.Cos(BAve));
        //    return new(BCur, LCur, HCur, sPre.GeodeticPosition.EllipsoidType);

        //    //VectorD midvel = (sPre.Velocity + vCur) / 2;
        //    //double B = sPre.GeodeticPosition.Latitude.Radian;
        //    //double L = sPre.GeodeticPosition.Longitude.Radian;
        //    //double H = sPre.GeodeticPosition.Height;
        //    //Matrix<double> dri = Matrix<double>.Build.DenseOfDiagonalArray(new double[]
        //    //{
        //    //    1 / (sPre.GeodeticPosition.RadiusOfMeridian + H),
        //    //    1 / ((sPre.GeodeticPosition.RadiusOfPrimeVertical + H) * Math.Cos(B)),
        //    //    -1
        //    //});
        //    //VectorD temp = dri * midvel * (deltaTime / 2);
        //    //GeodeticCoordinate midpos = new(B + temp[0], L + temp[1], H + temp[2], sPre.GeodeticPosition.EllipsoidType);

        //    //StateMid = new(sPre.Time + TimeSpan.FromSeconds(deltaTime / 2), StateMid!.Attitude, midpos, midvel);

        //    //VectorD temp1 = (StateMid.Omegaien + StateMid.Omegaenn) * deltaTime;
        //    //Quaternion qnn = Quaternion.FromRotationVector(temp1);
        //    //VectorD temp2 = VectorD.Build.DenseOfArray(new double[]
        //    //{
        //    //    0, 0, -sPre.GeodeticPosition.EllipsoidType.AngularVelocity * deltaTime
        //    //});
        //    //Quaternion qee = Quaternion.FromRotationVector(temp2);

        //    //Quaternion qne = new
        //    //(
        //    //    Math.Cos(-Math.PI / 4 - B / 2) * Math.Cos(L / 2),
        //    //    -Math.Sin(-Math.PI / 4 - B / 2) * Math.Sin(L / 2),
        //    //    Math.Sin(-Math.PI / 4 - B / 2) * Math.Cos(L / 2),
        //    //    Math.Cos(-Math.PI / 4 - B / 2) * Math.Sin(L / 2)
        //    //);

        //    //qne = qee * qne * qnn;
        //    //return new
        //    //(
        //    //    -2 * Math.Atan(qne[2] / qne[0]) - Math.PI / 2,
        //    //    2 * Math.Atan2(qne[3], qne[0]),
        //    //    H - midvel[2] * deltaTime,
        //    //    sPre.GeodeticPosition.EllipsoidType
        //    //);
        //}

    }
}
