using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class TextStatusIcon : MonoBehaviour
    {
        public TMP_Text txtStatus;
        [SerializeField] Animator animator;
        public SpriteRenderer render;

        public void Set(int value, bool animate = true)
        {
            if (animate && animator) animator.Play("Blink");

            txtStatus.SetText($"{value}");
        }
    }
}