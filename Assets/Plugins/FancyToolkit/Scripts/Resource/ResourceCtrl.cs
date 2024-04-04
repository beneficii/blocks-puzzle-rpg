using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    [DefaultExecutionOrder(-10)]
    public class ResourceCtrl : MonoBehaviour
    {
        public static ResourceCtrl current;

        public static event System.Action<ResourceType, int> OnChanged;
        public static event System.Action<ResourceType, int> OnDelta;

        List<int> values;

        private void Awake()
        {
            current = this;
            values = new List<int>();
            foreach (var item in EnumUtil.GetValues<ResourceType>())
            {
                values.Add(0);
            }
        }

        public int Idx(ResourceType type) => (int)type;

        public int Get(ResourceType type) => values[Idx(type)];

        public bool Enough(ResourceType type, int value) => Get(type) >= value;

        public bool Enough(ResourceInfo info) => Enough(info.type, info.value);

        public void Add(ResourceType type, int amount)
        {
            values[Idx(type)] += amount;
            OnChanged?.Invoke(type, Get(type));
            OnDelta?.Invoke(type, amount);
        }

        public void Add(List<ResourceInfo> list)
        {
            foreach (var item in list)
            {
                Add(item.type, item.value);
            }
        }

        public bool Remove(ResourceType type, int amount, bool canBeNegative = false)
        {
            if (!canBeNegative && !Enough(type, amount)) return false;
            int idx = Idx(type);
            int current = values[idx];

            values[idx] -= amount;
            OnChanged?.Invoke(type, Get(type));
            OnDelta?.Invoke(type, -amount);

            return true;
        }

        public void Set(ResourceType type, int value)
        {
            int idx = Idx(type);
            int oldValue = values[idx];
            values[idx] = value;
            OnChanged?.Invoke(type, Get(type));
            OnDelta?.Invoke(type, value - oldValue);
        }

        public void SetFrom(ResourceType target, ResourceType from)
        {
            Set(target, Get(from));
        }

        public bool Enough(List<ResourceInfo> list)
        {
            foreach (var item in list)
            {
                if (!Enough(item.type, item.value)) return false;
            }

            return true;
        }

        public bool Remove(List<ResourceInfo> list)
        {
            if (!Enough(list)) return false;

            foreach (var item in list)
            {
                Remove(item.type, item.value);
            }
            return true;
        }
    }
}