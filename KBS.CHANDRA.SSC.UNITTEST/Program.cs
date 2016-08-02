using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KBS.CHANDRA.SSC.FUNCTION;

namespace KBS.CHANDRA.SSC.UNITTEST
{
    class Program
    {
        private static SSCFunction test = new SSCFunction();
        static void Main(string[] args)
        {
            try
            {
                test.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }    
        }
    }
}
