using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FancyToolkit;

namespace UpgradeSystemCards
{
    public class UIUpgradeCard : MonoBehaviour
    {
        [SerializeField] Button btnBuy;
        [SerializeField] TextMeshProUGUI txtButtonCaption;

        UIUpgradePanel parent;

        UpgradeCurrency currency;
        int price;

        public void SetParent(UIUpgradePanel parent)
        {
            this.parent = parent;
        }

        private void OnEnable()
        {
            ResCtrl<UpgradeCurrency>.OnChanged += HandleResource;
        }

        private void OnDisable()
        {
            ResCtrl<UpgradeCurrency>.OnChanged -= HandleResource;
        }

        public void RefreshUI()
        {
            bool enough = ResCtrl<UpgradeCurrency>.current.Enough(currency, price);

            btnBuy.interactable = enough;
        }

        public void Buy()
        {
            if (!ResCtrl<UpgradeCurrency>.current.Remove(currency, price))
            {
                Debug.LogError("Not enough gold");
                return;
            }

            foreach (var item in GetComponents<ICanUpgrade>())
            {
                item.Upgrade();
            }
        }

        public void SetPrice(int price, UpgradeCurrency currency = UpgradeCurrency.Gold)
        {
            this.price = price;
            this.currency = currency;
            txtButtonCaption.text = $"{price}";
            RefreshUI();
        }

        void HandleResource(UpgradeCurrency type, int value)
        {
            if (type != currency) return;

            RefreshUI();
        }
    }

    public interface ICanUpgrade
    {
        void Upgrade();
    }
}

