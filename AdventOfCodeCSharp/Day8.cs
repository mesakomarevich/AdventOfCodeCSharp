using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCodeCSharp
{
    class Day8
    {
        static Stopwatch watch = new Stopwatch();

        static void Main(string[] args)
        {
            watch.Start();
            int[] nums;
            int width = 25;
            int height = 6;
            int layerSize = width * height;

            nums = File.ReadAllText("Day8Input.txt").ToArray()
                .Select(x => (int)char.GetNumericValue(x))
                .ToArray();

            // nums = new int[] { 0, 2, 2, 2, 1, 1, 2, 2, 2, 2, 1, 2, 0, 0, 0, 0 };

            List<int[]> layers = new List<int[]>();

            for (int i = 0; i < (nums.Length / layerSize); i++)
            {
                layers.Add(nums.Skip(layerSize * i).Take(layerSize).ToArray());
            }

            //layers = layers.OrderBy(x => x.Count(n => n == 0)).ToList();

            //int[] layer = layers.First();
            int[] layer = layers.OrderBy(x => x.Count(n => n == 0)).ToList().First();

            Console.WriteLine($"Part 1 1s * 2s: {layer.Count(x => x == 1) * layer.Count(x => x == 2)}");


            layer = new int[layerSize];

            Parallel.For(0, layerSize, (i) =>
            {
                int color;

                color = layers.FirstOrDefault(x => x[i] != 2)?[i] ?? 2;
                layer[i] = color;
            });

            for (int i = 0; i < height; i++)
            {
                string line = "";
                for (int n = 0; n < width; n++)
                {
                    line += layer[i * width + n] == 1 ? '1' : ' ';
                }
                Console.WriteLine(line);
            }

            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
