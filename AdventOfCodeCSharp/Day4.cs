using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdventOfCodeCSharp
{
    class Day4
    {
        const int min = 356261;
        const int max = 846303;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            bool adjacent;
            bool gap;
            bool nonDecreasing;
            int adjacentVal;
            int currDiv;
            int prevDiv;
            int curr;
            int prev;

            //int goodPasswords = 0;
            List<int> goodPasswords = new List<int>();

            for(int i = min; i <= max; i++)
            {
                adjacent = false;
                nonDecreasing = true;
                currDiv = 10000;
                prevDiv = 100000;

                for (int n = 0; n < 5; n++)
                {
                    curr = (i/currDiv)%10;
                    prev = (i/prevDiv)%10;

                    //adjacency is satisfied
                    if(curr == prev)
                    {
                        adjacent = true;
                    }

                    //nonDecreasing is not
                    if(prev > curr)
                    {
                        nonDecreasing = false;
                        break;
                    }
                    currDiv/=10;
                    prevDiv/=10;
                }

                if(adjacent && nonDecreasing)
                {
                    goodPasswords.Add(i);
                }
            }

            Console.WriteLine($"Good passwords part 1: {goodPasswords.Count}");


            List<int> newGoodPasswords = new List<int>();

            for (int i = 0; i < goodPasswords.Count; i++)
            {
                adjacent = false;
                gap = false;
                nonDecreasing = true;
                adjacentVal = -1;
                currDiv = 10000;
                prevDiv = 100000;
                int pw = goodPasswords[i];

                for (int n = 0; n < 5; n++)
                {
                    curr = (pw / currDiv) % 10;
                    prev = (pw / prevDiv) % 10;

                    //adjacency is satisfied and no previous adjacent pair has been found
                    if (curr == prev && !gap)
                    {
                        //new adjacent value found
                        if(adjacentVal != curr)
                        {
                            adjacentVal = curr;
                            adjacent = true;
                        }
                        //More than two nums in sequence
                        else
                        {
                            adjacent = false;
                        }
                    }
                    //if adjacentcy has been satisfied and we have found a gap
                    else if (adjacent)
                    {
                        gap = true;
                    }

                    //not nonDecreasing
                    if (prev > curr)
                    {
                        nonDecreasing = false;
                        break;
                    }
                    currDiv /= 10;
                    prevDiv /= 10;
                }

                if (adjacent && nonDecreasing)
                {
                    newGoodPasswords.Add(pw);
                }
            }

            Console.WriteLine($"Good passwords part 2: {newGoodPasswords.Count}");

            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
