using Ginss.Core;
using Ginss.Core.GINSS;
using Ginss.Core.INS;
using MathNet.Numerics.LinearAlgebra;
using NaviTools.Attitude;
using NaviTools;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NaviTools.Geodesy;

namespace Ginss.Wpf.Service
{
    public static class InsService
    {

        public static AscFileReader? fileReader;

        public static StaticAlignment? staticAlignment;

        public static StateOfEpoch state;

        public static ImuData? imuDataPre;

        public static InsMechanization? insMech;

        //public static IEnumerable<StateOfEpoch> stateCollection;

        public static double progress;

        //public static List<StateOfEpoch> states;

        public static void Initialize()
        {
            imuDataPre = null;
            insMech = null;
            fileReader = null;
            staticAlignment = null;
            //states.Clear();
        }

        public static IEnumerable<StateOfEpoch> Process()
        {
            Initialize();
            ImuData.SamplingRate = CalculateService.imuSamplingRate;
            fileReader = new(CalculateService.filePath, CalculateService.startTime, CalculateService.endTime);
            //if (CalculateService.filePath != null && new System.IO.FileInfo(CalculateService.filePath).Exists)
            //else
            //    return;
            staticAlignment = new(CalculateService.staticTimeSpan, CalculateService.initialPosition);
            int idx = 0;
            int count = fileReader.Read().DistinctBy(imuData => imuData.Time).Count();
            foreach (var imuDataCur in fileReader.Read().DistinctBy(imuData => imuData.Time))
            {
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
                }
                if (insMech != null)
                {
                    var imuList = ImuDataInterpolation.Process(imuDataPre, imuDataCur, ImuData.SamplingRate);
                    foreach (var imuMid in imuList)
                    {
                        yield return insMech.Process(imuDataCur);
                    }
                    yield return insMech.Process(imuDataCur);
                }

                idx++;
                progress = idx * 100.0 / count;
                imuDataPre = imuDataCur;
            }
        }
    }
}
