using System;
using System.Collections.Generic;
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

        public int NextInt() => int.Parse(NextString());
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

        
    }
}