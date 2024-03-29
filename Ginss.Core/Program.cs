﻿// See https://aka.ms/new-console-template for more information
using Ginss.Core.GINSS;
using Ginss.Core;
using Ginss.Core.INS;
using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Attitude;
using NaviTools.Geodesy;
using System.Diagnostics;

Console.WriteLine(double.Parse("1e-3"));

AscFileReader ascReader = new("D:\\Tencent\\398943960\\FileRecv\\3-2开阔环境-Decompress.ASC");
GnssFileReader gnssReader = new("D:\\Tencent\\398943960\\FileRecv\\wide.pos");
IEnumerator<GnssData> gnssIterator = gnssReader.Read().GetEnumerator();
//InsResultWriter resultWriter = new(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt");
StreamWriter writer = new("res.csv");
ImuErrorModel.Gyroscope gyroErrorModel = new(
    Vector<double>.Build.Dense(3, 0),
    Vector<double>.Build.Dense(3, 0),
    Vector<double>.Build.Dense(3, 0.2.ToRad() / 60),
    Vector<double>.Build.Dense(3, 24d.ToRad() / 3600),
    Vector<double>.Build.Dense(3, 1e-3),
    Vector<double>.Build.Dense(3, 3600),
    Vector<double>.Build.Dense(3, 3600)
);
ImuErrorModel.Accelerometer acceErrorModel = new(
    Vector<double>.Build.Dense(3, 0),
    Vector<double>.Build.Dense(3, 0),
    Vector<double>.Build.Dense(3, 0.4 / 60),
    Vector<double>.Build.Dense(3, 4 * 1e-3),
    Vector<double>.Build.Dense(3, 1e-3),
    Vector<double>.Build.Dense(3, 3600),
    Vector<double>.Build.Dense(3, 3600)
);
ImuErrorModel errorModel = new(gyroErrorModel, acceErrorModel, "XW-GI6615");
Vector<double> antennaLever = Vector<double>.Build.DenseOfArray(new double[]
{
    -0.1000,0.2350, -0.8900
});


Stopwatch sw = new();
sw.Start();

ImuData.SamplingRate = 0.01;
ImuData? imuDataPre = null;

GeodeticCoordinate iniPos = new(30.5282960229, 114.3557510106, 23.141, Ellipsoid.WGS84, AngleUnit.deg);

StaticAlignment staticAlignment = new(new(0, 5, 0), iniPos);

StateOfEpoch? stateCur = null;
InsMechanization? insMech = null;
LooseCombination? lcProcessor = null;
foreach (var imuDataCur in ascReader.Read().DistinctBy(imuData => imuData.Time))
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
        StateOfEpoch statePre = new(imuDataPre!.Time, iniAttitude, iniPos, iniVel);
        StateOfEpoch statePrePre = new(imuDataPre!.Time, iniAttitude, iniPos, iniVel);
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
            var cov1 = lcProcessor.KalmanFilter.Cov;
            var time1 = lcProcessor.StateCur.GpsSeconds;
            writer.Write($"{time1:F3},{Math.Sqrt(cov1[0, 0]):F4},{Math.Sqrt(cov1[1,1]):F4},{Math.Sqrt(cov1[2,2]):F4}\r\n");
        }
        lcProcessor.ForwardProcess(imuDataCur, gnssIterator, antennaLever);
        var cov = lcProcessor.KalmanFilter.Cov;
        var time = lcProcessor.StateCur.GpsSeconds;
        writer.Write($"{time:F3},{Math.Sqrt(cov[0, 0]):F4},{Math.Sqrt(cov[1, 1]):F4},{Math.Sqrt(cov[2, 2]):F4}\r\n");
    }

    imuDataPre = imuDataCur;
}
stateCur = lcProcessor!.StateCur;
sw.Stop();
Console.WriteLine(sw.Elapsed.TotalSeconds);