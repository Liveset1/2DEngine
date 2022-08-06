using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FlexileEngine.Runtime.utils
{
    public class Time
    {
        public static float timeStarted = Stopwatch.GetTimestamp();

        public static float getTime()
        {
            return Stopwatch.GetTimestamp()-timeStarted/Stopwatch.Frequency;
        }
    }
}
