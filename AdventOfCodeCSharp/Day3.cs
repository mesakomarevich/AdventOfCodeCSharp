using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace AdventOfCodeCSharp
{


    class Day3
    {
        enum direction
        {
            U = 0,
            D = 1,
            L = 2,
            R = 3
        }

        static int uMax = 0;
        static int dMax = 0;
        static int lMax = 0;
        static int rMax = 0;
        static int yMax = 0;
        static int xMax = 0;
        static int yPort = 0;
        static int xPort = 0;

        static List<Point> points = new List<Point>();

        struct Path
        {
            public int num { get; set; }
            public direction dir { get; set; }

            public Path(int newNum, direction newDir)
            {
                num = newNum;
                dir = newDir;
            }

            public void ApplyPath(ref int y, ref int x)
            {
                switch (dir)
                {
                    case direction.U:
                        y++;
                        break;

                    case direction.D:
                        y--;
                        break;

                    case direction.L:
                        x--;
                        break;

                    case direction.R:
                        x++;
                        break;
                }
            }
        }

        struct Point
        {
            public int x { get; set; }
            public int y { get; set; }

            public int distance { get; set; }

            public int w1Steps { get; set; }

            public int w2Steps { get; set; }

            public int steps => w1Steps + w2Steps;

            public Point(int newY, int newX)
            {
                y = newY;
                x = newX;

                distance = Math.Abs(y - (yPort)) + Math.Abs(x - (xPort));

                w1Steps = 0;
                w2Steps = 0;
            }
        }

        static List<Path> GetPaths(string pathStrs)
        {
            List<string> strs = pathStrs.Split(',').ToList();
            List<Path> paths = new List<Path>();

            foreach (var str in strs)
            {
                string dir = str.Substring(0, 1);
                int num = int.Parse(str.Substring(1, str.Length - 1));

                if (dir == "U")
                {
                    paths.Add(new Path(num, direction.U));
                }
                else if (dir == "D")
                {
                    paths.Add(new Path(num, direction.D));
                }
                else if (dir == "L")
                {
                    paths.Add(new Path(num, direction.L));
                }
                else if (dir == "R")
                {
                    paths.Add(new Path(num, direction.R));
                }
            }

            return paths;
        }

        //static void FindMax(List<Path> paths, ref int uMax, ref int dMax, ref int lMax, ref int rMax)
        static void FindMax(List<Path> paths)
        {
            int y = 0;
            int x = 0;

            foreach (var path in paths)
            {
                switch (path.dir)
                {
                    case direction.U:
                        y += path.num;
                        uMax = uMax < y ? y : uMax;
                        break;

                    case direction.D:
                        y -= path.num;
                        dMax = dMax > y ? y : dMax;
                        break;

                    case direction.L:
                        x -= path.num;
                        lMax = lMax > x ? x : lMax;
                        break;

                    case direction.R:
                        x += path.num;
                        rMax = rMax < x ? x : rMax;
                        break;
                }
            }
        }

        static int[,] AllocArray()
        {
            if (Math.Abs(uMax) > Math.Abs(dMax))
            {
                yMax = (2 * Math.Abs(uMax)) + 10;
            }
            else
            {
                yMax = (2 * Math.Abs(dMax)) + 10;
            }

            if (Math.Abs(lMax) > Math.Abs(rMax))
            {
                xMax = (2 * Math.Abs(lMax)) + 10;
            }
            else
            {
                xMax = (2 * Math.Abs(rMax)) + 10;
            }

            yPort = yMax / 2;
            xPort = xMax / 2;

            Console.WriteLine($"grid bytes: {yMax * xMax * sizeof(int)}");
            return new int[yMax, xMax];
        }


        //mark indicates which wire is being plotted
        static void PlotWire(List<Path> paths, int[,] grid, int mark)
        {
            int yPos = yPort;
            int xPos = xPort;
            int steps = 0;

            int idx = 0;

            foreach (var path in paths)
            {
                for (int i = 0; i < path.num; i++)
                {
                    //don't remark anywhere we have already been
                    if (grid[yPos, xPos] != mark && grid[yPos, xPos] != 3)
                    {
                        grid[yPos, xPos] += mark;
                    }

                    if (grid[yPos, xPos] == 3 && !(yPos == yPort && xPos == xPort))
                    {
                        Point point;

                        if (points.Any(p => p.y == yPos && p.x == xPos))
                        {
                            point = points.Where(p => p.y == yPos && p.x == xPos).SingleOrDefault();
                            //remove the old point
                            points.Remove(point);
                        }
                        else
                        {
                            point = new Point(yPos, xPos);
                        }

                        //Add steps to the point
                        if (mark == 1)
                        {
                            if (point.w1Steps == 0)
                            {
                                point.w1Steps = steps;
                            }
                        }
                        else
                        {
                            if (point.w2Steps == 0)
                            {
                                point.w2Steps = steps;
                            }
                        }
                        points.Add(point);
                    }

                    path.ApplyPath(ref yPos, ref xPos);
                    steps++;
                }
                
                Perf.CheckMemory();
                if(idx % 100 == 0)
                {
                    Console.WriteLine("Pausing ");

                    Console.ReadLine();
                }
                
                idx++;
            }
        }

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string[] lines = File.ReadAllLines("Day3Input.txt");
            List<Path> w1 = GetPaths(lines[0]);
            List<Path> w2 = GetPaths(lines[1]);

            FindMax(w1);
            FindMax(w2);

            Console.WriteLine($"uMax {uMax}\ndMax {dMax}\nlMax {lMax}\nrMax {rMax}");

            int[,] grid = AllocArray();
            PlotWire(w1, grid, 1);
            PlotWire(w2, grid, 2);

            //Run w1 again because the steps for intersections will only be
            //recorded for w2 the first time around
            PlotWire(w1, grid, 1);

            Point closest = points.OrderBy(p => p.distance).First();

            Console.WriteLine($"Closest intersection point for part 1:\nx: {closest.x}\ny: {closest.y}\ndistance: {closest.distance}\n");


            closest = points.OrderBy(p => p.steps).First();
            Console.WriteLine($"Closest intersection point for part 2:\nx: {closest.x}\ny: {closest.y}\nw1Steps: {closest.w1Steps}\nw2Steps: {closest.w2Steps}\nsteps: {closest.steps}\n");
            watch.Stop();

            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
            //Console.WriteLine($"Array max{System.Int32.MaxValue}");

            Perf.Print();
        }
    }
}
