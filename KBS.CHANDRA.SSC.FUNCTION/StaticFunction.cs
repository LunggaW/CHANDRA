using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.FUNCTION
{
    public static class StaticFunction
    {
        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            return new string((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
        }
    }
}
