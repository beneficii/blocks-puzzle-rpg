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
        [SerializeField] AudioClip sound;

        [SerializeField] bool hideAfterUse;

        Color clrTextNormal;

        bool initDone;


        public void EnsureInit()
        {
            if (initDone) return;
            initDone = true;

            if (txtCaption) clrTextNormal = txtCaption.color;
        }

        public void SetNeedsAttention(bool value)
        {
            if (!imgHighlight) return;
            imgHighlight.gameObject.SetActive(value);
        }

        public bool IsInteractable()
        {
            return button.interactable;
        }

        public void SetInteractable(bool value)
        {
            EnsureInit();
            if (txtCaption) txtCaption.color = value ? clrTextNormal : clrTextGray;
            if (shaderComponent) shaderComponent.SetGrayscale(!value);
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

        public void OnClicked()
        {
            sound?.PlayNow();
            if (hideAfterUse)
            {
                gameObject.SetActive(false);
            }
        }
    }


}