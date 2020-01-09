using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCodeCSharp
{
    public static class Perf
    {
        public static long MaxMemory { get; set; } = 0;
        public static long Temp { get; set; } = 0;

        public static void CheckMemory()
        {
            Temp = GC.GetTotalMemory(false);
            if(Temp > MaxMemory)
            {
                MaxMemory = Temp;
            }
        }

        public static string HumanReadableBytes(double num)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (num >= 1024 && order < sizes.Length - 1)
            {
                order++;
                num = num / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", num, sizes[order]);
            return result;
        }

        public static void Print()
        {
            Console.WriteLine($"Maximum Memory recorded during execution: " + HumanReadableBytes(MaxMemory));
        }
    }
}
