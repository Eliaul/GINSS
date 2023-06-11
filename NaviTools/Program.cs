// See https://aka.ms/new-console-template for more information


using MathNet.Numerics.LinearAlgebra;
using NaviTools;
using NaviTools.Attitude;
using NaviTools.Geodesy;
using NaviTools.UnitTest;
using System.Globalization;

//TestEulerAngle.TestCtor1();
//TestEulerAngle.TestCtor2();
//TestEulerAngle.TestFormat1();
//TestEulerAngle.TestFormat2();
//TestEulerAngle.TestEquals1();
//TestEulerAngle.TestEquals2();

Queue<double> queue = new(10);

queue.Enqueue(1);
queue.Enqueue(2);

Console.WriteLine(queue.First());