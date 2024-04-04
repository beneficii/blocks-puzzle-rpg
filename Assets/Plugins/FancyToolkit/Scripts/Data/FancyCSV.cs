using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Nortal.Utilities.Csv;
using System.Globalization;
using FixedMath;

namespace FancyToolkit
{

    public static class FancyCSV
    {
        public static List<T> FromText<T>(string text) where T : new()
        {
            return new FancySheet(text).Convert<T>();
        }

        public static List<T> FromFile<T>(string filePath) where T : new()
        {
            FancySheet result = null;
            using (StreamReader sr = new StreamReader(filePath))
            {
                result = new FancySheet(sr);
            }

            return result?.Convert<T>();
        }

        public class FancySheet
        {
            string[] keys;
            string[][] values;

            CsvSettings Settings => new CsvSettings() { FieldDelimiter = '|' };

            public FancySheet(StreamReader streamReader)
            {
                using (var parser = new CsvParser(streamReader, Settings))
                {
                    var vals = new List<string[]>();
                    while (parser.HasMoreRows)
                    {
                        var row = parser.ReadNextRow();

                        if (row == null) break;

                        vals.Add(row);
                    }

                    values = vals.ToArray();
                }

                keys = values[0];
            }


            public FancySheet(string content)
            {
                values = CsvParser.Parse(content, Settings);

                keys = values[0];
            }

            public static System.Reflection.MethodInfo GetStingConvertMethod(Type t)
            {
                return t
                    .GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttributes(typeof(CreateFromStringAttribute), false).Length > 0);
            }

            static object ConvertFromString(Type t, object s)
            {
                return GetStingConvertMethod(t).Invoke(null, new[] { s });
            }

            static bool IsStringConvertible(Type t)
            {
                return GetStingConvertMethod(t) != null;
            }

            static bool IsGeneric(Type test, Type inputType, out Type elementType)
            {
                if (inputType.IsGenericType && (inputType.GetGenericTypeDefinition() == test))
                {
                    elementType = inputType.GetGenericArguments()[0];
                    return true;
                }

                elementType = null;
                return false;
            }

            static bool IsGeneric(Type inputType, out Type elementType)
            {
                if (inputType.IsGenericType)
                {
                    elementType = inputType.GetGenericArguments()[0];
                    return true;
                }

                elementType = null;
                return false;
            }

            T Convert<T>(string[] row) where T : new()
            {
                var item = new T();
                var type = typeof(T);

                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];
                    if (key == null) continue;
                    string value = i < row.Length ? row[i] : "";
                    var field = type.GetField(key);
                    if (field == null) continue;
                    var fType = field.FieldType;

                    var obj = ConvertOne(fType, value);
                    if (obj != null)
                    {
                        field.SetValue(item, obj);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Unconvertable type {fType}");
                    }

                }

                return item;
            }

            public static object GetDefault(Type type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                return null;
            }

            public static object ConvertGeneric(Type fType, Type eType, string value, string addMethodName = "Add")
            {
                var list = Activator.CreateInstance(fType);

                // support empty lists
                if (string.IsNullOrWhiteSpace(value)) return list;

                var addMethod = fType.GetMethod(addMethodName);

                foreach (var s in value.Split(';').Select(x => x.Trim()))
                {
                    var o = ConvertOne(eType, s);
                    if (o == null) return null; //means we can't convert

                    addMethod.Invoke(list, new[] { o });
                }

                return list;
            }

            public static object ConvertOne(Type fType, string value)
            {
                if (value == null) value = "";

                if (fType.IsEnum)
                {
                    return Enum.Parse(fType, value, true);
                }
                else if (fType == typeof(fixed64))
                {
                    return GetFixed64(value);
                }
                else if (IsStringConvertible(fType))
                {
                    return ConvertFromString(fType, value);
                }
                else if (IsGeneric(fType, out var eType))
                {
                    return ConvertGeneric(fType, eType, value);
                }

                try
                {
                    if (value == "") throw new InvalidCastException();
                    return System.Convert.ChangeType(value, fType);
                } catch (InvalidCastException)
                {
                    return GetDefault(fType);
                }
            }


            public List<T> Convert<T>() where T : new()
            {
                var result = new List<T>();

                for (int i = 1; i < values.Length; i++)
                {
                    var item = values[i];

                    // skip empty rows
                    if (item.All(string.IsNullOrWhiteSpace)) continue;
                    result.Add(Convert<T>(item));
                }

                return result;
            }

            public static TEnum GetEnum<TEnum>(string obj) where TEnum : struct
            {
                if (Enum.TryParse(obj, true, out TEnum value))
                {
                    return value;
                }

                return default;
            }

            public static fixed64 GetFixed64(string obj)
            {
                if (!string.IsNullOrEmpty(obj))
                {
                    return fixed64.Parse(obj);
                }

                return new fixed64();
            }
        }
    }

    public class CreateFromStringAttribute : Attribute
    {
    }

    public static class ReflectionHelpersForSheets
    {
        public static object InvokeStatic(this System.Reflection.MethodInfo method, params object[] args) => method.Invoke(null, args);
    }
}