using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace FancyToolkit
{
    public class UIGenericButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Color colorHlAttention = Color.blue;

        [Header("References")]
        [SerializeField] TextMeshProUGUI txtCaption;
        [SerializeField] Image imgHighlight;
        [SerializeField] Button button;

        public void SetNeedsAttention(bool value)
        {
            imgHighlight.gameObject.SetActive(value);
        }

        public void SetInteractable(bool value)
        {
            var cg = GetComponent<CanvasGroup>();
            if (cg)
            {
                cg.alpha = value ? 1 : 0.3f;
            }
        }

        public void SetText(string text)
        {
            txtCaption.text = text;
        }

        public void AddOnClick(UnityAction call)
        {
            button.onClick.AddListener(call);
        }
    }


}