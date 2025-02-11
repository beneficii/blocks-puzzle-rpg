using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FancyToolkit
{
    public class UIInfoPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtDescription;
        [SerializeField] Image imgIcon;

        public void InitText(IInfoTextProvider infoProvider)
        {
            if (!txtDescription) return;

            int fontSize = Mathf.RoundToInt(txtDescription.fontSize);
            txtDescription.text = infoProvider.GetInfoText(fontSize);
        }

        public void InitHintText(IHintProvider infoProvider)
        {
            if (!txtDescription) return;

            txtDescription.text = infoProvider.GetHintText();
        }

        public void InitIcon(IIconProvider iconProvider)
        {
            if (!imgIcon) return;

            imgIcon.sprite = iconProvider?.GetIcon();
        }
    }
}