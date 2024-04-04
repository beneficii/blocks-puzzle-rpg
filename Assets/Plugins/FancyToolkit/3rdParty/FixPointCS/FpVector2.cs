using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
using Unity.Mathematics;
#endif

namespace FixedMath
{
    public struct FpVector2
    {
        public static FpVector2 ConstFarAway = new FpVector2(fixed64.FromLong(long.MinValue), fixed64.FromLong(long.MinValue));

        public fixed64 x;
        public fixed64 y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FpVector2(fixed64 x, fixed64 y) { this.x = x; this.y = y; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fixed64 Distance(FpVector2 a, FpVector2 b)
        {
            var diff_x = a.x - b.x;
            var diff_y = a.y - b.y;
            return (diff_x * diff_x + diff_y * diff_y).Sqrt();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fixed64 SqrMagnitude(FpVector2 a) { return a.x * a.x + a.y * a.y; }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FpVector2 MoveTowards(FpVector2 current, FpVector2 target, fixed64 maxDistanceDelta)
        {
            // avoid vector ops because current scripting backends are terrible at inlining
            var toVector_x = target.x - current.x;
            var toVector_y = target.y - current.y;

            var sqDist = toVector_x * toVector_x + toVector_y * toVector_y;

            if (sqDist == fixed64.Zero || (maxDistanceDelta >= fixed64.Zero && sqDist <= maxDistanceDelta * maxDistanceDelta))
                return target;

            var rdist = sqDist.RSqrt();     // 1/distance

            return new FpVector2(current.x + toVector_x * rdist * maxDistanceDelta,
                current.y + toVector_y * rdist * maxDistanceDelta);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is FpVector2 vector &&
                   EqualityComparer<fixed64>.Default.Equals(x, vector.x) &&
                   EqualityComparer<fixed64>.Default.Equals(y, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FpVector2 a, FpVector2 b) => a.x == b.x && a.y == b.y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FpVector2 a, FpVector2 b) => a.x != b.x && a.y != b.y;

#if UNITY_5_3_OR_NEWER
        public static explicit operator Vector2(FpVector2 fp) => new Vector2((float)fp.x, (float)fp.y);
        public static explicit operator Vector3(FpVector2 fp) => new Vector3((float)fp.x, (float)fp.y, 0);
        public static explicit operator int2(FpVector2 fp) => new int2((int)fp.x, (int)fp.y);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out int x, out int y)
        {
            x = (int)this.x;
            y = (int)this.y;
        }

        public void To2Fixed(out fixed64 x, out fixed64 y)
        {
            x = this.x;
            y = this.y;
        }
    }
}
