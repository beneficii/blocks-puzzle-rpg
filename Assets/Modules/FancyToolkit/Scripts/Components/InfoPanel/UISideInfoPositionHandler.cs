using System.Collections;
using UnityEngine;

namespace FancyToolkit
{
    public class UISideInfoPositionHandler : MonoBehaviour
    {
        private RectTransform rectTransform;

        int cachedX = -1;

        int GetX() => Input.mousePosition.x > Screen.width * 0.5f ? 0 : 1;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            PositionUIElement();
        }

        private void PositionUIElement()
        {
            int x = GetX();
            if (x == cachedX) return;
            cachedX = x;

            rectTransform.anchorMin = new Vector2(x, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(x, rectTransform.anchorMax.y);
            rectTransform.pivot = new Vector2(x, rectTransform.pivot.y);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private void Update()
        {
            PositionUIElement();
        }
    }
}