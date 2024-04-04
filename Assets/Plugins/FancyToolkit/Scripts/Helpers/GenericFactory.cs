using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FancyToolkit
{
    public static class Factory<TClass> where TClass : class, IFactorableWithCreate
    {
        public static Dictionary<string, TClass> dict;

        static void Init()
        {
            var type = typeof(TClass);

            dict = Assembly.GetAssembly(type)
                .GetTypes()
                .Where(t => type.IsAssignableFrom(t) && t.IsAbstract == false)
                .Select(t => System.Activator.CreateInstance(t) as TClass)
                .ToDictionary(t => t.Id);
        }

        public static TClass Create(string line, TClass defaultObject = null)
        {
            if (dict == null) Init();

            if (string.IsNullOrWhiteSpace(line)) return defaultObject;

            var scanner = new StringScanner(line);

            // get id
            if (dict.TryGetValue(scanner.NextString(), out var factory))
            {
                return (TClass)factory.Create(scanner);
            }

            return defaultObject;
        }

        public static TClass GetFactoryClass(string line, TClass defaultObject = null)
        {
            if (dict == null) Init();

            if (string.IsNullOrWhiteSpace(line)) return defaultObject;

            StringUtil.TryIdSplit(line, out var id, out var other);

            if (dict.TryGetValue(id, out var factory))
            {
                return factory;
            }

            return defaultObject;
        }
    }

    public interface IFactorableWithCreate
    {
        string Id { get; }
        object Create(StringScanner scanner);
    }
}