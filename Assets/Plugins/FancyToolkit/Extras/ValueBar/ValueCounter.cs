using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class ValueCounter : MonoBehaviour
    {
        public event System.Action OnZero;

        public NumberFloatingAnimator numberAnimator;

        [SerializeField] TMP_Text caption;
        [SerializeField] bool hiddenOnZero = false;

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
                currentValue = value;
                caption.text = value.ToString();
                
            }
        }

        public void Remove(int amount)
        {
            Value -= amount;
            if (numberAnimator) numberAnimator.Spawn(-amount);
        }

        public void Add(int amount)
        {
            Value += amount;
            if (numberAnimator) numberAnimator.Spawn(amount);
        }
    }
}
