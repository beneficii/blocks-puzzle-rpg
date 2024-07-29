using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public interface IHasDescription
    {
        string GetDescription();
    }

    public interface IHasTitle
    {
        string GetTitle();
    }

    public interface IHasIcon
    {
        Sprite GetIcon();
    }
}
