using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class UIWithIcon : MonoBehaviour
    {
        public Image icon;

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }
    }
}
