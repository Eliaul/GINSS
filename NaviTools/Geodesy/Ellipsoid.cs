using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.Geodesy
{
    public readonly record struct Ellipsoid(double SemiMajorAxis, double Flatting, double AngularVelocity) : IEquatable<Ellipsoid>
    {

        public readonly double SemiMinorAxis => (1.0 - Flatting) * SemiMajorAxis;

        public readonly double FirstEccentricity => Math.Sqrt(1 - (1 - Flatting) * (1 - Flatting));

        public readonly double SecondEccentricity => Math.Sqrt(FirstEccentricity * SemiMajorAxis / SemiMinorAxis);


        #region "reference ellipsoids"

        public static readonly Ellipsoid WGS84 = new()
        { 
            SemiMajorAxis = 6378137.0,
            Flatting = 1.0 / 298.2572235629972,
            AngularVelocity = 7.292115e-5 
        };

        public static readonly Ellipsoid GRS80 = new()
        {
            SemiMajorAxis = 6378137.0,
            Flatting = 1.0 / 298.2572221008827,
            AngularVelocity = 7.292115e-5 
        };

        public static readonly Ellipsoid CGCS2000 = new() 
        {
            SemiMajorAxis = 6378137.0,
            Flatting = 1.0 / 298.2572221010042,
            AngularVelocity = 7.292115e-5 
        };


        #endregion


    }
}
