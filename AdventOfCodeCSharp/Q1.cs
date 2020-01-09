using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCodeCSharp
{
    class Q1
    {
        static void Main(string[] args)
        {
            Console.WriteLine(File.ReadAllLines("input.txt").Sum(x => (int.Parse(x) / 3) - 2));
        }
    }
}
