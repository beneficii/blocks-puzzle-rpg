using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/ShieldBash")]
    public class ShieldBash : BlockActionBase
    {
        public override string GetDescription() => $"Deal your armor value in damage";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            var damage = 0;
            var player = CombatArena.current.player;
            if (player) damage = player.GetArmor();
            
            ResCtrl<MatchStat>.current.SetIfMore(MatchStat.MaxDamage, damage);

            MakeBullet(parent)
                .SetTarget(CombatArena.current.enemy)
                .SetDamage(damage)
                .SetLaunchDelay(0.2f);
        }
    }
}
