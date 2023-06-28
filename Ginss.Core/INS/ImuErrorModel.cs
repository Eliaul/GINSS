using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ginss.Core.INS.ImuErrorModel;

namespace Ginss.Core.INS
{
    public record class ImuErrorModel(Gyroscope GyroscopeError, Accelerometer AccelerometerError, string TypeName = "Unknown")
    {
        public record class Gyroscope
        {
            public Vector<double> Bias;
            public Vector<double> ScaleFactor;
            public Vector<double> ARW;
            public Vector<double> BiasProcessNoise;
            public Vector<double> ScaleFactorProcessNoise;
            public Vector<double> BiasRelevantTime;
            public Vector<double> ScaleFactorRelevantTime;

            public Gyroscope(Vector<double> bias, Vector<double> scaleFactor, Vector<double> aRW, Vector<double> biasProcessNoise, Vector<double> scaleFactorProcessNoise, Vector<double> biasRelevantTime, Vector<double> scaleFactorRelevantTime)
            {
                Bias = bias;
                ScaleFactor = scaleFactor;
                ARW = aRW;
                BiasProcessNoise = biasProcessNoise;
                ScaleFactorProcessNoise = scaleFactorProcessNoise;
                BiasRelevantTime = biasRelevantTime;
                ScaleFactorRelevantTime = scaleFactorRelevantTime;
            }
            public Gyroscope()
            {
                var zero = Vector<double>.Build.Dense(3, 0);
                Bias = zero;
                ScaleFactor = zero;
                ARW = zero;
                BiasProcessNoise = zero;
                ScaleFactorProcessNoise = zero;
                BiasRelevantTime = zero;
                ScaleFactorRelevantTime = zero;
            }
        }


        //public record class Accelerometer
        //(
        //    Vector<double> Bias,
        //    Vector<double> ScaleFactor,
        //    Vector<double> VRW,
        //    Vector<double> BiasProcessNoise,
        //    Vector<double> ScaleFactorProcessNoise,
        //    Vector<double> BiasRelevantTime,
        //    Vector<double> ScaleFactorRelevantTime
        //);

        public record class Accelerometer
        {
            public Vector<double> Bias;
            public Vector<double> ScaleFactor;
            public Vector<double> VRW;
            public Vector<double> BiasProcessNoise;
            public Vector<double> ScaleFactorProcessNoise;
            public Vector<double> BiasRelevantTime;
            public Vector<double> ScaleFactorRelevantTime;

            public Accelerometer(Vector<double> bias, Vector<double> scaleFactor, Vector<double> vRW, Vector<double> biasProcessNoise, Vector<double> scaleFactorProcessNoise, Vector<double> biasRelevantTime, Vector<double> scaleFactorRelevantTime)
            {
                Bias = bias;
                ScaleFactor = scaleFactor;
                VRW = vRW;
                BiasProcessNoise = biasProcessNoise;
                ScaleFactorProcessNoise = scaleFactorProcessNoise;
                BiasRelevantTime = biasRelevantTime;
                ScaleFactorRelevantTime = scaleFactorRelevantTime;
            }
            public Accelerometer()
            {
                var zero = Vector<double>.Build.Dense(3, 0);
                Bias = zero;
                ScaleFactor = zero;
                VRW = zero;
                BiasProcessNoise = zero;
                ScaleFactorProcessNoise = zero;
                BiasRelevantTime = zero;
                ScaleFactorRelevantTime = zero;
            }
        }
    }
}
