using System;
using NLua.Exceptions;
using UnityEngine;

namespace UStacker.Common
{
    public class Random
    {
        public ulong State { get; set; } = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

        private void MoveState()
        {
            State = State * 69069 + 1;
        }

        public Random(ulong? startState = null)
        {
            if (startState is { } value)
            {
                State = value;
            }
        }

        public Random(int startState)
        {
            State = (ulong)startState;
        }

        public double Next()
        {
            MoveState();
            return State / (double)ulong.MaxValue;
        }

        public double Next(double range, double offset = 0d)
        {
            return Next() * range + offset;
        }

        public int NextInt()
        {
            MoveState();
            return (int)((uint)(State % uint.MaxValue) - int.MaxValue);
        }

        public int NextInt(int range, int offset = 0)
        {
            if (Mathf.Abs(range) <= 1) return offset;
            return (int)Next(range, offset);
        }

        public double NextLua(int? start = null, int? end = null)
        {
            return start switch
            {
                null when end is null => Next(),
                { } rangeVal when end is null => rangeVal switch
                {
                    1 => 1,
                    _ => NextInt(rangeVal, 1)
                },
                { } startVal when end is { } endVal => NextInt(endVal - startVal + 1, startVal),
                _ => throw new LuaException($"Bad parameters to random method: {null}, {end}")
            };
        }
    }
}