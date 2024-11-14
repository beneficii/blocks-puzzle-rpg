using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FancyToolkit
{
    public class GenericDataCtrl<TData> where TData : DataWithId, new()
    {
        public Dictionary<string, TData> dict { get; private set; } = new Dictionary<string, TData>();
        public List<TData> list { get; private set; } = new();

        public TData Get(string id) => dict.Get(id);
        public List<TData> GetAll() => list;

        public virtual void Add(TData data)
        {
            dict.Add(data.id, data);
            list.Add(data);
        }

        public void AddData<TClass>(TextAsset csv, bool debug = false) where TClass : TData, new()
        {
            var list = FancyCSV.FromText<TClass>(csv.text, debug);
            Debug.Log($"GenericDataCtrl<{typeof(TData)}>::Add cnt: {list.Count}");
            foreach (var item in list)
            {
                Add(item);
            }
            PostInit();
        }

        public virtual void PostInit()
        {

        }

        public void AddData(TextAsset csv, bool debug = false) => AddData<TData>(csv, debug);
    }

    public class DataWithId
    {
        public string id;
    }
}