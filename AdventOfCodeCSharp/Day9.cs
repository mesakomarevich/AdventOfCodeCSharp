using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCodeCSharp
{
    class Day9
    {
        static Stopwatch watch = new Stopwatch();

        static long relativeBase = 0;

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
            RelativeBaseOffset = 9,
            Halt = 99
        }
        enum Mode
        {
            Position = 0,
            Immediate = 1,
            Relative = 2
        }

        static void printNums(long[] nums)
        {
            Console.WriteLine("\n\n");
            for (long i = 0; i < nums.Length; i++)
            {
                Console.Write($"{nums[i]} ");
                if (i > 0 && i % 4 == 0)
                {
                    Console.WriteLine("");
                }
            }
            Console.WriteLine($"val: {nums[0]}");
        }

        static long GetVal(long[] nums, long idx, Mode mode, bool writeVal = false)
        {
            long result;

            switch (mode)
            {
                case Mode.Position:
                    result = writeVal ? nums[idx] : nums[nums[idx]];
                    break;

                case Mode.Immediate:
                    result = writeVal ? throw new Exception($"Cannot write to immediate values") : nums[idx];
                    break;

                case Mode.Relative:
                    result = writeVal ? relativeBase + nums[idx] : nums[relativeBase + nums[idx]];
                    break;

                default:
                    throw new Exception($"Unkown mode: {mode}");
            }

            return result;
        }

        static long[] GetVals(long[] nums, long idx, Mode[] modes, bool writeVal = false)
        {
            long[] vals = new long[modes.Length];
            for (long i = 0; i < vals.Length; i++)
            {
                vals[i] = GetVal(nums, idx + i, modes[i], (writeVal && (i == vals.Length - 1)));
            }

            return vals;
        }

        //Get modes from the instruction
        static Mode[] GetModes(long modeDigits, long parameters)
        {
            //prune the opcode
            modeDigits /= 100;
            if (modeDigits > (Math.Pow(10, parameters + 1)))
            {
                throw new Exception($"Too many mode digits: {modeDigits}");
            }

            Mode[] modes = new Mode[parameters];

            for (long i = 0; i < modes.Length; i++)
            {
                modes[i] = (Mode)(modeDigits % 10);
                modeDigits /= 10;
            }

            return modes;
        }

        static void Add(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 3);
            long[] vals = GetVals(nums, idx + 1, modes, true);

            nums[vals[2]] = vals[0] + vals[1];
            //nums[nums[idx + 3]] = vals[0] + vals[1];
            idx += 4;
        }

        static void Multiply(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 3);
            long[] vals = GetVals(nums, idx + 1, modes, true);

            nums[vals[2]] = vals[0] * vals[1];
            //nums[nums[idx + 3]] = vals[0] * vals[1];
            idx += 4;
        }

        //input overrides the read sequence
        static void ReadInt(long[] nums, ref long idx, long? input = null)
        {
            Mode[] modes = GetModes(nums[idx], 1);
            long[] vals = GetVals(nums, idx + 1, modes, true);

            long result;
            if (input == null)
            {
                watch.Stop();
                for (; ; )
                {
                    Console.Write("Please enter an integer: ");
                    string inputStr = Console.ReadLine();

                    if (long.TryParse(inputStr, out result))
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
                result = (long)input;
            }

            nums[vals[0]] = result;
            idx += 2;
        }

        static long Write(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 1);
            long[] vals = GetVals(nums, idx + 1, modes);

            Console.WriteLine(vals[0]);
            idx += 2;

            return vals[0];
        }

        static void JumpIfTrue(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            long[] vals = GetVals(nums, idx + 1, modes);

            if (vals[0] != 0)
            {
                idx = vals[1];
            }
            else
            {
                idx += 3;
            }
        }

        static void JumpIfFalse(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 2);
            long[] vals = GetVals(nums, idx + 1, modes);

            if (vals[0] == 0)
            {
                idx = vals[1];
            }
            else
            {
                idx += 3;
            }
        }

        static void LessThan(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 3);
            long[] vals = GetVals(nums, idx + 1, modes, true);

            nums[vals[2]] = vals[0] < vals[1] ? 1 : 0;

            //if (vals[0] < vals[1])
            //{
            //    nums[nums[idx + 3]] = 1;
            //}
            //else
            //{
            //    nums[nums[idx + 3]] = 0;
            //}

            idx += 4;
        }

        static void Equal(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 3);
            long[] vals = GetVals(nums, idx + 1, modes, true);

            nums[vals[2]] = vals[0] == vals[1] ? 1 : 0;

            //if (vals[0] == vals[1])
            //{
            //    nums[nums[idx + 3]] = 1;
            //}
            //else
            //{
            //    nums[nums[idx + 3]] = 0;
            //}

            idx += 4;
        }


        static void RelativeBaseOffset(long[] nums, ref long idx)
        {
            Mode[] modes = GetModes(nums[idx], 1);
            long[] vals = GetVals(nums, idx + 1, modes);

            relativeBase += vals[0];
            idx += 2;
        }

        public static long Run(long[] nums, long[] input = null)
        {
            bool exit = false;
            Opcode instruction;
            long output = 0;

            long inputIdx = 0;

            for (long i = 0; i < nums.Length && !exit;)
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

                    case Opcode.RelativeBaseOffset:
                        RelativeBaseOffset(nums, ref i);
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

        static void Main(string[] args)
        {
            watch.Start();
            long[] nums;

            nums = File.ReadAllText("Day9Input.txt").Split(',')
                .Select(x => long.Parse(x)).ToArray();



            //nums = new long[] { 109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 };

            //nums = new long[] { 1102, 34915192, 34915192, 7, 4, 7, 99, 0 };

            //nums = new long[] { 104, 1125899906842624, 99 };

            Array.Resize(ref nums, nums.Length * 10);

            Run(nums);

            //printNums(nums);

            watch.Stop();
            Console.WriteLine($"Time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
