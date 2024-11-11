using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FancyToolkit
{
    public interface IHasInfo
    {
        public Sprite GetIcon();
        public bool ShouldShowInfo();
        public string GetTitle();
        public string GetDescription();
        public List<string> GetTooltips();
    }

    public interface IHasNestedInfo
    {
        IHasInfo GetInfo();
    }
}
