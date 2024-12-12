#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class AutoAssignFontSwitcher : MonoBehaviour
    {
        [SerializeField] List<PixelTextStyle> styles;

        [ContextMenu("Assign FontSwitchers ")]
        public void IterateSelected()
        {

            Object[] selectedObjects = Selection.objects;

            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No items selected!");
                return;
            }

            foreach (var obj in selectedObjects)
            {
                if (obj is not GameObject go) continue;
                var txtComponent = go.GetComponent<TMP_Text>();
                if (!txtComponent) continue;

                var switcher = go.GetComponent<FontSwitcher>();
                if (!switcher) switcher = go.AddComponent<FontSwitcher>();

                foreach (var item in styles)
                {
                    if (item.Match(txtComponent))
                    {
                        switcher.textStyle = item;
                    }
                }
            }
        }
    }
}
#endif