using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCodeCSharp
{
    class Q2
    {
        static void Main(string[] args)
        {
            //Func<Func<int, int>, Func<int, int>> F = fac => x => x == 0 ? 1 : x * fac(x - 1);
            //Func<Func<int, int>, int, int> fac = (_fac, x) => x == 0 ? 1 : x * _fac(x - 1);

            //Console.WriteLine(F(5));

            //Func<int, int> fuelRec => f => (f <= 0 ? f : fuelRec((f / 3) - 2));

            //Func<int, int> computeFactorial = (n) =>
            //{
            //    if (n == 0)
            //    {
            //        return 1;
            //    }

            //    return n * computeFactorial(n - 1);
            //};

            Func<int, int> fuelCalc = null;
            Func<int, int> fuelRec = null;
            fuelRec = f => (f <= 0 ? 0 : f + fuelCalc(f));
            fuelCalc = x => fuelRec((x / 3) - 2);


            //Console.WriteLine(fuelCalc(100756));

            Console.WriteLine(File.ReadAllLines("input.txt").Sum(x => fuelCalc(int.Parse(x))));

            //.Sum(x => Func<int, int> fuelCalc = ((int.Parse(x) / 3) - 2));
        }
    }
}
