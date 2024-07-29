using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public class SmoothBar : MonoBehaviour
    {
        public float animationSpeed = 16f;

        public System.Action OnZero;
        public System.Action OnMax;

        int currentValue;
        public int Value
        {
            get => currentValue;
            set
            {
                if (value == currentValue) return;

                currentValue = Mathf.Clamp(value, 0, maxValue);
                needsChange = true;
            }
        }
        
        float currentVisual;
        int maxValue;

        bool needsChange;
        bool initDone = false;

        public bool Finished => !needsChange;

        public void Init(int value, int max = 0)
        {
            if (max == 0)
            {
                max = value;
                if (value == 0)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }

            currentVisual = currentValue = value;
            maxValue = max;
            Refresh();
            needsChange = false;
            initDone = true;
        }


        void Refresh()
        {
            transform.localScale = new Vector3(currentVisual / maxValue, 1, 1);
        }


        void Update()
        {
            if (!initDone || !needsChange) return;
            float value = (float)currentValue;
            currentVisual = Mathf.MoveTowards(currentVisual, value, animationSpeed * Time.deltaTime);
            if (currentVisual == value)
            {
                needsChange = false;
                if (currentValue == 0) OnZero?.Invoke();
                if (currentValue == maxValue) OnMax?.Invoke();
            }
            Refresh();
        }
    }
}