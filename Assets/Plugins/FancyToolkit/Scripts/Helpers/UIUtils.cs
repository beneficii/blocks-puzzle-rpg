using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace FancyToolkit
{
    public static class UIUtils
    {
        public static T CreateFromTemplate<T>(T template) where T : Component
        {
            var instance = GameObject.Instantiate(template, template.transform.parent);

            return instance;
        }

        public static IEnumerable<T> IterateFromTemplate<T>(T template) where T : Component
        {
            foreach (Transform item in template.transform.parent)
            {
                if (item.gameObject.activeSelf)
                {
                    var retval = item.GetComponent<T>();
                    if (retval != null)
                    {
                        yield return retval;
                    }
                }
            }
        }

        public static void Clear<T>(T template) where T : Component
        {
            foreach (Transform item in template.transform.parent)
            {
                if (item.gameObject.activeSelf)
                {
                    GameObject.Destroy(item.gameObject);
                }
            }
        }

        public static void ScaleParent(TextMeshProUGUI text, float padding)
        {
            var parent = text.transform.parent.GetComponent<RectTransform>();

            var sizeDelta = parent.sizeDelta;
            sizeDelta.x = text.preferredWidth + padding * 2;

            parent.sizeDelta = sizeDelta;
        }

        public static Vector2 GetWorldPosition(RectTransform ui) => ui.TransformPoint(Vector3.zero);
    }
}