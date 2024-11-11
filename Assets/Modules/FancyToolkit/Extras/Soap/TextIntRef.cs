using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class TextIntRef : MonoBehaviour
    {
        TextMeshPro txtLabel;

        List<IntReferenceDisplay> items;

        float nextRefresh = 0f;
        float prevRefresh = 0f;

        public void Init(List<IntReferenceDisplay> items)
        {
            txtLabel = GetComponent<TextMeshPro>();
            this.items = items;
            foreach (var item in items)
            {
                item.reference.OnChanged += HandleChanged;
            }
            Refresh();
        }

        private void OnDestroy()
        {
            if (items == null) return;
            foreach (var item in items)
            {
                item.reference.OnChanged -= HandleChanged;
            }
        }

        void Refresh()
        {
            prevRefresh = Time.time;
            nextRefresh = float.PositiveInfinity;

            var lines = new List<string>();
            foreach (var item in items)
            {
                if (item.reference.Value == 0) continue;

                var d = item.reference.CurrentDelta;
                var color = (d == 0) ? item.clrDefault : (d > 0) ? item.clrPositive : item.clrNegative;
                string dbgColor = (d == 0) ? "default" : (d > 0) ? "positive" : "negative";

                string colorTag = ColorUtility.ToHtmlStringRGBA(color);

                var maxVal = item.reference.maxValue;
                var strMax = maxVal != int.MaxValue ? $"/{maxVal}" : "";

                lines.Add($"<color=#{colorTag}>{item.prefix}{item.reference.Value}{strMax}</color>");

                var nr = item.reference.NextRefresh;
                if (nr > prevRefresh && nr < nextRefresh)
                {
                    nextRefresh = nr;
                }
            }

            txtLabel.text = string.Join(' ', lines);
        }

        private void Update()
        {
            if (nextRefresh > prevRefresh && Time.time >= nextRefresh)
            {
                Refresh();
            }
        }

        void HandleChanged(int val, int dif)
        {
            Refresh();
        }
    }

    [System.Serializable]
    public class IntReferenceDisplay
    {
        public string prefix;
        public IntReference reference;

        public Color clrDefault = Color.white;
        public Color clrNegative = Color.red;
        public Color clrPositive = Color.green;

        public IntReferenceDisplay(string prefix, IntReference reference, Color clrDefault, Color clrNegative, Color clrPositive)
        {
            this.prefix = prefix;
            this.reference = reference;
            this.clrDefault = clrDefault;
            this.clrNegative = clrNegative;
            this.clrPositive = clrPositive;
        }

        public IntReferenceDisplay(IntReferenceDisplay other)
        {
            this.prefix = other.prefix;
            this.reference = other.reference;
            this.clrDefault = other.clrDefault;
            this.clrNegative = other.clrNegative;
            this.clrPositive = other.clrPositive;
        }

        public IntReferenceDisplay(IntReference reference, IntReferenceDisplay other)
        {
            this.prefix = other.prefix;
            this.reference = reference;
            this.clrDefault = other.clrDefault;
            this.clrNegative = other.clrNegative;
            this.clrPositive = other.clrPositive;
        }
    }
}
