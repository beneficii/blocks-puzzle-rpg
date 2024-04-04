using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FancyToolkit
{
    public static class Easings
    {
        public delegate float easingDelegate(float t);

        public static float Linear(float t) => t;

        public static float OutCubic(float t) => InCubic(t - 1) + 1;
        public static float InCubic(float t) => t * t * t;


        public static float InBack(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }

        public static float OutBack(float t) => 1 - InBack(1 - t);
        public static float InOutBack(float t)
        {
            if (t < 0.5) return InBack(t * 2) / 2;
            return 1 - InBack((1 - t) * 2) / 2;
        }
    }
}