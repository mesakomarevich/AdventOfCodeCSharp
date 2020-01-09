using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AdventOfCodeCSharp
{
    class Day7Part2
    {
        static Stopwatch watch = new Stopwatch();

        class IntcodeLock
        {
            public AutoResetEvent are { get; set; }
            public int val { get; set; }

            public IntcodeLock()
            {
                are = new AutoResetEvent(false);
                val = 0;
            }

            public void Reset()
            {
                are.Reset();
                val = 0;
            }
        }

        static IntcodeLock ab = new IntcodeLock();
        static IntcodeLock bc = new IntcodeLock();
        static IntcodeLock cd = new IntcodeLock();
        static IntcodeLock de = new IntcodeLock();
        static IntcodeLock ea = new IntcodeLock();


        enum Opcode
        {
            Add = 1,
            Multiply = 2,
            ReadInt = 3,
            Write = 4,
            JumpIfTrue = 5,
            JumpIfFalse = 6,
            LessThan = 7,
            Equal = 8,
            Halt = 99
        }
        enum Mode
        {
            Position = 0,
            Immediate = 1
        }

        class IntcodeThreadData
        {
            public string name { get; set; }

            public IntcodeLock wait { get; set; }
            public IntcodeLock signal { get; set; }

            public int[] nums { get; set; }
            public int[] input { get; set; }

            public IntcodeThreadData()
            {

            }
        }

        static void printNums(int[] nums)
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

        static int GetVal(int[] nums, int idx, Mode mode)
        {
            int result;

            switch (mode)
            {
                case Mode.Position:
                    result = nums[nums[idx]];
                    break;

                case Mode.Immediate:
                    result = nums[idx];
                    break;

                default:
                    throw new Exception($"Unkown mode: {mode}");
            }

            return result;
        }

        static int[] GetVals(int[] nums, int idx, Mode[] modes)
        {
            int[] vals = new int[modes.Length];
            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] = GetVal(nums, idx + i, modes[i]);
            }

            return vals;
        }

        //Get modes from the instruction
        static Mode[] GetModes(int modeDigits, int parameters)
        {
            //prune the opcode
            modeDigits /= 100;
            if (modeDigits > (Math.Pow(10, parameters + 1)))
            {
                throw new Exception($"Too many mode digits: {modeDigits}");
            }

            Mode[] modes = new Mode[parameters];

            for (int i = 0; i < modes.Length; i++)
            {
                modes[i] = (Mode)(modeDigits % 10);
                modeDigits /= 10;
            }

            return modes;
        }

        static void Add(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            int[] vals = GetVals(nums, idx + 1, modes);

            nums[nums[idx + 3]] = vals[0] + vals[1];
            idx += 4;
        }

        static void Multiply(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            int[] vals = GetVals(nums, idx + 1, modes);

            nums[nums[idx + 3]] = vals[0] * vals[1];
            idx += 4;
        }

        //input overrides the read sequence
        static void ReadInt(int[] nums, ref int idx, int? input = null)
        {
            int result;
            if (input == null)
            {
                watch.Stop();
                for (; ; )
                {
                    Console.Write("Please enter an integer: ");
                    string inputStr = Console.ReadLine();

                    if (int.TryParse(inputStr, out result))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"'{inputStr}' is not an integer");
                    }
                }
                watch.Start();
            }
            else
            {
                result = (int)input;
            }

            nums[nums[idx + 1]] = result;
            idx += 2;
        }

        static int Write(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 1);
            int[] vals = GetVals(nums, idx + 1, modes);

            Console.WriteLine(vals[0]);
            idx += 2;

            return vals[0];
        }

        static void JumpIfTrue(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            int[] vals = GetVals(nums, idx + 1, modes);

            if (vals[0] != 0)
            {
                idx = vals[1];
            }
            else
            {
                idx += 3;
            }
        }

        static void JumpIfFalse(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            int[] vals = GetVals(nums, idx + 1, modes);

            if (vals[0] == 0)
            {
                idx = vals[1];
            }
            else
            {
                idx += 3;
            }
        }

        static void LessThan(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            int[] vals = GetVals(nums, idx + 1, modes);

            if (vals[0] < vals[1])
            {
                nums[nums[idx + 3]] = 1;
            }
            else
            {
                nums[nums[idx + 3]] = 0;
            }

            idx += 4;
        }

        static void Equal(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            int[] vals = GetVals(nums, idx + 1, modes);

            if (vals[0] == vals[1])
            {
                nums[nums[idx + 3]] = 1;
            }
            else
            {
                nums[nums[idx + 3]] = 0;
            }

            idx += 4;
        }

        public static int Run(Object obj)
        {
            IntcodeThreadData data = (IntcodeThreadData)obj;
            int[] nums = data.nums;
            int[] input = data.input;


            bool exit = false;
            Opcode instruction;
            int output = 0;

            int inputIdx = 0;

            for (int i = 0; i < nums.Length && !exit;)
            {
                //get opcode
                instruction = (Opcode)(nums[i] % 100);

                switch (instruction)
                {
                    case Opcode.Add:
                        Add(nums, ref i);
                        break;

                    case Opcode.Multiply:
                        Multiply(nums, ref i);
                        break;

                    case Opcode.ReadInt:
                        //this should basically only ever be used for the initial input
                        if (input != null && inputIdx < input.Length)
                        {
                            ReadInt(nums, ref i, input[inputIdx]);
                            inputIdx++;
                        }
                        else
                        {
                            //wait until the other thread has written the data
                            //that we want to read
                            data.wait.are.WaitOne();
                            //paas in thread value set by the other thread
                            ReadInt(nums, ref i, data.wait.val);
                        }
                        break;

                    case Opcode.Write:
                        output = Write(nums, ref i);
                        //set the output val for the next thread to read
                        data.signal.val = output;
                        //signal that thread
                        data.signal.are.Set();
                        break;

                    case Opcode.JumpIfTrue:
                        JumpIfTrue(nums, ref i);
                        break;

                    case Opcode.JumpIfFalse:
                        JumpIfFalse(nums, ref i);
                        break;

                    case Opcode.LessThan:
                        LessThan(nums, ref i);
                        break;

                    case Opcode.Equal:
                        Equal(nums, ref i);
                        break;

                    case Opcode.Halt:
                        exit = true;
                        break;

                    default:
                        exit = true;
                        Console.WriteLine("You done fucked up");
                        break;
                }
            }

            return output;
        }

        static void TMain(Object obj)
        {
            Run(obj);
        }

        private static IEnumerable<T[]> GetPermutations<T>(T[] values)
        {
            if (values.Length == 1)
                return new[] { values };

            return values.SelectMany(v => GetPermutations(values.Except(new[] { v }).ToArray()),
                (v, p) => new[] { v }.Concat(p).ToArray());
        }

        static void Main(string[] args)
        {
            watch.Start();

            int[] nums;
            int[] temp;
            int best = 0;
            int[] input;
            Thread thread;
            IntcodeThreadData data;
            List<int[]> permuataions;

            nums = File.ReadAllText("Day7Input.txt").Split(',')
                .Select(x => int.Parse(x)).ToArray();

            permuataions = GetPermutations(new int[] { 5, 6, 7, 8, 9 }).ToList();

            List<Thread> threads = new List<Thread>();
            List<IntcodeThreadData> datas = new List<IntcodeThreadData>();
            IntcodeLock[,] locks = new IntcodeLock[,] { { ea, ab }, { ab, bc }, { bc, cd }, { cd, de }, { de, ea } };

            foreach (var phases in permuataions)
            {
                threads = new List<Thread>();
                ab.Reset();
                bc.Reset();
                cd.Reset();
                de.Reset();
                ea.Reset();

                ab.are.Reset();
                
                for (int i = 0; i < phases.Length; i++)
                {
                    //first thread must get 0 as it's inital value
                    if (i == 0)
                    {
                        input = new int[] { phases[i], 0 };
                    }
                    else
                    {
                        input = new int[] { phases[i] };
                    }
                    thread = new Thread(TMain);

                    //copy program
                    temp = new int[nums.Length];
                    nums.CopyTo(temp, 0);

                    //initialize thread data
                    data = new IntcodeThreadData()
                    {
                        name = i.ToString(),
                        wait = locks[i, 0],
                        signal = locks[i, 1],
                        nums = temp,
                        input = input
                    };

                    threads.Add(thread);
                    //start running the thread
                    thread.Start(data);
                }

                //wait on all threads to finish
                foreach (var t in threads)
                {
                    t.Join();
                }


                if (best < ea.val)
                {
                    best = ea.val;
                }
            }

            Console.WriteLine($"Highest signal: {best}");

            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
