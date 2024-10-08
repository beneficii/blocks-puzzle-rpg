using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class ValueBar : MonoBehaviour
    {
        public event System.Action OnZero;
        public event System.Action OnFull;

        public static event System.Action<ValueBar, int> OnRemove;
        public static event System.Action<ValueBar, int> OnAdd;

        public NumberFloatingAnimator numberAnimator;

        [SerializeField] TextMeshPro txtCaption;
        [SerializeField] string captionPrefix;
        [SerializeField] SpriteRenderer fillFg;
        [SerializeField] SpriteRenderer fillBg;

        [SerializeField] Color clrFill = Color.red;
        [SerializeField] Color clrRemove = Color.white;
        [SerializeField] Color clrAdd = Color.green;

        [SerializeField] float oldValueDelay = 0.15f;
        //[SerializeField] float animationCatchupSpeed = 1f;

        [SerializeField] bool HiddenInitially = false;

        IntReference reference;

        int currentValue;
        int maxValue;
        public int Value
        {
            get => currentValue;
        }

        float animationCatchupTime;
        int oldValue;

        Vector3 GetBarSize(int current)
        {
            return new Vector3(current / (float)maxValue, 1, 1);
        }

        void SetFill(SpriteRenderer fill, int value, Color color = default)
        {
            if (value == 0)
            {
                fill.transform.localScale = Vector3.zero;
            } 
            fill.transform.localScale = GetBarSize(value);
            fill.color = color;
        }

        public void Init(int max) => Init(max, max);

        public void Init(int current, int max)
        {
            maxValue = max;
            Set(current);
            if (HiddenInitially) gameObject.SetActive(false);
        }

        public void Init(IntReference reference)
        {
            this.reference = reference;
            Init(reference.Value, reference.maxValue);
        }

        void RefreshInstant()
        {
            SetFill(fillFg, currentValue, clrFill);
            SetFill(fillBg, 0, Color.white);
            oldValue = currentValue;
            RefreshText();
        }

        public void SetFillColor(Color color)
        {
            clrFill = color;
            RefreshInstant();
        }

        public void Set(int value)
        {
            currentValue = Mathf.Clamp(value, 0, maxValue);
            RefreshInstant();
        }

        public void SetMax(int max)
        {
            maxValue = max;
            Set(Value);
        }

        void SetAnimated(int value)
        {
            oldValue = currentValue;
            currentValue = Mathf.Clamp(value, 0, maxValue);
            if (oldValue == currentValue) return;

            
            if (HiddenInitially && !gameObject.activeSelf) gameObject.SetActive(true);

            if (currentValue == maxValue)
            {
                OnFull?.Invoke();
            }
            else if (currentValue == 0)
            {
                if (HiddenInitially) gameObject.SetActive(false);
                OnZero?.Invoke();
            }

            animationCatchupTime = Time.time + oldValueDelay;

            if (oldValue > currentValue)
            {
                SetFill(fillFg, currentValue, clrFill);
                SetFill(fillBg, oldValue, clrRemove);
                int removed = oldValue - currentValue;
                OnRemove?.Invoke(this, removed);
                if (numberAnimator) numberAnimator.Spawn(-removed);
            }
            else
            {
                SetFill(fillFg, oldValue, clrFill);
                SetFill(fillBg, currentValue, clrAdd);
                int added = currentValue - oldValue;
                OnAdd?.Invoke(this, added);
                if (numberAnimator) numberAnimator.Spawn(added);
            }
        }

        public void Remove(int amount)
        {
            SetAnimated(currentValue - amount);
        }

        void RefreshText()
        {
            if (!txtCaption) return;

            txtCaption.text = $"{captionPrefix}{Value}";
        }

        public void Add(int amount)
        {
            SetAnimated(currentValue + amount);
        }

        private void Update()
        {
            if (oldValue != currentValue && Time.time > animationCatchupTime)
            {
                //Animate();
                RefreshInstant();
            }
        }
    }
}
