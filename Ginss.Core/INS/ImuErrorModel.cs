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
        (
            Vector<double> Bias,
            Vector<double> ScaleFactor, 
            Vector<double> ARW,
            Vector<double> BiasProcessNoise,
            Vector<double> ScaleFactorProcessNoise,
            Vector<double> BiasRelevantTime,
            Vector<double> ScaleFactorRelevantTime
        );


        public record class Accelerometer
        (
            Vector<double> Bias,
            Vector<double> ScaleFactor,
            Vector<double> VRW,
            Vector<double> BiasProcessNoise,
            Vector<double> ScaleFactorProcessNoise,
            Vector<double> BiasRelevantTime,
            Vector<double> ScaleFactorRelevantTime
        );
    }
}
