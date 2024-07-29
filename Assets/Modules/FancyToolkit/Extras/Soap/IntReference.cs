using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    [System.Serializable]
    public class IntReference
    {
        const float animationDuration = 0.13f;

        public System.Action<int> OnRemoved;
        public System.Action<int> OnAdded;
        public System.Action<int, int> OnChanged;

        int value;
        public int minValue { get; private set; }
        public int maxValue { get; private set; }

        int lastDelta;
        float animationResetTime;

        [SerializeField] public int CurrentDelta => Time.time < animationResetTime ? lastDelta : 0;
        [SerializeField] public float NextRefresh => animationResetTime;

        public int Value
        {
            get => value;

            set
            {
                var old = this.value;
                this.value = Mathf.Clamp(value, minValue, maxValue);

                var delta = this.value - old;
                if (delta == 0) return;
                lastDelta = delta;
                animationResetTime = Time.time + animationDuration;
                OnChanged?.Invoke(this.value, lastDelta);
            }
        }

        public IntReference(int value = 0, int maxValue = int.MaxValue, int minValue = 0)
        {
            this.value = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public void Remove(int val)
        {
            Value -= val;
            OnRemoved?.Invoke(-lastDelta);
        }

        public void Add(int val)
        {
            Value += val;
            OnRemoved?.Invoke(lastDelta);
        }
    }
}