using Ginss.Core;
using Ginss.Core.GINSS;
using Ginss.Core.INS;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NaviTools;
using NaviTools.Attitude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.Service
{
    public static class GinssService
    {
        public static AscFileReader? ascReader;

        public static GnssFileReader? gnssReader;

        public static StaticAlignment? staticAlignment;

        public static ImuErrorModel.Gyroscope gyroErrorModel = new();

        public static ImuErrorModel.Accelerometer acceErrorModel = new();

        public static LooseCombination? lcProcessor;

        public static Vector<double>? antennaLever;

        public static double progress;

        public static int count;

        public static void Initialize()
        {
            ascReader = null;
            lcProcessor = null;
            staticAlignment = null;
        }

        public static IEnumerable<StateOfEpoch> Process()
        {
            Initialize();
            ImuErrorModel errorModel = new(gyroErrorModel!, acceErrorModel!);
            ImuData? imuDataPre = null;
            InsMechanization? insMech = null;
            int idx = 0;
            IEnumerator<GnssData> gnssIterator = gnssReader!.Read().GetEnumerator();
            ImuData.SamplingRate = CalculateService.imuSamplingRate;
            ascReader = new(CalculateService.ascFilePath, CalculateService.startTime, CalculateService.endTime);
            staticAlignment = new(CalculateService.staticTimeSpan, CalculateService.initialPosition);
            var collection = ascReader.Read().DistinctBy(imuData => imuData.Time);
            count = collection.Count();
            foreach (var imuDataCur in collection)
            {
                idx++;
                EulerAngle? eulerAngle;
                eulerAngle = staticAlignment.RoughAlignmentProcessor(imuDataCur);
                if (eulerAngle != null)
                {
                    // Static alignment completes.
                    Quaternion iniAttitude = Quaternion.FromEulerAngle((EulerAngle)eulerAngle);
                    Vector<double> iniVel = Vector<double>.Build.DenseOfArray(new double[]
                    {
                        0, 0, 0
                    });
                    StateOfEpoch statePre = new(imuDataPre!.Time, iniAttitude, CalculateService.initialPosition, iniVel);
                    StateOfEpoch statePrePre = new(imuDataPre!.Time, iniAttitude, CalculateService.initialPosition, iniVel);
                    insMech = new(imuDataPre, statePre, statePrePre);
                    GnssData? gnssData = null;
                    while (gnssIterator.MoveNext())
                    {
                        gnssData = gnssIterator.Current;
                        if (gnssData.Time > imuDataCur.Time)
                            break;
                    }
                    lcProcessor = new(insMech, LooseCombination.ImuError.Zero, errorModel, Matrix<double>.Build.Dense(21, 1, 0), gnssData!.PosStd, Vector<double>.Build.Dense(3, 0), Vector<double>.Build.Dense(3, 0.05.ToRad()), 50);
                }
                if (lcProcessor != null)
                {
                    var imuList = ImuDataInterpolation.Process(imuDataPre, imuDataCur, ImuData.SamplingRate);
                    foreach (var imuMid in imuList)
                    {
                        lcProcessor.ForwardProcess(imuMid, gnssIterator, antennaLever);
                        yield return lcProcessor.StateCur!;
                        //writer.Write(lcProcessor.StateCur!.ToString("degval", null, ',') + "\r\n");
                    }
                    lcProcessor.ForwardProcess(imuDataCur, gnssIterator, antennaLever);
                    yield return lcProcessor.StateCur!;
                    //writer.Write(lcProcessor.StateCur!.ToString("degval", null, ',') + "\r\n");
                }

                progress = idx * 100.0 / count;
                imuDataPre = imuDataCur;
            }

        }
    }
}
