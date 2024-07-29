using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class UIResourceBar : MonoBehaviour
    {
        public ResourceType type;
        public int max = 100;
        public TMP_Text txtValue;
        public Slider slider;

        private void OnEnable()
        {
            ResourceCtrl.OnChanged += HandleResourceChange;
            slider.maxValue = max;
            txtValue.SetText($"{ResourceUtil.Possix(type)}");
            HandleResourceChange(type, ResourceCtrl.current.Get(type));
        }

        private void OnDisable()
        {
            ResourceCtrl.OnChanged -= HandleResourceChange;
        }

        void HandleResourceChange(ResourceType type, int value)
        {
            if (this.type != type) return;
            slider.value = value;
        }
    }
}