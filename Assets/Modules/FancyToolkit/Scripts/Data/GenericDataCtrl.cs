using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FancyToolkit
{
    public class GenericDataCtrl<TData> where TData : DataWithId, new()
    {
        public Dictionary<string, TData> dict { get; private set; } = new Dictionary<string, TData>();

        public TData Get(string id) => dict.Get(id);
        public List<TData> GetAll() => dict.Values.ToList();

        public virtual void Add(TData data)
        {
            dict.Add(data.id, data);
        }

        public void AddData<TClass>(TextAsset csv) where TClass : TData, new()
        {
            var list = FancyCSV.FromText<TClass>(csv.text);
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public void AddData(TextAsset csv) => AddData<TData>(csv);
    }

    public class DataWithId
    {
        public string id;
    }
}