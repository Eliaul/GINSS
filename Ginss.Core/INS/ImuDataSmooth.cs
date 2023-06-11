using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.INS
{
    public class ImuDataSmoother
    {
        public readonly int Windows;

        public readonly Queue<ImuData> imuDatas;

        public ImuDataSmoother(int windows) 
        {
            Windows = windows;
            imuDatas = new(Windows);
        }

        public void Add(ImuData imuData)
        {
            imuDatas.Enqueue(imuData);
            if (imuDatas.Count == Windows + 1)
                imuDatas.Dequeue();
        }

        public ImuData Smooth()
        {
            Vector<double> deltaVelSum = Vector<double>.Build.Dense(3, 0);
            Vector<double> deltaAngSum = Vector<double>.Build.Dense(3, 0);
            foreach (var imuData in imuDatas)
            {
                deltaVelSum += imuData.DeltaVelocity;
                deltaAngSum += imuData.DeltaAngle;
            }
            var count = imuDatas.Count;
            return new(imuDatas.Last().Time, deltaVelSum / count, deltaAngSum / count);
        }


    }
}
