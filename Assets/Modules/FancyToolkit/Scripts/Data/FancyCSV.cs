using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Nortal.Utilities.Csv;
using System.Globalization;
using FixedMath;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FancyToolkit
{

    public static class FancyCSV
    {
        public static List<T> FromCSV<T>(string fileName, bool debug = false) where T : new()
        {
            var content = CSVManager.current.GetContent(fileName);
            if (content == null)
            {
                UnityEngine.Debug.LogError($"Could not find csv: {fileName}");
            }
            return new FancySheet(content, debug).Convert<T>();
        }

        public static List<T> FromText<T>(string text, bool debug = false) where T : new()
        {
            return new FancySheet(text, debug).Convert<T>();
        }

        public static List<T> FromFile<T>(string filePath, bool debug = false) where T : new()
        {
            FancySheet result = null;
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                result = new FancySheet(sr, debug);
            }

            return result?.Convert<T>();
        }

        public class FancySheet
        {
            string[][] keys;
            string[][] values;

            bool debug;

            CsvSettings Settings => new CsvSettings()
            {
                FieldDelimiter = '|',
                RowDelimiter = "\n",
            };

            void QLog(string msg)
            {
                if (debug) UnityEngine.Debug.Log(msg);
            }

            public FancySheet(StreamReader streamReader, bool debug = false)
            {
                this.debug = debug;

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

                keys = values[0]
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Split('.'))
                    .ToArray();
            }

            public FancySheet(string content, bool debug = false)
            {

                values = CsvParser.Parse(content.Replace("\r",""), Settings);

                // Extract keys from the parsed content
                keys = values[0]
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Split('.'))
                    .ToArray();
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

            public static void SetFieldValue(object obj, string[] fieldNames, string value)
            {
                if (obj == null) throw new ArgumentNullException(nameof(obj));
                if (fieldNames == null || fieldNames.Length == 0) return;

                for (int i = 0; i < fieldNames.Length; i++)
                {
                    var fieldName = fieldNames[i];
                    if (string.IsNullOrWhiteSpace(fieldName)) return;

                    var type = obj.GetType();
                    var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (field == null)
                    {
                        UnityEngine.Debug.LogError($"Field '{fieldName}' not found in type '{type.FullName}'");
                        return;
                    }

                    // If it's the last field name, set the value
                    if (i == fieldNames.Length - 1)
                    {
                        var convertedValue = ConvertValue(field.FieldType, value, fieldName);
                        field.SetValue(obj, convertedValue);
                    }
                    else
                    {
                        // If it's not the last field name, traverse into the nested object
                        var nestedObj = field.GetValue(obj);
                        if (nestedObj == null)
                        {
                            nestedObj = Activator.CreateInstance(field.FieldType);
                            field.SetValue(obj, nestedObj);
                        }
                        obj = nestedObj;
                    }
                }
            }

            T Convert<T>(string[] row) where T : new()
            {
                var item = new T();

                for (int i = 0; i < keys.Length; i++)
                {
                    SetFieldValue(item, keys[i], i < row.Length ? row[i] : "");
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
                    var o = ConvertValue(eType, s);
                    if (o == null) return null; //means we can't convert

                    addMethod.Invoke(list, new[] { o });
                }

                return list;
            }

            public static object ConvertValue(Type fType, string value, string dbg = "<none>")
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
                else if (TryConvertFactoryBuilder(fType, value, out var factoryBuilder))
                {
                    return factoryBuilder;
                }
                else if (IsGeneric(fType, out var eType))
                {
                    return ConvertGeneric(fType, eType, value);
                }

                var converter = TypeDescriptor.GetConverter(fType);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFromInvariantString(value);
                }

                UnityEngine.Debug.Log($"Could not convert: {value}[{fType}]");
                return GetDefault(fType);
            }

            public static bool TryConvertFactoryBuilder(Type fType, string value, out object result)
            {
                result = null;
                
                if (!fType.IsGenericType || fType.GetGenericTypeDefinition() != typeof(FactoryBuilder<>)) return false;
                var tClassType = fType.GetGenericArguments()[0];
                var factoryType = typeof(Factory<>).MakeGenericType(tClassType);
                var createBuilderMethod = factoryType.GetMethod("CreateBuilder", new Type[] { typeof(string) });

                if (createBuilderMethod == null) return false;

                result = createBuilderMethod.Invoke(null, new object[] { value });

                return true;
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