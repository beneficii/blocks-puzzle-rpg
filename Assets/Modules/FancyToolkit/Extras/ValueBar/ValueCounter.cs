using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class ValueCounter : MonoBehaviour
    {
        public event System.Action OnZero;
        public event System.Action<int, int> OnChanged;

        public NumberFloatingAnimator numberAnimator;

        [SerializeField] TMP_Text caption;
        [SerializeField] string prefix;
        [SerializeField] bool hiddenOnZero = false;

        [SerializeField] bool animateColor;
        [SerializeField] Color clrAdd = Color.green;
        [SerializeField] Color clrRemove = Color.white;
        [SerializeField] float colorAnimationDuration = 0.15f;

        Color clrDefault;
        float animationClearTime;

        int currentValue;
        public int Value
        {
            get => currentValue;
            set
            {
                value = Mathf.Max(value, 0);
                if (hiddenOnZero && ((value == 0) != (currentValue == 0)))
                {
                    gameObject.SetActive(value != 0);
                    if (value == 0) OnZero?.Invoke();
                }
                OnChanged?.Invoke(currentValue, value);
                currentValue = value;
                caption.text = $"{prefix}{value}";
            }
        }

        private void Start()
        {
            if (hiddenOnZero) gameObject.SetActive(currentValue != 0);
            clrDefault = caption.color;
        }

        void AnimateTextColor(Color color)
        {
            caption.color = color;
            animationClearTime = Time.time + colorAnimationDuration;
        }

        public void Remove(int amount)
        {
            Value -= amount;
            if (numberAnimator) numberAnimator.Spawn(-amount);
            if (animateColor) AnimateTextColor(clrRemove);
        }

        public void Add(int amount)
        {
            Value += amount;
            if (numberAnimator) numberAnimator.Spawn(amount);
            if (animateColor) AnimateTextColor(clrAdd);
        }

        private void Update()
        {
            if (animationClearTime > 0f && Time.time >= animationClearTime)
            {
                caption.color = clrDefault;
                animationClearTime = 0f;
            }
        }
    }
}
