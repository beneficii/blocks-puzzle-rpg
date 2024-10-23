using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace FancyToolkit
{
    public static class Factory<TClass> where TClass : class
    {
        public static Dictionary<string, Func<StringScanner, TClass>> ctrDict;
        public static Dictionary<string, TClass> objDict;
        public static Dictionary<string, Func<FactoryBuilder<TClass>>> builderDict;

        public static bool ImplementsGenericClass(Type type, Type genericClassType)
        {
            while (type != null && type != typeof(object))
            {
                var currentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (genericClassType == currentType)
                {
                    return true;
                }
                type = type.BaseType;
            }

            return false;
        }

        private static Type FindNestedClass(Type outerType, Type searchType)
        {
            return outerType.GetNestedTypes()
                .FirstOrDefault(t => ImplementsGenericClass(t, searchType));
        }

        public static void AddEntries<TType>()
        {
            if (ctrDict == null) Init();

            var type = typeof(TClass);

            var additionalEntries = Assembly.GetAssembly(typeof(TType)).GetTypes()
                .Where(t => type.IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    StringScannerConstructor = FindConstructorWithStringScanner(t),
                    ParameterlessConstructor = t.GetConstructor(Type.EmptyTypes),
                    BuilderType = FindNestedClass(t, typeof(FactoryBuilder<>))
                })
                .ToList();

            foreach (var entry in additionalEntries)
            {
                var typeName = entry.Type.Name;
                if (entry.StringScannerConstructor != null)
                {
                    if (!ctrDict.ContainsKey(typeName))
                    {
                        ctrDict[typeName] = CreateConstructorDelegate<TClass>(entry.StringScannerConstructor);
                    }
                    else
                    {
                        Debug.LogError($"{typeName} already in the ctrDict!");
                    }
                }

                if (entry.ParameterlessConstructor != null)
                {
                    if (!objDict.ContainsKey(typeName))
                    {
                        objDict[typeName] = (TClass)entry.ParameterlessConstructor.Invoke(null);
                    }
                    else
                    {
                        Debug.LogError($"{typeName} already in the objDict!");
                    }
                }

                if (entry.BuilderType != null)
                {
                    var builderConstructor = entry.BuilderType.GetConstructor(Type.EmptyTypes);
                    if (builderDict.ContainsKey(typeName))
                    {
                        Debug.LogError($"builderDict already contains builder for `{typeName}`");
                    }
                    else if (builderConstructor == null)
                    {
                        Debug.LogError($"No empty constructor for `{typeName}`");
                    }
                    else
                    {
                        builderDict[typeName] = CreateEmptyConstructor<FactoryBuilder<TClass>>(builderConstructor);
                    }
                }
            }
        }

        static ConstructorInfo FindConstructorWithStringScanner(Type type)
        {
            while (type != null)
            {
                var constructor = type.GetConstructor(new Type[] { typeof(StringScanner) });
                if (constructor != null)
                {
                    return constructor;
                }
                type = type.BaseType;
            }
            return null;
        }

        static Func<TResult> CreateEmptyConstructor<TResult>(ConstructorInfo constructor)
        {
            var newExpr = Expression.New(constructor);
            var lambda = Expression.Lambda<Func<TResult>>(newExpr);
            return lambda.Compile();
        }

        static Func<StringScanner, TResult> CreateConstructorDelegate<TResult>(ConstructorInfo constructor)
        {
            var scannerParam = Expression.Parameter(typeof(StringScanner), "scanner");
            var newExpr = Expression.New(constructor, scannerParam);
            var lambda = Expression.Lambda<Func<StringScanner, TResult>>(newExpr, scannerParam);
            return lambda.Compile();
        }

        static void Init()
        {
            ctrDict = new();
            objDict = new();
            builderDict = new();
            AddEntries<TClass>();
        }

        public static FactoryBuilder<TClass> CreateBuilder(string line)
        {
            if (builderDict == null) Init();

            if (string.IsNullOrWhiteSpace(line)) return null;

            return CreateBuilder(new StringScanner(line));
        }

        public static FactoryBuilder<TClass> CreateBuilder(StringScanner scanner)
        {
            if (builderDict == null) Init();

            var id = scanner.NextString();
            if (!builderDict.TryGetValue(id, out var builderFactory))
            {
                Debug.LogError($"Could not find builder for `{id}`");
                return null;
            }

            var instance = builderFactory();
            instance.Init(scanner);
            return instance;
        }

        public static TClass Create(string line, TClass defaultObject = null)
        {
            if (ctrDict == null) Init();

            if (string.IsNullOrWhiteSpace(line)) return defaultObject;

            var scanner = new StringScanner(line);

            var id = scanner.NextString();
            if (ctrDict.TryGetValue(id, out var factory))
            {
                var obj = factory(scanner);
                if (obj == null) Debug.LogError($"Factory::Create failed '{id}'");
                return obj;
            }
            else if (objDict.TryGetValue(id, out var instance))
            {
                if (!scanner.Empty)
                {
                    Debug.LogError("Scanner had some parameters, but using instance Factory obj");
                }

                return instance;
            }

            return defaultObject;
        }
    }

    public abstract class FactoryBuilder<TClass>
    {
        public virtual void Init(StringScanner scanner) { }
        public abstract TClass Build();
    }

    public abstract class FactoryBuilder<TClass, TValue> : FactoryBuilder<TClass>
    {
        protected TValue value;
        public override void Init(StringScanner scanner) { scanner.TryGetGeneric(out value); }
    }

    public abstract class FactoryBuilder<TClass, TValue1, TValue2> : FactoryBuilder<TClass>
    {
        protected TValue1 value;
        protected TValue2 value2;
        public override void Init(StringScanner scanner)
        {
            scanner.TryGetGeneric(out value);
            scanner.TryGetGeneric(out value2);
        }
    }

    public abstract class FactoryBuilder<TClass, TValue1, TValue2, TValue3> : FactoryBuilder<TClass>
    {
        protected TValue1 value;
        protected TValue2 value2;
        protected TValue3 value3;

        public override void Init(StringScanner scanner)
        {
            scanner.TryGetGeneric(out value);
            scanner.TryGetGeneric(out value2);
            scanner.TryGetGeneric(out value3);
        }
    }

    public abstract class FactoryBuilder<TClass, TValue1, TValue2, TValue3, TValue4> : FactoryBuilder<TClass>
    {
        protected TValue1 value;
        protected TValue2 value2;
        protected TValue3 value3;
        protected TValue4 value4;

        public override void Init(StringScanner scanner)
        {
            scanner.TryGetGeneric(out value);
            scanner.TryGetGeneric(out value2);
            scanner.TryGetGeneric(out value3);
            scanner.TryGetGeneric(out value4);
        }
    }
}