using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FancyToolkit
{
    public class GenericDataCtrl<TData> where TData : DataWithId, new()
    {
        Dictionary<string, TData> dict = new();
        List<TData> list = new();

        public TData Get(string id) => dict.Get(id);
        public TClass Get<TClass>(string id) where TClass: TData => dict.Get(id) as TClass;
        public List<TData> GetAll() => list;

        public TData Add(TData data)
        {
            dict.Add(data.id, data);
            list.Add(data);
            PostInitSingle(data);
            return data;
        }

        public virtual void PostInitSingle(TData data)
        {

        }

        public void AddData<TClass>(List<TClass> entries) where TClass : TData, new()
        {
            foreach (var item in entries)
            {
                Add(item);
            }
            PostInit();
        }

        public void AddData(TextAsset csv, bool debug = false) => AddData<TData>(csv, debug);
        public void AddData<TClass>(TextAsset csv, bool debug = false) where TClass : TData, new()
        {
            var list = FancyCSV.FromText<TClass>(csv.text, debug);
            AddData(list);
        }

        public void AddCSV(string csvName, bool debug = false) => AddCSV<TData>(csvName, debug);
        public void AddCSV<TClass>(string csvName, bool debug = false) where TClass : TData, new()
        {
            var list = FancyCSV.FromCSV<TClass>(csvName, debug);
            AddData(list);
        }

        public virtual void PostInit()
        {

        }
    }

    public class DataWithId
    {
        public string id;
    }
}
