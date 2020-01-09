using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCodeCSharp
{
    class Day5
    {
        static Stopwatch watch = new Stopwatch();

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

        static void ReadInt(int[] nums, ref int idx)
        {
            int result;
            watch.Stop();
            for (; ; )
            {
                Console.Write("Please enter an integer: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out result))
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"'{input}' is not an integer");
                }
            }
            watch.Start();
            nums[nums[idx + 1]] = result;
            idx += 2;
        }

        static void Write(int[] nums, ref int idx)
        {
            Mode[] modes = GetModes(nums[idx], 1);
            int[] vals = GetVals(nums, idx + 1, modes);

            Console.WriteLine(vals[0]);
            idx += 2;
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

        public static int run(int[] nums)
        {
            bool exit = false;
            Opcode instruction;
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
                        ReadInt(nums, ref i);
                        break;

                    case Opcode.Write:
                        Write(nums, ref i);
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

            return nums[0];
        }

        static void Main(string[] args)
        {
            watch.Start();
            int[] nums;

            nums = File.ReadAllText("Day5Input.txt").Split(',')
                .Select(x => int.Parse(x)).ToArray();

            //nums = new int[] { 3, 0, 4, 0, 99 };

            //nums = new int[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 };

            //nums = new int[] { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 };

            //nums = new int[] { 3, 3, 1108, -1, 8, 3, 4, 3, 99 };

            //nums = new int[] { 3, 3, 1107, -1, 8, 3, 4, 3, 99 };

            //nums = new int[] { 3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9 };

            //nums = new int[] { 3,3,1105,-1,9,1101,0,0,12,4,12,99,1};

            //nums = new int[] {3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
            //            1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
            //999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99};

            run(nums);
            //printNums(nums);

            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
        }
    }
}