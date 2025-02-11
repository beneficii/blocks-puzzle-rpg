using FancyToolkit;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FancyToolkit
{
    public class BasicInfo : MonoBehaviour, IInfoTextProvider, IHoverInfoTarget
    {
        [SerializeField] string title;
        [SerializeField] string description;
        public string GetInfoText(int size)
        {
            var sb = new StringBuilder();

            sb.AppendLine(title
                .Center()
                .Bold());

            sb.AppendLine();
            sb.AppendLine(description);

            return sb.ToString();
        }

        public bool ShouldShowHoverInfo() => true;
    }
}
