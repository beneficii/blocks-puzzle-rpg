using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FixedMath;

namespace FancyToolkit
{
    public class StringScanner
    {
        private Queue<string> tokens;

        public int Count => tokens.Count;
        public bool Empty => Count == 0;

        public StringScanner(string line)
        {
            var array = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            tokens = new Queue<string>(array);
        }

        public void AddLine(string line)
        {
            var array = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array)
            {
                tokens.Enqueue(item);
            }
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            while (tokens.Count > 0)
            {
                sb.Append(tokens.Dequeue());
                if (tokens.Count > 0) sb.Append(' ');
            }
            return sb.ToString();
        }

        bool TryParse(string input, out int val)
        {
            var parts = input.Split("..");

            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
            {
                val = UnityEngine.Random.Range(x, y);
                return true;
            }
            return int.TryParse(input, out val);
        }

        public int NextInt() => TryParse(NextString(), out var val) ? val : 0;
        public string NextString() => tokens.Dequeue();
        public float NextFloat() => float.Parse(NextString());
        public double NextDouble() => double.Parse(NextString());
        public bool NextBool() => bool.Parse(NextString());
        public fixed64 NextFixed() => fixed64.Parse(NextString());

        public TEnum NextEnum<TEnum>() where TEnum : struct
        {
            Enum.TryParse(NextString(), true, out TEnum result);

            return result;
        }

        public bool TryGetGeneric<TValue>(out TValue val)
        {
            val = default;

            switch (val)
            {
                case int _ when TryGet(out int intValue):
                    val = (TValue)(object)intValue;
                    return true;

                case float _ when TryGet(out float floatValue):
                    val = (TValue)(object)floatValue;
                    return true;

                case double _ when TryGet(out double doubleValue):
                    val = (TValue)(object)doubleValue;
                    return true;

                case bool _ when TryGet(out bool boolValue):
                    val = (TValue)(object)boolValue;
                    return true;

                case string _ when TryGet(out string stringValue):
                    val = (TValue)(object)stringValue;
                    return true;
            }

            return false;
        }

        public bool TryGet<TEnum>(out TEnum val, TEnum defaultValue = default) where TEnum : struct
        {
            val = defaultValue;
            if (Empty) return false;
            return Enum.TryParse(NextString(), true, out val);
        }

        public bool TryGet(out int val, int defaultValue = default)
        {
            val = defaultValue;
            if (Empty) return false;
            return TryParse(NextString(), out val);
        }

        public bool TryGet(out string val, string defaultValue = default)
        {
            val = defaultValue;
            if (Empty) return false;
            val = NextString();
            return !string.IsNullOrWhiteSpace(val);
        }

        public bool TryGet(out float val, float defaultValue = default)
        {
            val = defaultValue;
            if (Empty) return false;
            return float.TryParse(NextString(), NumberStyles.Float, CultureInfo.InvariantCulture, out val);
        }

        public bool TryGet(out double val, double defaultValue = default)
        {
            val = defaultValue;
            if (Empty) return false;
            return double.TryParse(NextString(), out val);
        }

        public bool TryGet(out bool val, bool defaultValue = default)
        {
            val = defaultValue;
            if (Empty) return false;
            return bool.TryParse(NextString(), out val);
        }

        public List<T> NextListCustom<T>(Func<string, T> converter, char separator = ';')
        {
            var result = new List<T>();

            foreach (var item in NextString().Split(separator))
            {
                result.Add(converter(item));
            }

            return result;
        }

        public List<int> NextListInt(char separator = ';') => NextListCustom(int.Parse, separator);

        public List<float> NextListFloat(char separator = ';') => NextListCustom(float.Parse, separator);

        public List<string> NextListString(char separator = ';') => NextListCustom(x=>x, separator);

        public List<fixed64> NextListFixed(char separator = ';') => NextListCustom(fixed64.Parse, separator);

        public List<TEnum> NextListEnum<TEnum>(char separator = ';') where TEnum : struct
            => NextListCustom(EnumUtil.Parse<TEnum>, separator);

        public string Current()
        {
            return String.Join(" ", tokens);
        }
    }
}