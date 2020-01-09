using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCodeCSharp
{
    class Day2
    {
        public static void printNums(int[] nums)
        {
            Console.WriteLine("\n\n");
            for (int i = 0; i < nums.Length; i++)
            {
                Console.Write($"{nums[i]} ");
                if (i > 0 && i % 4 == 0)
                {
                    Console.WriteLine("");
                }
            }
            Console.WriteLine($"val: {nums[0]}");
        }

        public static int run(int[] nums, int i, int n)
        {
            bool exit = false;
            nums[1] = i;
            nums[2] = n;

            for (i = 0; i < nums.Length && !exit; i += 4)
            {
                switch (nums[i])
                {
                    case 1:
                        nums[nums[i + 3]] = nums[nums[i + 1]] + nums[nums[i + 2]];
                        break;

                    case 2:
                        nums[nums[i + 3]] = nums[nums[i + 1]] * nums[nums[i + 2]];
                        break;

                    case 99:
                        exit = true;
                        break;
                    default:
                        exit = true;
                        Console.WriteLine("You done fucked up");
                        break;
                }
            }
            
            return nums[0];
        }

        static void Main(string[] args)
        {
            
            int[] nums = File.ReadAllText("Day2Input.txt").Split(',')
                .Select(x => int.Parse(x)).ToArray();

            int[] test = new int[nums.Length];

            //brute force this mf
            for(int i = 0; i < 100; i++)
            {
                for(int n = 0; n < 100; n++)
                {
                    nums.CopyTo(test, 0);
                    if(run(test, i, n) == 19690720)
                    {
                        printNums(test);
                        Console.WriteLine($"i {i}\nn {n}\n");
                    }
                }
            }
        }
    }
}
