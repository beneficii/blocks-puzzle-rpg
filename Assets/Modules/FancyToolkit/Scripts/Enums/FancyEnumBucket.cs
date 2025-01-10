using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace FancyToolkit
{
    public class FancyEnumBucket<TEnum> where TEnum : struct
    {
        readonly Dictionary<TEnum, int> bucket;
        readonly TEnum defaultType;

        public FancyEnumBucket(Dictionary<TEnum, int> weights, TEnum defaultType, int totalSpots)
        {
            this.defaultType = defaultType;
            bucket = new();

            int totalWeight = weights.Sum(x => x.Value);
            int totalInBucket = 0;
            foreach (var item in weights)
            {
                var cnt = item.Value * totalSpots / totalWeight;
                totalInBucket += cnt;
                bucket[item.Key] = cnt;
            }

            if (!bucket.ContainsKey(defaultType)) bucket.Add(defaultType, 0);

            if (totalInBucket < totalSpots)
            {
                bucket[defaultType] += totalSpots - totalInBucket;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Bucket");
            foreach (var item in bucket)
            {
                sb.AppendLine($"{item.Key}: {item.Value}");
            }
            Debug.Log(sb.ToString());
        }

        public void RemoveOne(TEnum key)
        {
            if (!bucket.TryGetValue(key, out var val)) return;
            if (val < 1) return;

            bucket[key] = val - 1;
        }

        public TEnum PickOne(List<TEnum> filter, System.Random rng)
        {
            var bounds = new List<int>();
            int totalWeight = 0;
            foreach (var key in filter)
            {
                if (!bucket.TryGetValue(key, out var val)) val = 0;

                int upper = totalWeight + val;
                bounds.Add(upper);
                totalWeight = upper;
            }

            // if nothing matches, just return default
            if (totalWeight < 1)
            {
                if (bucket[defaultType] > 0) bucket[defaultType]--;
                return defaultType;
            }

            int roll = rng.Next(totalWeight);
            for (int i = 0; i < bounds.Count; i++)
            {
                if (roll < bounds[i])
                {
                    var rKey = filter[i];
                    bucket[rKey]--;
                    return rKey;
                }
            }

            Assert.IsFalse(true, "Somehow overrolled bucket!");
            return defaultType;
        }
    }
}