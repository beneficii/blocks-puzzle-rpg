using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine;

namespace FancyToolkit
{
    [System.Serializable]
    public class TableList<TData> where TData : new()
    {
        [SerializeField] protected TextAsset csv;
        public List<TData> list;

        public void Load()
        {
            list = FancyCSV.FromText<TData>(csv.text);
        }

        public List<TData> Generate() => FancyCSV.FromText<TData>(csv.text);
    }
}

