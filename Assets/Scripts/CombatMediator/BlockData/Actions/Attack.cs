using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/Attack")]
    public class Attack : BlockActionBase
    {
        [SerializeField] int damage;
        protected virtual int Damage => damage;

        public override string GetDescription() => $"Deal {Damage} damage";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            ResCtrl<MatchStat>.current.SetIfMore(MatchStat.MaxDamage, damage);

            MakeBullet(parent)
                .SetTarget(CombatArena.current.enemy)
                .SetDamage(Damage)
                .SetLaunchDelay(0.2f);
        }
    }
}
