using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class UIMultiImage : MonoBehaviour
    {
        [SerializeField] List<Image> images = new();

        public Sprite sprite
        {
            set
            {
                foreach (Image image in images) image.sprite = value;
            }
        }
    }
}