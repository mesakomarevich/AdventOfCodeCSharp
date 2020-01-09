using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;

namespace AdventOfCodeCSharp
{
    class Day7Part1
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

        public static int Run(int[] nums, int[] input = null)
        {
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
                        if (input != null && inputIdx < input.Length)
                        {
                            ReadInt(nums, ref i, input[inputIdx]);
                            inputIdx++;
                        }
                        else
                        {
                            ReadInt(nums, ref i);
                        }
                        break;

                    case Opcode.Write:
                        output = Write(nums, ref i);
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

            nums = File.ReadAllText("Day7Input.txt").Split(',')
                .Select(x => int.Parse(x)).ToArray();

            int output = 0;
            int best = 0;

            var permuataions = GetPermutations(new int[] { 0, 1, 2, 3, 4 }).ToList();


            foreach(var phases in permuataions)
            {
                output = 0;
                for (int i = 0; i < phases.Length; i++)
                {
                    int[] input = new int[] { phases[i], output };
                    output = Run(nums, input);
                }

                if (best < output)
                {
                    best = output;
                }
            }

            Console.WriteLine($"Test output: {best}");

            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
