using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace FancyToolkit
{
    public class FancyTimer : MonoBehaviour
    {
        float endTime;

        int cachedSeconds = -1;
        TMP_Text txt;

        public void Init(int seconds)
        {
            txt = GetComponent<TMP_Text>();
            endTime = Time.time + seconds;
        }

        void Update()
        {
            if (!txt) return;

            int seconds = Mathf.RoundToInt(endTime - Time.time);

            if (seconds < 0) return;

            if (seconds != cachedSeconds)
            {
                cachedSeconds = seconds;
                txt.SetText($"{seconds}");
            }
        }
    }
}