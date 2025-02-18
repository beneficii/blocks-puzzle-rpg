using FancyToolkit;
using System.Collections;
using UnityEngine;

namespace ActionHint
{
    public class Clear : IHintProvider
    {
        public string GetHintText()
        {
            return $"<b>Clear</b> - Triggered when this tile is cleared by filling a line";
        }
    }

    public class Place : IHintProvider
    {
        public string GetHintText()
        {
            return $"<b>Place</b> - Triggered when the tile is placed on the board";
        }
    }

    public class Erase : IHintProvider
    {
        public string GetHintText()
        {
            return $"<b>Erase</b> - Make ocupied tiles empty";
        }
    }

    public class Cleared : IHintProvider
    {
        public string GetHintText()
        {
            return $"<b>Cleared</b> - Tiles cleared together with this";
        }
    }
}