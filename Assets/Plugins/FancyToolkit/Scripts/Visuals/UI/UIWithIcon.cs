﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class UIWithIcon : MonoBehaviour
    {
        public Image bg;
        public Image icon;
        public Image frame;

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }

        public void SetBg(Sprite sprite)
        {
            bg.sprite = sprite;
        }
    }
}
