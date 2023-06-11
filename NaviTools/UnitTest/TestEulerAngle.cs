using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaviTools.Attitude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.UnitTest
{
    [TestClass]
    internal static class TestEulerAngle
    {
        [TestMethod]
        internal static void TestCtor1()
        {
            EulerAngle e1 = new(10.23, -58.21, 130.11, AngleUnit.deg);
            EulerAngle e2 = new(2, 1, Math.PI / 2);
            Console.WriteLine($"e1:{e1} e2:{e2}");
        }

        [TestMethod]
        internal static void TestCtor2()
        {
            EulerAngle e1 = new(700, -120, 200, AngleUnit.deg);
            EulerAngle e2 = new(200, 200, 200, AngleUnit.deg);
            Console.WriteLine($"e1:{e1} e2:{e2}");
        }

        [TestMethod]
        internal static void TestFormat1()
        {
            EulerAngle e = new(120, -13, 3.4, AngleUnit.deg);
            Console.WriteLine("e:{0:deg} e:{1:rad} e:{2:dms}", e, e, e);
        }

        [TestMethod]
        internal static void TestFormat2()
        {
            Console.WriteLine("e:" + new EulerAngle(1, 1, 1).ToString());
        }

        [TestMethod]
        internal static void TestEquals1()
        {
            EulerAngle e1 = new(1, 1, 1);
            EulerAngle e2 = new(2, 2, 2);
            EulerAngle e3 = new(1, 1, 1);
            double[] e4 = new double[] { 1, 1, 1 };
            Assert.IsFalse(e1.Equals(null));
            Assert.IsFalse(e1.Equals(e2));
            Assert.IsFalse(e1.Equals(e4));
            Assert.IsTrue(e1.Equals(e1));
            Assert.IsTrue(e1.Equals(e3));
        }

        [TestMethod]
        internal static void TestEquals2()
        {
            EulerAngle e1 = new(1, 1, 1);
            EulerAngle e2 = new(2, 2, 2);
            EulerAngle e3 = new(1, 1, 1);
            Assert.IsFalse(e1 == null);
            Assert.IsFalse(e1 == e2);
            Assert.IsTrue(e1 == e1);
            Assert.IsTrue(e1 == e3);
        }

    }
}
