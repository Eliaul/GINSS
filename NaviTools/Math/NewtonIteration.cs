using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.MathTools
{
    public static class NewtonIteration
    {
        public static double Solve(double initialValue, double tolerance, int maxIterationNum, Func<double, double> funtionDivDerivative)
        {
            double xPre = initialValue;
            double xCur = initialValue - funtionDivDerivative(initialValue);
            int count = 0;
            while (Math.Abs(xCur - xPre) > tolerance)
            {
                xPre = xCur;
                count++;
                xCur -= funtionDivDerivative(xCur);
                if (count > maxIterationNum)
                    break;
            }
            return xCur;
        }

        public static double Solve(double initialValue, double tolerance, int maxIterationNum, Func<double, double> funtion, Func<double, double> derivativeFunction)
        {
            double xPre = initialValue;
            double xCur = initialValue - funtion(initialValue) / derivativeFunction(initialValue);
            int count = 0;
            while (System.Math.Abs(xCur - xPre) > tolerance)
            {
                xPre = xCur;
                count++;
                xCur -= funtion(xCur) / derivativeFunction(xCur);
                if (count > maxIterationNum)
                    break;
            }
            return xCur;
        }
    }
}
