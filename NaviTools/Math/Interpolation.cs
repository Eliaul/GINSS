using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.MathTools
{
    public static class Interpolation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">Endpoint p1</param>
        /// <param name="p2">Endpoint p2</param>
        /// <param name="x">Interpolation point</param>
        /// <returns></returns>
        public static double LinerInterpolation((double, double) p1, (double, double) p2, double x)
        {
            var (x1, y1) = p1;
            var (x2, y2) = p2;
            return y1 + (x - x1) * (y2 - y1) / (x2 - x1);
        }


    }
}
