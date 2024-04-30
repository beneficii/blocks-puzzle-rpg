using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/DamageCopy")]
    public class CopyTopDamage : BlockActionBase
    {
        public override string GetDescription() => $"Deals highest damage in this combo again";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            int damage = ResCtrl<MatchStat>.current.Get(MatchStat.MaxDamage);

            var bullet = MakeBullet(parent)
                            .SetTarget(CombatArena.current.enemy)
                            .SetDamage(damage)
                            .SetLaunchDelay(0.5f);
        }
    }
}
