using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FancyToolkit
{
    public class GenericDataCtrl<TData> where TData : DataWithId, new()
    {
        Dictionary<string, TData> dict = new();
        List<TData> allDataList = new();

        Dictionary<string, List<TData>> upgrades = new();

        public TData Get(string id) => dict.Get(id);
        public TClass Get<TClass>(string id) where TClass: TData => dict.Get(id) as TClass;
        public List<TData> GetAll() => allDataList;

        public int GetUpgradeCount(string id)
        {
            if (!upgrades.TryGetValue(id, out var list))
            {
                return 0;
            }

            return list.Count;
        }

        public TData GetLevel(string id, int level)
        {
            if (level == 0) return Get(id);
            if (!upgrades.TryGetValue(id, out var list)) return null;
            
            int upLevel = level - 1;
            if (upLevel >= list.Count) return null;

            return list[upLevel];
        }

        public TData Add(TData data)
        {
            dict.Add(data.id, data);
            allDataList.Add(data);
            PostInitSingle(data);
            return data;
        }

        public void AddUpgrade(TData data, string id)
        {
            data.id = id;
            if (!upgrades.TryGetValue(id, out var upList))
            {
                upList = new List<TData>();
                upgrades.Add(id, upList);
            }
            upList.Add(data);
            PostInitSingle(data);
        }

        public virtual void PostInitSingle(TData data)
        {

        }

        public void AddData<TClass>(List<TClass> entries) where TClass : TData, new()
        {
            string lastId = null;
            foreach (var item in entries)
            {
                if (string.IsNullOrEmpty(item.id) && lastId != null)
                {
                    AddUpgrade(item, lastId);
                    continue;
                }
                Add(item);
                lastId = item.id;
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
