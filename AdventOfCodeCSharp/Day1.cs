using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCodeCSharp
{
    class Day1
    {
        public static int fuelCalc(int val)
        {
            int ret = 0;

            for(;;)
            {
                val = ((val / 3) - 2);
                if(val >= 0)
                {
                    ret += val;
                }
                else
                {
                    break;
                }
            }
            return ret;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(File.ReadAllLines("Day1Input.txt")
                .Select(x => int.Parse(x))
                .Sum(x => fuelCalc(x)));
        }
    }
}
