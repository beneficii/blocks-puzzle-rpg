using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    public class FontSwitcher : MonoBehaviour
    {
        const int lowResolutionThreshold = 1080;

        public PixelTextStyle textStyle;
        
        private TMP_Text textComponent;
        private bool isPixelFontActive;

        [SerializeField] bool switchSize;
        [SerializeField] int pixelSize = 100;

        int normalSize;


        void Awake()
        {
            textComponent = GetComponent<TMP_Text>();
            if (textStyle == null)
            {
                Debug.LogError("TextStyle ScriptableObject is not assigned!", gameObject);
                return;
            }

            UpdateFontBasedOnResolution();
        }

        void OnEnable()
        {
            UpdateFontBasedOnResolution();
        }

        private void UpdateFontBasedOnResolution()
        {
            if (textStyle == null || textComponent == null) return;

            bool usePixelFont = Screen.height < lowResolutionThreshold;

            if (usePixelFont == isPixelFontActive) return;

            if (switchSize)
            {
                if (usePixelFont)
                {
                    normalSize = (int)textComponent.fontSize;
                    textComponent.fontSize = pixelSize;
                }
                else
                {
                    textComponent.fontSize = normalSize;
                }
            }
            

            textStyle.SetFont(textComponent, usePixelFont);
            isPixelFontActive = usePixelFont;
        }

        private Vector2 resolution;
        private void Update()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                UpdateFontBasedOnResolution();

                resolution.x = Screen.width;
                resolution.y = Screen.height;
            }

        }
    }
}