using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using Unity.Mathematics;
using FixedMath;

namespace FancyToolkit
{

    public static class Extensions
    {
        public static bool MoveTowards(this Transform transform, Vector3 target, float speed)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed);
            return transform.position == target;
        }

        public static bool MoveTowards(this Rigidbody2D rb2d, Vector2 target, float speed)
        {
            rb2d.position = Vector2.MoveTowards(rb2d.position, target, speed);
            return rb2d.position == target;
        }

        public static void SetAlpha(this SpriteRenderer render, float alpha)
        {
            var color = render.color;
            color.a = alpha;
            render.color = color;
        }

        public static IEnumerator SmoothMoveRoutine(this Transform transform, Vector3 endPosition, float time)
        {
            float elapsedTime = 0;
            Vector3 startingPos = transform.position;
            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(startingPos, endPosition, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = endPosition;
        }

        public static Vector3 RandomAround(this Vector3 vector, float radiusX, float radiusY = -1f, float radiusZ = 0f)
        {
            if (radiusY < 0) radiusY = radiusX;
            return vector + new Vector3(
                UnityEngine.Random.Range(-radiusX, +radiusX),
                UnityEngine.Random.Range(-radiusY, +radiusY),
                UnityEngine.Random.Range(-radiusZ, +radiusZ)
            );
        }

        public static Vector2 RandomAround(this Vector2 vector, float radiusX, float radiusY = -1f)
        {
            if (radiusY < 0) radiusY = radiusX;
            return vector + new Vector2(
                UnityEngine.Random.Range(-radiusX, +radiusX),
                UnityEngine.Random.Range(-radiusY, +radiusY)
            );
        }

        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2
            {
                x = Mathf.Round(vector.x),
                y = Mathf.Round(vector.y),
            };
        }

        public static Vector2Int ToIntVector2(this Vector2 vector)
        {
            return new Vector2Int(
                Mathf.RoundToInt(vector.x),
                Mathf.RoundToInt(vector.y)
            );
        }

        public static Vector2Int ToIntVector2(this Vector3 vector)
        {
            return new Vector2Int(
                Mathf.RoundToInt(vector.x),
                Mathf.RoundToInt(vector.y)
            );
        }

        public static Vector2 ToFloatVector(this Vector2Int vector)
        {
            return new Vector2Int(vector.x, vector.y);
        }

        public static int2 ToInt2(this Vector3 vector)
        {
            return new int2(
                Mathf.RoundToInt(vector.x),
                Mathf.RoundToInt(vector.y)
            );
        }

        public static int2 ToInt2(this Vector2 vector)
        {
            return new int2(
                Mathf.RoundToInt(vector.x),
                Mathf.RoundToInt(vector.y)
            );
        }

        public static Vector2 ToVector2(this int2 i2)
        {
            return new Vector2(i2.x, i2.y);
        }

        public static TReturn GetMin<TReturn>(this IEnumerable<TReturn> list, Func<TReturn, float> valueGetter, float limit = float.MaxValue)
        {
            float minValue = limit;
            TReturn result = default;

            foreach (var item in list)
            {
                float value = valueGetter(item);
                if (value < minValue)
                {
                    minValue = value;
                    result = item;
                }
            }

            return result;
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }

        public static TValue Rand<TValue>(this IEnumerable<TValue> list)
        {
            if (list.Count() == 0) return default;

            return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
        }

        public static void Shuffle<T>(this List<T> list)
        {
            // Use the Fisher-Yates shuffle algorithm to shuffle the list.
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);

                // Swap the element at i with the element at j.
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        public static List<T> Shuffled<T>(this List<T> list)
        {
            var list2 = list.ToList();
            list2.Shuffle();
            return list2;
        }

        public static List<TValue> RandN<TValue>(this IEnumerable<TValue> list, int n)
        {
            n = Mathf.Min(list.Count(), n);

            return list
                .OrderBy(x => UnityEngine.Random.Range(0, 1f))
                .Take(n)
                .ToList();
        }

        public static T GetClosest<T>(this IEnumerable<T> list, Vector2 center, Func<T, Vector2> func, float maxDistance = float.MaxValue)
        {
            T closestItem = default(T);
            float closestDistance = maxDistance;

            foreach (var item in list)
            {
                Vector2 itemPosition = func(item);
                float distanceToCenter = Vector2.Distance(itemPosition, center);

                if (distanceToCenter < closestDistance)
                {
                    closestDistance = distanceToCenter;
                    closestItem = item;
                }
            }

            return closestItem;
        }

        public static bool TryStringAt(this IEnumerable<string> list, int idx, out string value)
        {
            if (list.Count() > idx)
            {
                value = list.ElementAt(idx);
                return true;
            }

            value = null;
            return false;
        }


        public static float FloatAt(this IEnumerable<string> list, int idx)
        {
            if (list.TryStringAt(idx, out var s))
            {
                return float.Parse(s, CultureInfo.InvariantCulture);
            }

            return 0f;
        }

        public static int IntAt(this IEnumerable<string> list, int idx)
        {
            if (list.TryStringAt(idx, out var s))
            {
                return int.Parse(s, CultureInfo.InvariantCulture);
            }

            return 0;
        }

        public static string StringAt(this IEnumerable<string> list, int idx)
        {
            if (list.TryStringAt(idx, out var s))
            {
                return s;
            }

            return null;
        }

        public static float GetFloat(this string s)
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }

        public static int GetInt(this string s)
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }

        public static void AnimateTowards(this Slider slider, int value, float speed)
        {
            if (slider.value == value) return;
            slider.value = Mathf.MoveTowards(slider.value, value, Time.deltaTime * speed);
        }

        public static string SignedStr(this int value)
        {
            if (value > 0)
            {
                return "+" + value.ToString();
            }
            else if (value < 0)
            {
                return value.ToString();
            }
            else
            {
                return "0";
            }
        }

        public static string WithOrdinal(this int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}