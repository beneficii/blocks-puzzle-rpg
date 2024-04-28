using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class ResCtrl<TRes> where TRes : struct, System.Enum
    {
        static ResCtrl<TRes> _current;
        public static ResCtrl<TRes> current
        {
            get
            {
                if (_current == null) new ResCtrl<TRes>().Init();

                return _current;
            }
        }

        public static event System.Action<TRes, int> OnChanged;
        public static event System.Action<TRes, int> OnDelta;

        List<int> values;

        public void Clear()
        {
            foreach (var item in EnumUtil.GetValues<TRes>())
            {
                Set(item, 0);
            }
        }

        private void Init()
        {
            _current = this;
            values = new List<int>();
            foreach (var item in EnumUtil.GetValues<TRes>())
            {
                values.Add(0);
            }
        }

        public int Idx(TRes type) => System.Convert.ToInt32(type);

        public int Get(TRes type) => values[Idx(type)];

        public bool Enough(TRes type, int value) => Get(type) >= value;

        public bool Enough(Info info) => Enough(info.type, info.value);

        public void Add(TRes type, int amount)
        {
            values[Idx(type)] += amount;
            OnChanged?.Invoke(type, Get(type));
            OnDelta?.Invoke(type, amount);
        }

        public void SetIfMore(TRes type, int value)
        {
            int idx = Idx(type);
            int oldValue = values[idx];
            if (oldValue >= value) return;
            values[idx] = value;
            OnChanged?.Invoke(type, Get(type));
            OnDelta?.Invoke(type, value - oldValue);
        }

        public void Add(List<Info> list)
        {
            foreach (var item in list)
            {
                Add(item.type, item.value);
            }
        }

        public bool Remove(TRes type, int amount, bool canBeNegative = false)
        {
            if (!canBeNegative && !Enough(type, amount)) return false;
            int idx = Idx(type);
            int current = values[idx];

            values[idx] -= amount;
            OnChanged?.Invoke(type, Get(type));
            OnDelta?.Invoke(type, -amount);

            return true;
        }

        public void Set(TRes type, int value)
        {
            int idx = Idx(type);
            int oldValue = values[idx];
            values[idx] = value;
            OnChanged?.Invoke(type, Get(type));
            OnDelta?.Invoke(type, value - oldValue);
        }

        public void SetFrom(TRes target, TRes from)
        {
            Set(target, Get(from));
        }

        public bool Enough(List<Info> list)
        {
            foreach (var item in list)
            {
                if (!Enough(item.type, item.value)) return false;
            }

            return true;
        }

        public bool Remove(List<Info> list)
        {
            if (!Enough(list)) return false;

            foreach (var item in list)
            {
                Remove(item.type, item.value);
            }
            return true;
        }

        public bool Set(List<Info> list)
        {
            foreach (var item in list)
            {
                Set(item.type, item.value);
            }
            return true;
        }

        public class Info
        {
            public TRes type;
            public int value;

            public Info(TRes type, int value)
            {
                this.type = type;
                this.value = value;
            }
        }
    }
}