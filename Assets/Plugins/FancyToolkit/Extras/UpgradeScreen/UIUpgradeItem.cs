using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public class UIUpgradeItem : MonoBehaviour
    {
        public System.Action<UIUpgradeItem> onSelect;

        public void Select()
        {
            onSelect?.Invoke(this);
        }
    }
}