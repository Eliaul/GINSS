using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools
{
    public static class LeapSeconds
    {
        public readonly static Dictionary<DateTime, int> GpsTimeTable = new() 
        {
            {new(2016, 12, 31, 23, 59, 59), 18},
            {new(2015, 6, 30, 23, 59, 59), 17},   
            {new(2012, 6, 30, 23, 59, 59), 16},    
            {new(2008, 12, 31, 23, 59, 59), 15},    
            {new(2005, 12, 31, 23, 59, 59), 14},    
            {new(1998, 12, 31, 23, 59, 59), 13},    
            {new(1997, 6, 30, 23, 59, 59), 12},    
            {new(1995, 12, 31, 23, 59, 59), 11},    
            {new(1994, 6, 30, 23, 59, 59), 10},    
            {new(1993, 6, 30, 23, 59, 59), 9},    
            {new(1992, 6, 30, 23, 59, 59), 8},    
            {new(1990, 12, 31, 23, 59, 59), 7},    
            {new(1989, 12, 31, 23, 59, 59), 6},    
            {new(1987, 12, 31, 23, 59, 59), 5},    
            {new(1985, 6, 30, 23, 59, 59), 4},    
            {new(1983, 6, 30, 23, 59, 59), 3},    
            {new(1982, 6, 30, 23, 59, 59), 2},    
            {new(1981, 6, 30, 23, 59, 59), 1},   
        };

        

        public static TimeSpan FindLeapSeconds(UtcTime utcTime)
        {
            var utc = utcTime.UtcDateTime;

            foreach (var keyVal in GpsTimeTable)
            {
                if (utc > keyVal.Key)
                {
                    return new TimeSpan(0, 0, keyVal.Value);
                }
            }

            return TimeSpan.Zero;
        }

    }
}
