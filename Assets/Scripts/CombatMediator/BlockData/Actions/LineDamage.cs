using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/LineDamage")]
    public class LineDamage : BlockActionBase
    {
        [SerializeField] int damage;

        public override string GetDescription() => $"Deal {damage} damage for each matched line";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            var totalDamage = (info.columnsMatched + info.rowsMatched) * damage;
            ResCtrl<MatchStat>.current.SetIfMore(MatchStat.MaxDamage, totalDamage);

            MakeBullet(parent)
                .SetTarget(CombatArena.current.enemy)
                .SetDamage(totalDamage)
                .SetLaunchDelay(0.2f);
        }
    }
}
