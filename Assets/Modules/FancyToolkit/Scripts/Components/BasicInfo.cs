using FancyToolkit;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public class BasicInfo : MonoBehaviour, IHasInfo
    {
        [SerializeField] string title;
        [SerializeField] string description;

        public string GetDescription() => description;

        public IHasInfo GetExtraInfo() => null;

        public Sprite GetIcon() => null;

        public List<string> GetTags() => new();

        public string GetTitle() => title;

        public List<string> GetTooltips() => new();

        public bool ShouldShowInfo() => true;
    }
}
