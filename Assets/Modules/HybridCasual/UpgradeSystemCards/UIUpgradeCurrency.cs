using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FancyToolkit;

namespace UpgradeSystemCards
{
    public class UIUpgradeCurrency : UIGenericResourceDisplay<UpgradeCurrency>
    {
        [SerializeField] List<Sprite> icons;
        [SerializeField] Image imgIcon;

        protected override IEnumerator Start()
        {
            var iconIdx = (int)type - 1;
            if (iconIdx >= 0 && iconIdx < icons.Count)
            {
                imgIcon.sprite = icons[iconIdx];
            }
            return base.Start();
        }
    }

    public enum UpgradeCurrency
    {
        None,
        Gold,
        Diamonds,
    }
}
