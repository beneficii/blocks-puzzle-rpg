using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FixPointCS;

namespace FixedMath
{
    public struct fixed64 : IComparable<fixed64>
    {
        public static readonly fixed64 Zero = new fixed64(0);
        public static readonly fixed64 One = FromInt(1);
        public static readonly fixed64 NegativeOne = FromInt(-1);
        public static readonly fixed64 MaxValue = new fixed64(long.MaxValue);
        public static readonly fixed64 MinValue = new fixed64(long.MinValue);
        
        static long[] tenTable = { 1L, 10L, 100L, 1000L, 10000L, 100000L, 1000000L, 10000000L, 100000000L, 1000000000L, 10000000000L, 100000000000L, 1000000000000L };

        long rawValue;

        [MethodImpl(FixedUtil.AggressiveInlining)]
        private fixed64(long value)
        {
            this.rawValue = value;
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 FromInt(int value) => new fixed64(Fixed64.FromInt(value));
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 FromLong(long value) => new fixed64(value);
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 FromFloat(float value) => new fixed64(Fixed64.FromFloat(value));

        public static fixed64 Parse(string value)
        {
            var tokens = value.Split('.');
            if (tokens.Length == 1) return FromInt(int.Parse(value));

            if(tokens.Length == 2)
            {
                string tkn2 = tokens[1];
                return new fixed64(Fixed64.FromInt(int.Parse(tkn2)) / tenTable[tkn2.Length]);
            }

            return new fixed64();
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static explicit operator int(fixed64 fp) => Fixed64.FloorToInt(fp.rawValue);
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static explicit operator float(fixed64 fp) => Fixed64.ToFloat(fp.rawValue);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static explicit operator fixed64(int value) => FromInt(value);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator +(fixed64 a) => a;
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator -(fixed64 a) => new fixed64(-a.rawValue);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator +(fixed64 a, fixed64 b)
            => new fixed64(Fixed64.Add(a.rawValue, b.rawValue));

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator -(fixed64 a, fixed64 b)
            => new fixed64(Fixed64.Sub(a.rawValue, b.rawValue));

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator +(fixed64 a, int b)
            => new fixed64(Fixed64.Add(a.rawValue, Fixed64.FromInt(b)));

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator -(fixed64 a, int b)
            => new fixed64(Fixed64.Sub(a.rawValue, Fixed64.FromInt(b)));

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator *(fixed64 a, fixed64 b)
            => new fixed64(Fixed64.Mul(a.rawValue, b.rawValue));

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator *(fixed64 a, int b)
            => new fixed64(a.rawValue * b);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator *(int b, fixed64 a)
            => new fixed64(a.rawValue * b);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator /(fixed64 a, int b)
            => new fixed64(a.rawValue / b);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static fixed64 operator /(fixed64 a, fixed64 b)
            => new fixed64(Fixed64.DivFastest(a.rawValue, b.rawValue));

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator >(fixed64 a, fixed64 b) => a.rawValue > b.rawValue;
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator <(fixed64 a, fixed64 b) => a.rawValue < b.rawValue;
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator >=(fixed64 a, fixed64 b) => a.rawValue >= b.rawValue;
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator <=(fixed64 a, fixed64 b) => a.rawValue <= b.rawValue;

        // int comparison
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator >(fixed64 a, int b) => a.rawValue > Fixed64.FromInt(b);
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator <(fixed64 a, int b) => a.rawValue < Fixed64.FromInt(b);
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator >=(fixed64 a, int b) => a.rawValue >= Fixed64.FromInt(b);
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator <=(fixed64 a, int b) => a.rawValue <= Fixed64.FromInt(b);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator ==(fixed64 a, fixed64 b) => a.rawValue == b.rawValue;
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator !=(fixed64 a, fixed64 b) => a.rawValue != b.rawValue;

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator ==(fixed64 a, int b) => a.rawValue == (int)b;
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static bool operator !=(fixed64 a, int b) => a.rawValue != (int)b;

        public override bool Equals(object obj) => obj is fixed64 fp && fp.rawValue == rawValue;

        public override int GetHashCode() => rawValue.GetHashCode();

        public override string ToString() => $"{(float)this}";

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public fixed64 Sqrt() => new fixed64(Fixed64.SqrtFastest(rawValue));
        [MethodImpl(FixedUtil.AggressiveInlining)]
        public fixed64 RSqrt() => new fixed64(Fixed64.RSqrtFastest(rawValue));


        [MethodImpl(FixedUtil.AggressiveInlining)]  // fraction
        public static fixed64 Fract(int a, int b) => new fixed64(Fixed64.FromInt(a) / b);

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public int CompareTo(fixed64 other) => rawValue.CompareTo(other.rawValue);
    }
}