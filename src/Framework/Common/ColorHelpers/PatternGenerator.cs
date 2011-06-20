using System;

namespace Kinect.Common.ColorHelpers
{
    internal class PatternGenerator
    {
        public static string NextPattern(int index)
        {
            switch (index%7)
            {
                case 0:
                    return "{0}0000";
                case 1:
                    return "00{0}00";
                case 2:
                    return "0000{0}";
                case 3:
                    return "{0}{0}00";
                case 4:
                    return "{0}00{0}";
                case 5:
                    return "00{0}{0}";
                case 6:
                    return "{0}{0}{0}";
                default:
                    throw new Exception("Math error");
            }
        }
    }
}