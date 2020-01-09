using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace AdventOfCodeCSharp
{
    class Day10
    {
        static Stopwatch watch = new Stopwatch();

        public static IEnumerable<int> RandInt()
        {
            Random rnd = new Random();

            for (; ; )
            {
                yield return rnd.Next(0, 10000);
            }
        }


        public static void LazyEnumerate(int times)
        {
            watch.Start();
            int total = 0;
            int idx = 0;
            IEnumerable<int> nums;

            for (int i = 0; i < times; i++)
            {
                nums = RandInt().Take(1000);

                foreach (var num in nums)
                {
                    idx++;
                    if (num > 9990)
                    {
                        break;
                    }
                }
                total += idx;
                idx = 0;
            }

            watch.Stop();
            Console.WriteLine($"Lazy:\n" +
                $"Total enumerations: {total}\n" +
                $"Average enumerations: {total / times}\n" +
                $"Enumerations per ms: {total / watch.ElapsedMilliseconds}\n" +
                $"Time: {watch.ElapsedMilliseconds}ms\n");
        }

        public static void EagerEnumerate(int times)
        {
            watch.Start();
            int total = 0;
            int idx = 0;
            //List<int> nums;
            int[] nums;

            for (int i = 0; i < times; i++)
            {
                nums = RandInt().Take(1000).ToArray();

                foreach (var num in nums)
                {
                    idx++;
                    if (num > 9990)
                    {
                        break;
                    }
                }
                total += idx;
                idx = 0;
            }

            watch.Stop();
            Console.WriteLine($"Eager:\n" +
                $"Total enumerations: {total}\n" +
                $"Average enumerations: {total / times}\n" +
                $"Enumerations per ms: {total / watch.ElapsedMilliseconds}\n" +
                $"Time: {watch.ElapsedMilliseconds}ms\n");
        }

        public static bool TryCast<T>(object obj, out T result)
        {
            if (obj is T)
            {
                result = (T)obj;
                return true;
            }

            result = default(T);
            return false;
        }

        static Task<string> cont()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(1000);
                return $"Task 1 time {watch.ElapsedMilliseconds}";
            })
            .ContinueWith(a =>
            {
                Thread.Sleep(1000);
                return a.Result + $"\nTask 2 time {watch.ElapsedMilliseconds}";
            })
            .ContinueWith(a =>
            {
                Thread.Sleep(1000);
                return a.Result + $"\nTask 3 time {watch.ElapsedMilliseconds}";
            });
        }

        static async Task Main(string[] args)
        {

            Action<object> cw = x => Console.WriteLine(x); //shorten things a bit
            //int i = 1;
            //object o = i;
            //cw(o.GetType());
            //cw(o);
            ////var n = Convert.ChangeType(o, o.GetType());

            //int n;

            //if (TryCast(o, out n))
            //    Console.WriteLine(n);


            //cw(n+100);

            //int times = 10000;
            //watch.Restart();
            //LazyEnumerate(times);
            //watch.Restart();
            //EagerEnumerate(times);

            watch.Start();
            var t = cont();

            Thread.Sleep(3500);
            cw(await t);
            watch.Stop();
        }
    }
}
