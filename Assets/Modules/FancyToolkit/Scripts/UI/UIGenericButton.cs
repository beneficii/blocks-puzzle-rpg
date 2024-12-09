using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using FancyToolkit;

namespace FancyToolkit
{
    public class UIGenericButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Color clrTextGray = Color.gray;

        [Header("References")]
        [SerializeField] TextMeshProUGUI txtCaption;
        [SerializeField] Image imgHighlight;
        [SerializeField] Button button;
        [SerializeField] UIShaderComponent shaderComponent;


        Color clrTextNormal;

        bool initDone;


        public void EnsureInit()
        {
            if (initDone) return;
            initDone = true;

            clrTextNormal = txtCaption.color;
        }

        public void SetNeedsAttention(bool value)
        {
            imgHighlight.gameObject.SetActive(value);
        }

        public void SetInteractable(bool value)
        {
            EnsureInit();
            txtCaption.color = value ? clrTextNormal : clrTextGray;
            shaderComponent.SetGrayscale(!value);
            button.interactable = value;
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