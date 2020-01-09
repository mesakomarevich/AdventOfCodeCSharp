using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;

namespace AdventOfCodeCSharp
{
    class Day6
    {
        static Stopwatch watch = new Stopwatch();

        static List<Orbit> orbits = new List<Orbit>();

        class Orbit
        {
            public string Name { get; set; }

            public Orbit Parent { get; set; }

            public List<Orbit> Children { get; set; }

            public int Distance { get; set; }


            public Orbit()
            {
                Children = new List<Orbit>();
            }

            public Orbit(string name) : this()
            {
                Name = name;
                if (Name == "COM")
                {
                    Distance = 0;
                }
            }
        }

        static void ParseOrbits(string[] orbitStrs)
        {
            foreach (var orbitStr in orbitStrs)
            {
                string[] orbs = orbitStr.Split(')');

                Orbit orb1;
                Orbit orb2;

                orb1 = orbits.SingleOrDefault(x => x.Name == orbs[0]);
                if (orb1 == null)
                {
                    orb1 = new Orbit(orbs[0]);
                }

                orb2 = orbits.SingleOrDefault(x => x.Name == orbs[1]);
                if (orb2 == null)
                {
                    orb2 = new Orbit(orbs[1]);
                }

                orb2.Parent = orb1;
                orb1.Children.Add(orb2);

                //Add if not aleady in the list
                if (!orbits.Contains(orb1))
                {
                    orbits.Add(orb1);
                }
                if (!orbits.Contains(orb2))
                {
                    orbits.Add(orb2);
                }
            }
        }

        static void CountOrbits(Orbit orbit, ref long total)
        {
            if (orbit.Parent != null)
            {
                orbit.Distance = orbit.Parent.Distance + 1;
                total += orbit.Distance;
            }

            foreach (var child in orbit.Children)
            {
                CountOrbits(child, ref total);
            }
        }

        static void PrintOrbs(Orbit orbit)
        {
            Console.WriteLine($"Orbit: {orbit.Name}\nDistance: {orbit.Distance}");

            foreach (var child in orbit.Children)
            {
                PrintOrbs(child);
            }
        }

        static void Part1()
        {
            Orbit com = orbits.SingleOrDefault(x => x.Name == "COM");
            long total = 0;
            CountOrbits(com, ref total);
            Console.WriteLine($"Total Orbits: {total}");
        }

        static int GetOrbits(Orbit from, Orbit to, List<Orbit> path)
        {
            int length = 0;

            if (from != to)
            {
                while (from != null && from != to)
                {
                    path.Add(from);
                    from = from.Parent;
                    length++;
                } 
            }
            return length;
        }

        static void Part2()
        {
            Orbit com = orbits.SingleOrDefault(x => x.Name == "COM");
            Orbit you = orbits.SingleOrDefault(x => x.Name == "YOU");
            Orbit san = orbits.SingleOrDefault(x => x.Name == "SAN");
            List<Orbit> youPath = new List<Orbit>();
            List<Orbit> sanPath = new List<Orbit>();

            GetOrbits(you, com, youPath);
            GetOrbits(san, com, sanPath);

            //get the all common ancestors of YOU and SAN
            List<Orbit> ancestors = youPath.Intersect(sanPath).ToList();

            bool totalNotSet = true;
            int total = 0;
            int temp = 0;

            foreach(var ancestor in ancestors)
            {
                temp = GetOrbits(you.Parent, ancestor, youPath) + GetOrbits(san.Parent, ancestor, sanPath);

                if(totalNotSet || temp < total)
                {
                    total = temp;
                    totalNotSet = false;
                }
            }
            Console.WriteLine($"Minimum number of orbital transfers: {total}");
        }

        static void Main(string[] args)
        {
            watch.Start();
            string[] orbitStrs = File.ReadAllLines("Day6Input.txt");
            ParseOrbits(orbitStrs);

            Part1();
            Part2();

            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
