using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using FixedMath;
using NUnit.Framework;

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
        public string NextString() => tokens.Dequeue()??"";
        public float NextFloat() => float.Parse(NextString());
        public double NextDouble() => double.Parse(NextString());
        public bool NextBool() => bool.Parse(NextString());
        public fixed64 NextFixed() => fixed64.Parse(NextString());

        public TEnum NextEnum<TEnum>() where TEnum : struct, Enum 
        {
            Enum.TryParse(NextString(), true, out TEnum result);

            return result;
        }

        public bool TryGetGeneric<TValue>(out TValue val)
        {
            val = default;

            if (Empty)
            {
                return false;
            }

            // Handle specific cases for known types
            if (typeof(TValue) == typeof(int))
            {
                if (TryGet(out int intResult))
                {
                    val = (TValue)(object)intResult;
                    return true;
                }
                return false;
            }

            if (typeof(TValue) == typeof(float))
            {
                if (TryGet(out float floatResult))
                {
                    val = (TValue)(object)floatResult;
                    return true;
                }
                return false;
            }

            if (typeof(TValue) == typeof(double))
            {
                if (TryGet(out double doubleResult))
                {
                    val = (TValue)(object)doubleResult;
                    return true;
                }
                return false;
            }

            if (typeof(TValue) == typeof(bool))
            {
                if (TryGet(out bool boolResult))
                {
                    val = (TValue)(object)boolResult;
                    return true;
                }
                return false;
            }

            if (typeof(TValue) == typeof(string))
            {
                val = (TValue)(object)NextString();
                return true;
            }

            // Handle enums
            if (typeof(TValue).IsEnum)
            {
                try
                {
                    val = (TValue)Enum.Parse(typeof(TValue), NextString(), true);
                    return true;
                }
                catch
                {
                    val = default;
                    return false;
                }
            }

            // Handle List<T>
            if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(List<>))
            {
                Type itemType = typeof(TValue).GetGenericArguments()[0]; // Get the type of T in List<T>
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType)); // Create instance of List<T>

                foreach (var token in NextString().Split(';'))
                {
                    object convertedItem = Convert.ChangeType(token, itemType);
                    list.Add(convertedItem);
                }

                val = (TValue)list;
                return true;
            }

            // Handle FactoryBuilder<TClass> for classes
            if (typeof(TValue).IsClass)
            {
                if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(FactoryBuilder<>))
                {
                    Type classType = typeof(TValue).GetGenericArguments()[0];
                    Type factoryType = typeof(Factory<>).MakeGenericType(classType);

                    var factoryMethod = factoryType.GetMethod("CreateBuilder", new[] { typeof(StringScanner) });
                    if (factoryMethod != null)
                    {
                        val = (TValue)factoryMethod.Invoke(null, new object[] { this });
                        return true;
                    }
                }
            }

            // If no type match found, return false with an error
            UnityEngine.Debug.LogError($"Unrecognized type '{typeof(TValue)}' in TryGetGeneric");
            return false;
        }

        /*
        public bool TryGetGeneric<TClass>(out FactoryBuilder<TClass> val) where TClass : class
        {
            if (Empty)
            {
                val = default;
                return false;
            }

            val = Factory<TClass>.CreateBuilder(NextString());

            return false;
        }


        public bool TryGetGeneric(out int val)
        {
            if (Empty)
            {
                val = default;
                return false;
            }
            return TryGet(out val);
        }

        public bool TryGetGeneric(out float val)
        {
            if (Empty)
            {
                val = default;
                return false;
            }
            return TryGet(out val);
        }

        public bool TryGetGeneric(out double val)
        {
            if (Empty)
            {
                val = default;
                return false;
            }
            return TryGet(out val);
        }

        // Specialized TryGetGeneric overload for bool
        public bool TryGetGeneric(out bool val)
        {
            if (Empty)
            {
                val = default;
                return false;
            }
            return TryGet(out val);
        }

        // Specialized TryGetGeneric overload for string
        public bool TryGetGeneric(out string val)
        {
            if (Empty)
            {
                val = default;
                return false;
            }
            return TryGet(out val);
        }

        public bool TryGetGeneric<T>(out List<T> val)
        {
            if (Empty)
            {
                val = default;
                return false;
            }
            val = NextListCustom(x => (T)Convert.ChangeType(x, typeof(T)));
            return val.Count > 0;
        }

        public bool TryGetGeneric<TValue>(out TValue val)
        {
            val = default;
            if (Empty) return false;

            string nextToken = NextString();

            // Check if TValue is an enum
            if (typeof(TValue).IsEnum)
            {
                val = (TValue)Enum.Parse(typeof(TValue), nextToken, true);
                return true;
            }

            UnityEngine.Debug.LogError($"Unrecognized type '{typeof(TValue)}' in TryGetGeneric");
            return false;
        }*/

        public bool TryGet<TEnum>(out TEnum val, TEnum defaultValue = default) where TEnum : struct, System.Enum
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
            return string.Join(" ", tokens);
        }
    }
}