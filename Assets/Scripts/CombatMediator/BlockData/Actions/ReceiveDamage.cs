using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyToolkit;

namespace BlockAction
{
    [CreateAssetMenu(menuName = "Game/BlockAction/ReceiveDamage")]
    public class ReceiveDamage : BlockActionBase
    {
        [SerializeField] int damage;
        protected virtual int Damage => damage;

        public override string GetDescription() => $"Receive {Damage} damage";

        public override void HandleMatch(BtBlock parent, BtLineClearInfo info)
        {
            ResCtrl<MatchStat>.current.SetIfMore(MatchStat.MaxDamage, damage);

            MakeBullet(parent)
                .SetTarget(CombatArena.current.player)
                .SetDamage(Damage)
                .SetLaunchDelay(0.2f);
        }
    }
}
