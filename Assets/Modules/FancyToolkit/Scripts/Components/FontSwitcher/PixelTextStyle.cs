using System.Collections;
using UnityEngine;
using TMPro;

namespace FancyToolkit
{
    [CreateAssetMenu( menuName = "FancyToolkit/TextStyle")]
    public class PixelTextStyle : ScriptableObject
    {
        [SerializeField] TMP_FontAsset normalFont;
        [SerializeField] TMP_FontAsset pixelFont;

        public void SetFont(TMP_Text text, bool pixel)
        {
            text.font = pixel ? pixelFont : normalFont;
        }

        public bool Match(TMP_Text text)
        {
            return text.font == normalFont || text.font == pixelFont;
        }
    }
}