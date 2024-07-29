using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    [System.Serializable]
    public class HashList<TObj> : IEnumerable<TObj>
    {
        [SerializeField] List<TObj> list;
        HashSet<TObj> cachedSet;


        HashSet<TObj> GetSet()
        {
            if (cachedSet == null) Init();
            return cachedSet;
        }

        void Init()
        {
            cachedSet = new HashSet<TObj>(list);
        }

        public void Add(TObj obj)
        {
            list.Add(obj);
        }

        public bool Contains(TObj obj) => GetSet().Contains(obj);

        [CreateFromString]
        public static HashList<TObj> FromString(string str)
        {
            var list = (List<TObj>)FancyCSV.FancySheet.ConvertGeneric(typeof(List<TObj>), typeof(TObj), str);

            return new HashList<TObj>
            {
                list = list,
            };
        }

        public IEnumerator<TObj> GetEnumerator()
        {
            return ((IEnumerable<TObj>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}
