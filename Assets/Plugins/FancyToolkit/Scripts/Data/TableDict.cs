using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine;

namespace FancyToolkit
{
    [System.Serializable]
    public class TableDict <TData> where TData : ITableSerializable, new()
    {
        [SerializeField] protected TextAsset csv;
        public List<TData> list;

        protected Dictionary<string, TData> dict;

        public virtual void Init()
        {
            Assert.IsNotNull(list, "Table list can't be null!");
            dict = list.ToDictionary(x=>x.GetId());
        }

        public TData Get(string key)
        {
            if(dict == null) Init();

            return dict.Get(key);
        }

        public void Load()
        {
            LoadFromText(csv.text);
        }

        public List<TOut> GetFieldList<TOut>(System.Func<TData, TOut> getter) => list.Select(getter).ToList();

        void LoadFromText(string text)
        {
            list = FancyCSV.FromText<TData>(text);
            dict = null;
        }
    }

    public interface ITableSerializable
    {
        string GetId();
    }
}

