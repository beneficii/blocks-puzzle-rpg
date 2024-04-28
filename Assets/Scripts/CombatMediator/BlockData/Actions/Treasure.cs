using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/Treasure")]
    public class Treasure : BlockActionBase
    {
        [SerializeField] BtUpgradeRarity rarity;

        public override string GetDescription() => $"Gives {rarity} shape upgrade";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            BtUpgradeCtrl.Show(rarity, 3);
        }
    }
}
