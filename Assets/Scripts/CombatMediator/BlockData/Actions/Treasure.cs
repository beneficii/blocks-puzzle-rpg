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

        void Offer(Component comp)
        {
            BtUpgradeCtrl.current.Show(rarity, 3);
        }

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            MakeBullet(parent)
                .SetTarget(CombatArena.current.player)
                .SetAction(Offer)
                .SetLaunchDelay(0.4f);

        }
    }
}
